using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ScriptableObjects.Gameplay.Tiles
{
    public abstract class HZPLTile : Tile
    {
        [ReadOnly] public string persistentID;
        public Guid ID => Guid.TryParse(persistentID, out var guid) ? guid : Guid.Empty;
        private void OnValidate()
        {
#if UNITY_EDITOR
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
            Guid tryparse = Guid.TryParse(guid, out var guid2) ? guid2 : Guid.Empty;
            if (string.IsNullOrEmpty(persistentID) || persistentID != tryparse.ToString())
            {
                persistentID = tryparse.ToString();
            }
#endif
        }
    }
}