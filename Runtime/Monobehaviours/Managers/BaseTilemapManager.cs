using System;
using System.Collections.Generic;
using System.Linq;
using Models.CampaignEditor;
using Models.Gameplay.Campaign;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Gameplay.Tiles;
using Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class BaseTilemapManager : MonoBehaviour
{
    public Grid grid;
    public Tilemap landmassTilemap;
    public Tilemap supplyTilemap;
    public Tilemap cityTilemap;
    public Tilemap airportTilemap;
    public Tilemap fortTilemap;
    public Tilemap electricityTilemap;
    public Tilemap oilTilemap;
    public Tilemap factoryTilemap;
    public Tilemap steelTilemap;
    public Tilemap overlayTilemap;
    public Tilemap controlTilemap;
    public List<LandmassTiles> availableLandTiles;
    public List<HZPLTile> availableOtherTiles;

    public HZPLTile supplyNodeTile;
    public HZPLTile supplyLineTile;
    public HZPLTile cityTile;
    public HZPLTile suburbTile;
    public HZPLTile airportTile;
    public HZPLTile fortTile;
    public HZPLTile electricityTile;
    public HZPLTile oilTile;
    public HZPLTile factoryTile;
    public HZPLTile steelTile;
    public TileBase overlayTile;
    public TileBase ControlTile;
    public Vector3 GetCellCenterWorld(Vector3Int gridPosition) => grid.GetCellCenterWorld(gridPosition);

    public float overlayAlphaValue = 0.3f;
    private Dictionary<Vector3Int, HZPLTileData> TileData;
    private List<Area> Areas;
    private HashSet<Vector3Int> AirportCells = new HashSet<Vector3Int>();
    public List<HZPLTerrain> terrainTypes;

    public RiverPolylineOverlay riverOverlay;

    public virtual void OnEnable()
    {
        riverOverlay = GetComponent<RiverPolylineOverlay>();
        if (riverOverlay == null)
        {
            var go = new GameObject("RiverPolylineOverlay");
            go.transform.SetParent(transform, false);

            riverOverlay = go.AddComponent<RiverPolylineOverlay>();
            riverOverlay.grid = grid;
        }
    }

    public virtual void OnDisable()
    {
        
    }

    
    public void UpdateTile(Vector3Int cellPos)
    {
        var data = TileData[cellPos];
        var terrainData = terrainTypes.FirstOrDefault(p => p.ID == data.terrainID);
        if (data.landmassTileID != Guid.Empty)
        {
            var landmassNodeTile = availableLandTiles.First(p => p.ID == data.landmassTileID);
            landmassTilemap.SetTile(cellPos, landmassNodeTile);
            if (data.LandTile != landmassNodeTile.LandTile)
            {
                data.areaId = Guid.Empty;
            }

            data.LandTile = landmassNodeTile.LandTile;
        }
        else
        {
            landmassTilemap.SetTile(cellPos, null);
            data.LandTile = false;
        }

        if (data.infrastructure.isSupplyHub)
        {
            supplyTilemap.SetTile(cellPos, supplyNodeTile);
        }
        else if (data.infrastructure.supplyLineLevel > 0)
        {
            supplyTilemap.SetTile(cellPos, supplyLineTile);
        }
        else
        {
            supplyTilemap.SetTile(cellPos, null);
        }

        switch (data.infrastructure.cityType)
        {
            case CityType.Metropolitan:
                cityTilemap.SetTile(cellPos, cityTile);
                break;
            case CityType.Suburb:
                cityTilemap.SetTile(cellPos, suburbTile);
                break;
            default:
                cityTilemap.SetTile(cellPos, null);
                break;
        }

        airportTilemap.SetTile(cellPos, AirportCells.Contains(cellPos) ? airportTile : null);

        fortTilemap.SetTile(cellPos, data.infrastructure.fortificationLevel > 0 ? fortTile : null);

        electricityTilemap.SetTile(cellPos, data.infrastructure.electricityLevel > 0 ? electricityTile : null);

        oilTilemap.SetTile(cellPos, data.infrastructure.oilLevel > 0 ? oilTile : null);

        factoryTilemap.SetTile(cellPos, data.infrastructure.factoryLevel > 0 ? factoryTile : null);

        steelTilemap.SetTile(cellPos, data.infrastructure.steelLevel > 0 ? steelTile : null);

        landmassTilemap.SetTileFlags(cellPos, TileFlags.None);

        if (terrainData != null)
        {
            float encodedAlpha = EncodeTwoIndices(terrainData.terrainSpriteIndex, 0);

            var tileColor = new Color(1, 1, 1, encodedAlpha);
            landmassTilemap.SetColor(cellPos, tileColor);
        }

        overlayTilemap.SetTile(cellPos, overlayTile);
        overlayTilemap.SetTileFlags(cellPos, TileFlags.None);
        if (data.areaId != Guid.Empty)
        {
            Color color = Areas.FirstOrDefault(p => p.Id == data.areaId).AreaColor;
            color.a = overlayAlphaValue;
            overlayTilemap.SetColor(cellPos, color);
        }
        else
        {
            overlayTilemap.SetColor(cellPos, new Color(1, 1, 1, overlayAlphaValue));
        }

        if (data.LandTile)
        {
            controlTilemap.SetTile(cellPos, ControlTile);
            controlTilemap.SetTileFlags(cellPos, TileFlags.None);
            Color color = Color.white;
            if(data.controllingAlliance == Alliance.BlueFor)
                color = Color.blue;
            if(data.controllingAlliance == Alliance.RedFor)
                color = Color.red;
            color.a = overlayAlphaValue;
            controlTilemap.SetColor(cellPos, color);
        }
        else
        {
            controlTilemap.SetTile(cellPos, null);
        }
    }

    public void SetCampaign(Dictionary<Vector3Int, HZPLTileData> tileData,
        List<Area> areas,
        List<AirportDefinition> airports = null)
    {
        TileData = tileData;
        Areas = areas;
        AirportCells = airports != null
            ? new HashSet<Vector3Int>(airports.Where(airport => airport != null).Select(airport => airport.Tile))
            : new HashSet<Vector3Int>();
        RefreshTilemaps();
    }

    public void RefreshTilemaps()
    {
        landmassTilemap.ClearAllTiles();
        supplyTilemap.ClearAllTiles();
        cityTilemap.ClearAllTiles();
        airportTilemap.ClearAllTiles();
        fortTilemap.ClearAllTiles();
        electricityTilemap.ClearAllTiles();
        oilTilemap.ClearAllTiles();
        factoryTilemap.ClearAllTiles();
        steelTilemap.ClearAllTiles();
        overlayTilemap.ClearAllTiles();
        controlTilemap.ClearAllTiles();
        foreach (var kvp in TileData)
        {
            UpdateTile(kvp.Key);
        }

        riverOverlay.RebuildAll(TileData);
        overlayTilemap.enabled = false;
        controlTilemap.enabled = false;
    }

    private float EncodeTwoIndices(int index1, int index2)
    {
        // Clamp to 0-15 range
        index1 = Mathf.Clamp(index1, 0, 15);
        index2 = Mathf.Clamp(index2, 0, 15);

        // Combine into 0-255 range
        int combined = (index1 << 4) | index2;

        // Map to 0.1-1.0 range to avoid division issues
        return 0.1f + (combined / 255f) * 0.9f;
    }
}
