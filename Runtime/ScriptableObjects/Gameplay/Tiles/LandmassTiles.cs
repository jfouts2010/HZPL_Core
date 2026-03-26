using ScriptableObjects.Gameplay.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Terrain Tile", menuName = "Game/Tiles/Terrain Tile")]
public class LandmassTiles : HZPLTile
{
   public bool LandTile = false;
}