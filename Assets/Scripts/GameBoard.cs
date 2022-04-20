using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Transform _ground;
    [SerializeField] private GameTile _tilePrefab;
    
    private Vector2Int _size;
    private GameTile[] _tiles;
    private Queue<GameTile> _searchFrontier = new Queue<GameTile>();
    public void Init(Vector2Int size)
    {
        _size = size;
        _ground.localScale = new Vector3(size.x, size.y, 1f);

        Vector2 offset = new Vector2((size.x - 1) * .5f, (size.y - 1) * .5f);
        _tiles = new GameTile[size.x * size.y];
        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                GameTile tile = _tiles[i] = Instantiate(_tilePrefab);
                tile.transform.SetParent(transform,false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y- offset.y);

                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, _tiles[i-1]); // восточный - текущий тайл, западный тайл - 1
                }

                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighborn(tile, _tiles[i-size.x]); // северный - текущий, юный тайл - х
                }

                tile.IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0)
                    tile.IsAlternative = !tile.IsAlternative;
            }
        }
        FindPath();
    }

    public void FindPath()
    {
        foreach (var tile in _tiles)
        {
            tile.ClearPath();
        }

        int destinationIndex = _tiles.Length / 2; // середина пункт назначения
        _tiles[destinationIndex].BecomeDistanation();
        _searchFrontier.Enqueue(item: _tiles[destinationIndex]);

        while (_searchFrontier.Count > 0)
        {
            GameTile tile = _searchFrontier.Dequeue();
            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    _searchFrontier.Enqueue(item: tile.GrowPathNorth);
                    _searchFrontier.Enqueue(item: tile.GrowPathSouth);
                    _searchFrontier.Enqueue(item: tile.GrowPathEast);
                    _searchFrontier.Enqueue(item: tile.GrowPathWest);
                }
                else
                {
                    _searchFrontier.Enqueue(item: tile.GrowPathWest);
                    _searchFrontier.Enqueue(item: tile.GrowPathEast);
                    _searchFrontier.Enqueue(item: tile.GrowPathSouth);
                    _searchFrontier.Enqueue(item: tile.GrowPathNorth);
                }
                // _searchFrontier.Enqueue(item: tile.GrowPathNorth);
                // _searchFrontier.Enqueue(item: tile.GrowPathWest);
                // _searchFrontier.Enqueue(item: tile.GrowPathSouth);
                // _searchFrontier.Enqueue(item: tile.GrowPathEast);
            }
        }

        foreach (var tileRotation in _tiles)
        {
            tileRotation.ShowPath();
        }
    }
}
