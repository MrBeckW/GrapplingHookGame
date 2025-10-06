using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The manager of all the data relating to the tiles.
/// </summary>
public class TileManager : MonoBehaviour
{
    /// <summary>
    /// Represents an item in the TileData dictionary.
    /// Mainly just meant for allowing the modification of the data dictionary within the editor.
    /// </summary>
    [System.Serializable]
    private struct TileDataItem
    {
        /// <summary>
        /// The tile that will have said data.
        /// </summary>
        [SerializeField]
        private TileBase _tile;

        /// <summary>
        /// The tile that will have said data.
        /// </summary>
        public TileBase Tile { get => _tile; }

        /// <summary>
        /// The data of the tile.
        /// </summary>
        [SerializeField]
        private TileData _tileData;

        /// <summary>
        /// The data of the tile.
        /// </summary>
        public TileData TileData { get => _tileData; }
    }

    /// <summary>
    /// The tile map to store the data for.
    /// </summary>
    private Tilemap _tilemap;

    /// <summary>
    /// The tile data items to modify in the editor.
    /// </summary>
    [SerializeField]
    private TileDataItem[] _tileDataItems;
    
    /// <summary>
    /// The tile data dictionary used to access various info about tiles within the editor.
    /// </summary>
    private Dictionary<TileBase, TileData> _tileData;

    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
        UpdateTileData();
    }

    /// <summary>
    /// Gets the tile data of the specified tile.
    /// </summary>
    /// <param name="tile">
    /// The tile to get the data of.
    /// </param>
    /// <returns>
    /// The tile data of the given tile.
    /// </returns>
    public TileData GetTileData(TileBase tile) => _tileData[tile];

    /// <summary>
    /// Gets the tile data at the specified tile position.
    /// </summary>
    /// <param name="pos">
    /// The tile position to get the tile data from.
    /// </param>
    /// <returns>
    /// The tile data at the given tile position.
    /// </returns>
    public TileData GetTileData(Vector3Int pos) => _tileData[_tilemap.GetTile(pos)];

    /// <summary>
    /// Gets the tile data at the specified global position.
    /// </summary>
    /// <param name="pos">
    /// The global position to get the tile from.
    /// </param>
    /// <returns>
    /// The tile data at the given global position.
    /// </returns>
    public TileData GetTileData(Vector3 pos)
    {
        Vector3 cellSize = _tilemap.cellSize;
        Vector3Int cellPos = new Vector3Int(Mathf.FloorToInt(pos.x / cellSize.x), Mathf.FloorToInt(pos.y / cellSize.y), 0);
        TileBase tileBase = _tilemap.GetTile(cellPos);

        return tileBase ? _tileData[tileBase] : new TileData(true);
    }

    /// <summary>
    /// Updates the tile data dictionary to be of the tile data items array.
    /// </summary>
    public void UpdateTileData()
    {
        _tileData = new();

        // Go through each tile data item, and add it's info to the tile data dictionary.
        for (int i = 0; i < _tileDataItems.Length; i++)
            _tileData.Add(_tileDataItems[i].Tile, _tileDataItems[i].TileData);

        System.Array.Clear(_tileDataItems, 0, _tileDataItems.Length);
    }
}
