using Models.Gameplay.Campaign;
using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "New Terrain Type", menuName = "Game/Terrain Type")]
    public class HZPLTerrain : ScriptableObjectWithID
    {
        public string terrainTypeName;
        [Range(1f, 1.5f)]
        public float movementPenalty;
        [Range(.5f, 1f)]
        public float attackPenalty;
        [Range(50,80)]
        public int combatWidth;
        public int terrainSpriteIndex;
    }
    
}