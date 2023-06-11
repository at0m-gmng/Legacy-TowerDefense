using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

//отвечает за поле
public class GameBoard : MonoBehaviour
{
    [SerializeField] private Transform _ground; // объект поля
    [SerializeField] private GameTile _tilePrefab; //ссылка на клетку поля
    
    private Vector2Int _size; // размер поля
    private GameTile[] _tiles; // хранит созданные tile
    private Queue<GameTile> _searchFrontier = new Queue<GameTile>();
    private List<GameTile> _spawnPoints = new List<GameTile>(); // хранит точки спавна
    private GameTileContentFactory _contentFactory; //граница поиска //клетки, добавленные к пути, но ещё не увеличившие путь
    private List<GameTileContent> _contentToUpdate = new List<GameTileContent>();
    public int SpawnPointCount => _spawnPoints.Count;

    // генерируем поле
    public void Init(Vector2Int size, GameTileContentFactory contentFactory) // добавим пустой контент из фабрики всем клеткам
    {
        _size = size;
        _ground.localScale = new Vector3(size.x, size.y, 1f);

        Vector2 offset = new Vector2((size.x - 1) * .5f, (size.y - 1) * .5f); //смещение от начала координат/центра поля
        _tiles = new GameTile[size.x * size.y]; // инициализируем массив по размеру поля
        _contentFactory = contentFactory;
        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                GameTile tile = _tiles[i] = Instantiate(_tilePrefab); //создаём копию клетки и присваиваем по индексу
                tile.transform.SetParent(transform,false); //  делаем её дочерней у поля
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y- offset.y); // устанавливаем в нужную позицию

                if (x > 0) //проверка, чтобы не выйти за пределы поля
                {
                    GameTile.MakeEastWestNeighbors(tile, _tiles[i-1]); // восточный - текущий тайл, западный тайл - 1
                }

                if (y > 0) //проверка, чтобы не выйти за пределы поля
                {
                    GameTile.MakeNorthSouthNeighborn(tile, _tiles[i-size.x]); // северный - текущий, юный тайл - х
                }

                tile.IsAlternative = (x & 1) == 0; // при чётном х свойство IsAlternative true
                if ((y & 1) == 0) // для у наоборот, для создания шахматного порядка
                    tile.IsAlternative = !tile.IsAlternative;

            }
        }
        Clear();
    }

    public void GameUpdate()
    {
        for (int i = 0; i < _contentToUpdate.Count; i++)
        {
            _contentToUpdate[i].GameUpdate();
        }
    }

    public bool FindPath() // метод по поиску пути
    {
        foreach (var t in _tiles) // т.к путей может быть несколько, добавим их в границу поиска
        {
            if (t.Content.Type == GameTileContentType.Destination)
            {
                t.BecomeDistanation();
                _searchFrontier.Enqueue(t);
            }
            else
            {
                t.ClearPath();
            }
        }

        if (_searchFrontier.Count == 0) // если на границе поиска ничего нет, устанавливаем в false
        {
            return false;
        }
        //если в очереди не нулевые элементы, расширяемся по всем направлениям
        while (_searchFrontier.Count > 0)
        {
            GameTile tile = _searchFrontier.Dequeue();
            if (tile != null)
            {
                if (tile.IsAlternative) // меняем порядок обработки соседей
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
            }
        }
        foreach (var t in _tiles)
        {
            if (!t.HasPath) // проверяем, что у всех клеток есть путь
            {
                return false;
            }
        }
        foreach (var t in _tiles)
        {
            t.ShowPath(); // обновляем направление стрелок
        }

        return true;
    }

    public bool TryBuild(GameTile tile, GameTileContent content)
    {
        if (tile.Content.Type != GameTileContentType.Empty)
        {
            return false;
        }

        tile.Content = content;
        if (FindPath() == false)
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            return false;
        }

        _contentToUpdate.Add(content);
        if (content.Type == GameTileContentType.SpawnPoint)
        {
            _spawnPoints.Add(tile);
        }
        return true;
    }

    public GameTile GetTile(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1))
        {
            int x = (int) (hit.point.x + _size.x * 0.5f);
            int y = (int) (hit.point.z + _size.y * 0.5f);
            if (x >= 0 && x < _size.x && y >= 0 && y < _size.y)
            {
                return _tiles[x + y * _size.x];
            }
        }

        return null;
    }

    public GameTile GetSpawnPoint(int index)
    {
        return _spawnPoints[index];
    }

    public void Clear()
    {
        foreach (GameTile tile in _tiles)
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Empty);
        }
        _spawnPoints.Clear();
        _contentToUpdate.Clear();
        TryBuild(_tiles[_tiles.Length / 2], _contentFactory.Get(GameTileContentType.Destination));
        TryBuild(_tiles[0], _contentFactory.Get(GameTileContentType.SpawnPoint));
    }
}
