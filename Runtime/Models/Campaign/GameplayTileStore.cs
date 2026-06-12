using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    public sealed class GameplayTileStore : IReadOnlyDictionary<Vector3Int, GameplayTile>
    {
        private readonly GameplayTile[] _tiles;
        private readonly Vector2Int _bottomLeft;
        private readonly Vector2Int _topRight;
        private readonly int _width;
        private readonly int _height;

        /// <param name="topRight">Exclusive upper bound (one past the maximum cell coordinate).</param>
        public GameplayTileStore(
            Vector2Int bottomLeft,
            Vector2Int topRight,
            IReadOnlyDictionary<Vector3Int, GameplayTile> source)
        {
            _bottomLeft = bottomLeft;
            _topRight = topRight;
            _width = Mathf.Max(0, topRight.x - bottomLeft.x);
            _height = Mathf.Max(0, topRight.y - bottomLeft.y);
            _tiles = new GameplayTile[_width * _height];

            if (source == null)
                return;

            foreach (var pair in source)
            {
                if (TryGetIndex(pair.Key, out var index))
                    _tiles[index] = pair.Value;
            }
        }

        public static GameplayTileStore FromTiles(IReadOnlyDictionary<Vector3Int, GameplayTile> source)
        {
            if (source == null || source.Count == 0)
                return new GameplayTileStore(Vector2Int.zero, Vector2Int.zero, source);

            var minX = source.Keys.Min(cell => cell.x);
            var minY = source.Keys.Min(cell => cell.y);
            var maxX = source.Keys.Max(cell => cell.x);
            var maxY = source.Keys.Max(cell => cell.y);

            return new GameplayTileStore(
                new Vector2Int(minX, minY),
                new Vector2Int(maxX + 1, maxY + 1),
                source);
        }

        public IEnumerable<Vector3Int> Keys
        {
            get
            {
                for (var index = 0; index < _tiles.Length; index++)
                {
                    if (_tiles[index] != null)
                        yield return CellFromIndex(index);
                }
            }
        }

        public IEnumerable<GameplayTile> Values => _tiles.Where(tile => tile != null);

        public int Count => _tiles.Count(tile => tile != null);

        public GameplayTile this[Vector3Int key] => GetTile(key);

        public bool ContainsKey(Vector3Int key) => TryGetIndex(key, out var index) && _tiles[index] != null;

        public bool TryGetValue(Vector3Int key, out GameplayTile value) => TryGetTile(key, out value);

        public GameplayTile GetTile(Vector3Int cell)
        {
            if (!TryGetTile(cell, out var tile))
                throw new KeyNotFoundException($"No gameplay tile exists at {cell}.");

            return tile;
        }

        public bool TryGetTile(Vector3Int cell, out GameplayTile tile)
        {
            tile = null;
            if (!TryGetIndex(cell, out var index))
                return false;

            tile = _tiles[index];
            return tile != null;
        }

        public void SetControllingAlliance(Vector3Int cell, Alliance alliance)
        {
            var tile = GetTile(cell);
            if (tile.controllingAlliance == alliance)
                return;

            tile.controllingAlliance = alliance;
        }

        public void SetInfrastructure(Vector3Int cell, TileInfrastructure infrastructure)
        {
            GetTile(cell).infrastructure = TileInfrastructureClone.Clone(infrastructure);
        }

        public void UpdateInfrastructure(Vector3Int cell, Action<TileInfrastructure> update)
        {
            if (update == null)
                return;

            var tile = GetTile(cell);
            tile.infrastructure ??= new TileInfrastructure();
            update(tile.infrastructure);
        }

        public void ApplyInfrastructureDamage(Vector3Int cell, string componentKey, int damageAmount)
        {
            if (damageAmount <= 0)
                return;

            UpdateInfrastructure(cell, infrastructure => infrastructure.ApplyComponentDamage(componentKey, damageAmount));
        }

        public Dictionary<Vector3Int, RuntimeTileData> ToRuntimeTileData()
        {
            var result = new Dictionary<Vector3Int, RuntimeTileData>();
            foreach (var pair in this)
                result[pair.Key] = RuntimeTileData.FromGameplayTile(pair.Value);

            return result;
        }

        public IEnumerator<KeyValuePair<Vector3Int, GameplayTile>> GetEnumerator()
        {
            for (var index = 0; index < _tiles.Length; index++)
            {
                var tile = _tiles[index];
                if (tile != null)
                    yield return new KeyValuePair<Vector3Int, GameplayTile>(CellFromIndex(index), tile);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private bool TryGetIndex(Vector3Int cell, out int index)
        {
            index = -1;
            var x = cell.x - _bottomLeft.x;
            var y = cell.y - _bottomLeft.y;
            if (x < 0 || y < 0 || x >= _width || y >= _height)
                return false;

            index = y * _width + x;
            return true;
        }

        private Vector3Int CellFromIndex(int index)
        {
            var y = index / _width;
            var x = index % _width;
            return new Vector3Int(_bottomLeft.x + x, _bottomLeft.y + y, 0);
        }
    }
}
