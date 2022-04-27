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

                tile.Content = _contentFactory.Get(GameTileContentTipe.Empty); // добавим пустой контент из фабрики всем клеткам
            }
        }
        ToggleDestination(_tiles[_tiles.Length/2]); // устанавливаем при старте одну начальную позицию в центре
        ToggleSpawnPoint(_tiles[0]); // первый элемент как точка спавна по умолчанию
        FindPath();
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
        // foreach (var tile in _tiles) // обнуляем все клетки
        // {
        //     tile.ClearPath();
        // }
        //
        // int destinationIndex = _tiles.Length / 2; // середина пункт назначения
        // _tiles[destinationIndex].BecomeDistanation();
        // _searchFrontier.Enqueue(item: _tiles[destinationIndex]); // добавляем в очередь
        
        foreach (var t in _tiles) // т.к путей может быть несколько, добавим их в границу поиска
        {
            if (t.Content.Type == GameTileContentTipe.Destination)
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
                // _searchFrontier.Enqueue(item: tile.GrowPathNorth);
                // _searchFrontier.Enqueue(item: tile.GrowPathWest);
                // _searchFrontier.Enqueue(item: tile.GrowPathSouth);
                // _searchFrontier.Enqueue(item: tile.GrowPathEast);
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

    public void ToggleWall(GameTile tile) // тумблер переключения между обычной клеткой и стеной
    {
        if (tile.Content.Type == GameTileContentTipe.Wall)
        {
            tile.Content = _contentFactory.Get(GameTileContentTipe.Empty);
            FindPath();
        }
        else if(tile.Content.Type == GameTileContentTipe.Empty)
        {
            tile.Content = _contentFactory.Get(GameTileContentTipe.Wall);
            if (!FindPath()) // стены должны блокировать проверку поиска пути
            {
                tile.Content = _contentFactory.Get(GameTileContentTipe.Empty);
                FindPath();
            }
        }
    }
    public void ToggleTower(GameTile tile) // тумблер переключения между обычной клеткой и стеной
    {
        if (tile.Content.Type == GameTileContentTipe.Tower)
        {
            _contentToUpdate.Remove(tile.Content);
            tile.Content = _contentFactory.Get(GameTileContentTipe.Empty);
            FindPath();
        }
        else if(tile.Content.Type == GameTileContentTipe.Empty)
        {
            tile.Content = _contentFactory.Get(GameTileContentTipe.Tower);

            if (FindPath()) // стены должны блокировать проверку поиска пути
            {
                _contentToUpdate.Add(tile.Content);
            }
            else
            {
                tile.Content = _contentFactory.Get(GameTileContentTipe.Empty);
                FindPath();
            }
        }        
        else if(tile.Content.Type == GameTileContentTipe.Wall)
        {
            tile.Content = _contentFactory.Get(GameTileContentTipe.Tower);
            _contentToUpdate.Add(tile.Content);
        }
    }
    public void ToggleSpawnPoint(GameTile tile) // тумблер переключения между обычной клеткой и точкой спавна
    {
        //точки спавна не влияют на поиск пути, поэтому после их добавления не нужно ничего пересчитывать
        if (tile.Content.Type == GameTileContentTipe.SpawnPoint)
        {
            if (_spawnPoints.Count > 1) // должна быть хотя бы одна точка спавна
            {
                _spawnPoints.Remove(tile);
                tile.Content = _contentFactory.Get(GameTileContentTipe.Empty);
            }
        }
        else if(tile.Content.Type == GameTileContentTipe.Empty)
        {
            tile.Content = _contentFactory.Get(GameTileContentTipe.SpawnPoint);
            _spawnPoints.Add(tile);
        }
    }

    public void ToggleDestination(GameTile tile) // тумблер переключения между обычной клеткой и пунктом назначения
    {
        if (tile.Content.Type == GameTileContentTipe.Destination)
        {
            tile.Content = _contentFactory.Get(GameTileContentTipe.Empty);
            if (!FindPath())
            {
                tile.Content = _contentFactory.Get(GameTileContentTipe.Destination);
                FindPath();
            }
        }
        else if(tile.Content.Type == GameTileContentTipe.Empty)
        {
            tile.Content = _contentFactory.Get(GameTileContentTipe.Destination);
            FindPath();
        }
    }

    public GameTile GetTile(Ray ray) // проверяем, что пользователь нажал на клетку
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1)) // проверяем, попал ли луч во что-то
        {
            int x = (int) (hit.point.x + _size.x * .5f); // определяем клетку, в которую попали
            int y = (int) (hit.point.z + _size.y * .5f);
            if (x >= 0 && x < _size.x && y >= 0 && y < _size.y) // проверка на выход за границы поля
            {
                return _tiles[x + y * _size.x]; // преобразование точки пересечения луча в индекс клетки в массиве
            }
        }

        return null;
    }

    public GameTile GetSpawnPoint(int index)
    {
        return _spawnPoints[index];
    }
}
