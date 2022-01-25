using UnityEngine;
using UnityEngine.Tilemaps;

namespace GGJ.Utility
{
    public static class TilemapHelpers
    {
        // TODO? This could be massaged into a sparse matrix representation if we run into memory issues
        //       1. Keep all TileBase objects in a single list
        //       2. Use a second list to keep track of how many tiles in each row and what their starting (x, y) is
        //       3. Use same style representation to collapse matrices down into single sparse BoardSpace matrix
        public class TileMatrix
        {
            TileBase[][] m_Tiles;
            public BoundsInt Bounds { get; }

            public int Width => m_Tiles.Length;
            public int Height => m_Tiles[0].Length;

            public TileMatrix(TileBase[][] tiles, BoundsInt bounds)
            {
                m_Tiles = tiles;
                Bounds = bounds;
            }

            //public TileBase this[int i, int j] => m_Tiles[i][j];

            public TileBase GetAt(int x, int y)
            {
                return m_Tiles[x - Bounds.xMin][y - Bounds.yMin];
            }
        }

        public static TileMatrix GetAllTilesInTilemap(Tilemap map)
        {
            map.CompressBounds();
            var bounds = map.cellBounds;
            var tiles = map.GetTilesBlock(bounds);
            var tileMatrix = new TileBase[bounds.size.x][];
            for (var x = 0; x < bounds.size.x; ++x)
            {
                tileMatrix[x] = new TileBase[bounds.size.y];
                for (var y = 0; y < bounds.size.y; ++y)
                {
                    tileMatrix[x][y] = tiles[x + y * bounds.size.x];
                }
            }

            return new TileMatrix(tileMatrix, bounds);
        }
    }
}
