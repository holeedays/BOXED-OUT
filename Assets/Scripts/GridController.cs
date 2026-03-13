using System.Drawing.Text;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    // We're making our own type for gameobject based tiles
    public struct Tile3D 
    {
        public Tile3D(Tilemap tilemap, GameObject gameObject, Vector3Int cellPos)
        {
            Tmap = tilemap;
            GameObj = gameObject;
            CellPos = cellPos;
        }

        public Tilemap Tmap;
        public GameObject GameObj;
        public Vector3Int CellPos;
        public Vector3 WorldPos
        {
            get { return GridToWorldPos(Tmap, CellPos); }
            private set {; }
        }
    }

    // NOTE: The following method is based on a grid using the XZY swizzle so X and Y values are now attributed to X and Z respectively
    // Get position of grid in the world position
    public static Vector3 GridToWorldPos(Tilemap tmap, Vector3Int cellPos)
    {
        // it's a bit weird by anchors are different depending on which brush you are using, though their default value is (0.5f, 0.5f, 0f)
        // for ex: gameobjs made thru gameobj brush are not affected by changing anchor values in the tilemap (tile anchors), instead
        // you have to change the anchor in the game obj brush editor in tile palette

        // this just means we have to manually adjust the anchor... sigh
        Vector3 anchor = new Vector3(0.5f, 0, 0.5f);
        return tmap.CellToWorld(cellPos) + anchor;
    }

    // Get position of grid we're in
    public static Vector3Int WorldToGridPos(Tilemap tmap, Vector3 worldPos)
    {
        return tmap.WorldToCell(worldPos);
    }

    // Get the grid parent of tilemap
    public static Grid GetGrid(Tilemap tmap)
    {
        return tmap.transform.parent.GetComponent<Grid>();
    }

    // Get the cell size of the tilemap we're in
    public static Vector3 GetTileMapCellSize(Tilemap tmap)
    {
        return GetGrid(tmap).cellSize;
    }

    // since 3D gameobjects instantiated with a gameobject brush are only children, and not type of tiles, we can't check through tilemap.GetTile()
    // so we make our own custom function to detect it; returns a custom Tile3D type (nullable)
    public static Tile3D? GetTile3D(Tilemap tmap, Vector3Int cellPos)
    {
        Vector3 worldPos = tmap.CellToWorld(cellPos);
        Vector3 marginalExtent = Vector3.one * 0.5f;

        Collider[] colliders = Physics.OverlapBox(worldPos, marginalExtent, Quaternion.identity, LayerMask.GetMask("Default"));
        foreach (Collider col in colliders)
        {
            if (col.transform.parent == tmap.transform)
            {
                return new Tile3D(tmap, col.gameObject, cellPos);
            }
        }

        return null;
    }
}

