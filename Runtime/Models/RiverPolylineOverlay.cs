using System.Collections.Generic;
using Models.Gameplay.Campaign;
using UnityEngine;

namespace Models.CampaignEditor
{
    /// <summary>
    /// Builds long polylines from connected river segments,
    /// minimizing the number of LineRenderers.
    ///
    /// Rules:
    /// - Degree 2 nodes = continue path (one line)
    /// - Degree 1 nodes = endpoints (start/end)
    /// - Degree >=3 nodes = branch (start a new line for each outgoing path)
    /// - Loops (all degree 2) become a single line
    /// </summary>
    public class RiverPolylineOverlay : MonoBehaviour
    {
        public Grid grid;

        [Header("Appearance")]
        public float lineWidth = 0.08f;
        public float zOffset = -1f;
        public string sortingLayerName = "Default";
        public int sortingOrder = 50;

        public Material lineMaterial;
        public Material highlightMaterial;
        public float highlightWidth = 0.10f;

        // pool for line renderers
        private readonly Stack<LineRenderer> pool = new();
        private readonly List<LineRenderer> activeLines = new();

        // hover preview
        private LineRenderer hoverLine;

        #region Public API

        public void RebuildAll(Dictionary<Vector3Int, HZPLTileData> tileData)
        {
            ClearAllLines();

            if (tileData == null || grid == null)
                return;

            // Build graph: nodes = river corners, edges = river segments between corners
            var graph = BuildCornerGraph(tileData);

            // Walk paths and create polylines
            BuildPolylines(graph);
        }

        public void ShowHover(Vector3Int a, Vector3Int b, bool active)
        {
            if (!active)
            {
                if (hoverLine != null) hoverLine.gameObject.SetActive(false);
                return;
            }

            if (hoverLine == null)
            {
                hoverLine = CreateLineRenderer("RiverHover", true);
                hoverLine.startWidth = hoverLine.endWidth = highlightWidth;
            }

            hoverLine.gameObject.SetActive(true);

            // hover is always a single segment on the shared edge
            var pts = GetSharedEdgePoints(a, b);
            hoverLine.positionCount = 2;
            hoverLine.SetPosition(0, pts.p0);
            hoverLine.SetPosition(1, pts.p1);
        }

        public void ClearAllLines()
        {
            foreach (var lr in activeLines)
                ReturnLine(lr);

            activeLines.Clear();

            if (hoverLine != null)
                hoverLine.gameObject.SetActive(false);
        }

        #endregion

        #region Graph Structures

        private struct CornerNode
        {
            public Vector3 pos;
            public int id;
        }

        private class RiverGraph
        {
            public List<Vector3> nodePositions = new();
            public Dictionary<int, List<int>> adjacency = new();
        }

        #endregion

        #region Build Graph

        private RiverGraph BuildCornerGraph(Dictionary<Vector3Int, HZPLTileData> tileData)
        {
            var graph = new RiverGraph();

            // Map from "corner key" to node id
            var cornerNodeId = new Dictionary<CornerKey, int>();

            // Helper to fetch / create node id
            int GetNode(CornerKey key, Vector3 pos)
            {
                if (!cornerNodeId.TryGetValue(key, out int id))
                {
                    id = graph.nodePositions.Count;
                    graph.nodePositions.Add(pos);
                    graph.adjacency[id] = new List<int>();
                    cornerNodeId[key] = id;
                }
                return id;
            }

            foreach (var kvp in tileData)
            {
                var cell = kvp.Key;
                var data = kvp.Value;
                if (data == null) continue;

                for (int d = 0; d < 6; d++)
                {
                    var dir = (HexDirection)d;
                    if (!data.HasRiver(dir)) continue;

                    Vector3Int neighbor = GetNeighbor(cell, dir);
                    if (!tileData.ContainsKey(neighbor)) continue;

                    // Only build once per edge (canonical)
                    if (CompareCells(neighbor, cell) < 0) continue;

                    // Get the 2 corners this shared edge touches
                    var pts = GetSharedEdgePoints(cell, neighbor);

                    // Each endpoint corresponds to a CORNER in world space.
                    // We'll hash by world position (rounded) to unify shared corners.
                    var c0 = new CornerKey(pts.p0);
                    var c1 = new CornerKey(pts.p1);

                    int n0 = GetNode(c0, pts.p0);
                    int n1 = GetNode(c1, pts.p1);

                    graph.adjacency[n0].Add(n1);
                    graph.adjacency[n1].Add(n0);
                }
            }

            return graph;
        }

        #endregion

        #region Polyline Walk

        private void BuildPolylines(RiverGraph graph)
        {
            // Track visited edges using node pairs
            var visitedEdges = new HashSet<EdgeKey>();

            // Find all nodes that are endpoints or branch points
            var specialNodes = new List<int>();
            for (int i = 0; i < graph.nodePositions.Count; i++)
            {
                int deg = graph.adjacency[i].Count;
                if (deg != 2)
                    specialNodes.Add(i);
            }

            // Walk from each special node outward
            foreach (int start in specialNodes)
            {
                foreach (int next in graph.adjacency[start])
                {
                    var eKey = new EdgeKey(start, next);
                    if (visitedEdges.Contains(eKey)) continue;

                    List<Vector3> polyline = WalkPath(graph, start, next, visitedEdges);
                    CreatePolyline(polyline);
                }
            }

            // Handle loops: nodes all degree 2 that never got walked
            for (int i = 0; i < graph.nodePositions.Count; i++)
            {
                foreach (int next in graph.adjacency[i])
                {
                    var eKey = new EdgeKey(i, next);
                    if (visitedEdges.Contains(eKey)) continue;

                    // Walk loop
                    List<Vector3> loop = WalkPath(graph, i, next, visitedEdges, stopOnReturn: true);
                    CreatePolyline(loop);
                }
            }
        }

        private List<Vector3> WalkPath(RiverGraph graph, int start, int next, HashSet<EdgeKey> visitedEdges, bool stopOnReturn=false)
        {
            var result = new List<Vector3>();
            result.Add(graph.nodePositions[start]);

            int prev = start;
            int cur = next;

            while (true)
            {
                visitedEdges.Add(new EdgeKey(prev, cur));
                result.Add(graph.nodePositions[cur]);

                int deg = graph.adjacency[cur].Count;

                if (stopOnReturn && cur == start)
                    break;

                if (deg != 2 && cur != start)
                    break; // endpoint or branch reached

                // Choose the next node (not going backwards)
                int n0 = graph.adjacency[cur][0];
                int n1 = graph.adjacency[cur][1];
                int candidate = (n0 == prev) ? n1 : n0;

                prev = cur;
                cur = candidate;

                var edgeKey = new EdgeKey(prev, cur);
                if (visitedEdges.Contains(edgeKey))
                    break;
            }

            return result;
        }

        #endregion

        #region LineRenderer Creation

        private void CreatePolyline(List<Vector3> points)
        {
            if (points.Count < 2) return;

            var lr = GetLine();
            lr.positionCount = points.Count;

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 p = points[i];
                p.z += zOffset;
                lr.SetPosition(i, p);
            }

            activeLines.Add(lr);
        }

        private LineRenderer GetLine()
        {
            if (pool.Count > 0)
            {
                var lr = pool.Pop();
                lr.gameObject.SetActive(true);
                return lr;
            }

            return CreateLineRenderer("RiverPolyline", false);
        }

        private void ReturnLine(LineRenderer lr)
        {
            lr.gameObject.SetActive(false);
            pool.Push(lr);
        }

        private LineRenderer CreateLineRenderer(string name, bool highlight)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);

            var lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.numCapVertices = 4;
            lr.numCornerVertices = 0; // no smoothing around corners

            lr.sortingLayerName = sortingLayerName;
            lr.sortingOrder = sortingOrder;

            lr.startWidth = lr.endWidth = highlight ? highlightWidth : lineWidth;
            lr.material = highlight ? highlightMaterial : lineMaterial;

            return lr;
        }

        #endregion

        #region Geometry Helpers

        private (Vector3 p0, Vector3 p1) GetSharedEdgePoints(Vector3Int cellA, Vector3Int cellB)
        {
            Vector3 centerA = grid.GetCellCenterWorld(cellA);
            Vector3 centerB = grid.GetCellCenterWorld(cellB);

            Vector2 dir = (centerB - centerA);
            dir.Normalize();

            Vector3 cellSize = grid.cellSize;
            float r = Mathf.Max(cellSize.x, cellSize.y) * 0.5f;

            Vector3 mid = (centerA + centerB) * 0.5f;
            Vector2 perp = new Vector2(-dir.y, dir.x);

            float edgeHalf = r * 0.5f;

            Vector3 p0 = mid + (Vector3)(perp * edgeHalf);
            Vector3 p1 = mid - (Vector3)(perp * edgeHalf);

            return (p0, p1);
        }

        private Vector3Int GetNeighbor(Vector3Int cell, HexDirection dir)
        {
            bool isEvenRow = cell.y % 2 == 0;

            return dir switch
            {
                HexDirection.E => cell + new Vector3Int(1, 0, 0),
                HexDirection.W => cell + new Vector3Int(-1, 0, 0),

                HexDirection.NE => isEvenRow ? cell + new Vector3Int(0, 1, 0) : cell + new Vector3Int(1, 1, 0),
                HexDirection.NW => isEvenRow ? cell + new Vector3Int(-1, 1, 0) : cell + new Vector3Int(0, 1, 0),

                HexDirection.SE => isEvenRow ? cell + new Vector3Int(0, -1, 0) : cell + new Vector3Int(1, -1, 0),
                HexDirection.SW => isEvenRow ? cell + new Vector3Int(-1, -1, 0) : cell + new Vector3Int(0, -1, 0),

                _ => cell
            };
        }

        private static int CompareCells(Vector3Int a, Vector3Int b)
        {
            if (a.x != b.x) return a.x.CompareTo(b.x);
            if (a.y != b.y) return a.y.CompareTo(b.y);
            return a.z.CompareTo(b.z);
        }

        #endregion

        #region Hash Keys

        private readonly struct EdgeKey
        {
            private readonly int a;
            private readonly int b;

            public EdgeKey(int a, int b)
            {
                this.a = Mathf.Min(a, b);
                this.b = Mathf.Max(a, b);
            }

            public override int GetHashCode() => (a * 397) ^ b;
            public override bool Equals(object obj) => obj is EdgeKey other && other.a == a && other.b == b;
        }

        private readonly struct CornerKey
        {
            private readonly int x;
            private readonly int y;

            public CornerKey(Vector3 pos)
            {
                // quantize to avoid floating precision mismatch
                x = Mathf.RoundToInt(pos.x * 10f);
                y = Mathf.RoundToInt(pos.y * 10f);
            }

            public override int GetHashCode() => (x * 397) ^ y;
            public override bool Equals(object obj) => obj is CornerKey other && other.x == x && other.y == y;
        }

        #endregion
    }
}
