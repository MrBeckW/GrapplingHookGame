using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The data that a tile contains.
/// </summary>
[System.Serializable]
public struct TileData
{
    /// <summary>
    /// Whether or not the tile can be hooked onto.
    /// </summary>
    [SerializeField]
    private bool _canHook;

    /// <summary>
    /// Whether or not the tile can be hooked onto.
    /// </summary>
    public bool CanHook { get => _canHook; }

    /// <summary>
    /// Initializes the TileData object.
    /// </summary>
    /// <param name="canHook">
    /// Whether or not the tile can be hooked onto.
    /// </param>
    public TileData(bool canHook)
    {
        _canHook = canHook;
    }
}
