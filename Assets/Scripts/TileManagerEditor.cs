using UnityEngine;
using UnityEditor;

/// <summary>
/// The editor variant of the TileManager class.
/// Meant to allow developers to more easily interface with a TileManager.
/// </summary>
[CustomEditor(typeof(TileManager))]
public class TileManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Make a button for updating the tile data.
        if (GUILayout.Button("Update Tile Data"))
        {
            TileManager tileManager = (TileManager)target;
            tileManager.UpdateTileData();
        }
    }
}
