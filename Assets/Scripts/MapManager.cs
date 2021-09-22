using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    protected Tilemap tilemap;

    [SerializeField]
    protected List<TileData> tilesData;

    private readonly Dictionary<TileBase, TileData> tilesDataLookup = new Dictionary<TileBase, TileData>();

    private void Awake()
    {
        foreach (var tileData in tilesData)
        {
            foreach (var tile in tileData.tiles)
            {
                tilesDataLookup.Add(tile, tileData);
            }
        }
    }

    public TileData GetTileData(Vector3 positionWS)
    {
        Vector3Int coords = tilemap.WorldToCell(positionWS);
        return GetTileData(coords);
    }

    public TileData GetTileData(Vector3Int coords)
    {
        TileBase tile = tilemap.GetTile(coords);
        if (tile == null) return null;
        if (!tilesDataLookup.TryGetValue(tile, out TileData tileData))
        {
            return null;
        }
        return tileData;
    }
}
