using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//отвечает за одну клетку на поле
public class GameTile : MonoBehaviour
{
    [SerializeField] private Transform _arrow; // стрелка направления

    private GameTile _north, _east, _south, _west, _nextOnPath; //переменные для нахождение соседей и следующей клетки на поля
    private GameTileContent _content; // ссылка на контент
    private int _distance; // кол-во ячеек до пункта назначения
    
    private Quaternion _northRotation = Quaternion.Euler(90f, 0f, 90f); // переменные вращения для каждого направления
    private Quaternion _eastRotation = Quaternion.Euler(90f, 90f, 90f);
    private Quaternion _southRotation = Quaternion.Euler(90f, 180f, 90f);
    private Quaternion _westRotation = Quaternion.Euler(90f, 270f, 90f);
    public bool HasPath => _distance != int.MaxValue; //путь назначения 
    public bool IsAlternative { get; set; } // поле для чередования клеток
    public GameTile NextTileOnPath => _nextOnPath;
    public Vector3 ExitPoint { get; private set; }
    
    public Direction PathDirection { get; private set; } // хранит направление пути
    public GameTileContent Content
    {
        get => _content;
        set
        {
            if (_content != null) // если контент уже есть, вызываем Recycle
            {
                _content.Recycle();
            }

            _content = value;
            _content.transform.localPosition = transform.localPosition; // устанавливаем позицию, идентичную локальной позиции клетки
        }
    }
    public static void MakeEastWestNeighbors(GameTile east, GameTile west) //записываем соседей
    {
        west._east = east;
        east._west = west;
    }
    public static void MakeNorthSouthNeighborn(GameTile north, GameTile south) //записываем соседей
    {
        north._south = south;
        south._north = north;
    }

    public void ClearPath() // перезагружает состояние клетки
    {
        _distance = int.MaxValue;
        _nextOnPath = null;
    }

    public void BecomeDistanation() // помечает клетку как путь назначения
    {
        _distance = 0;
        _nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    
    //вызываем GrowPathTo для каждого из соседей
    public GameTile GrowPathNorth => GrowPathTo(_north, Direction.South);
    public GameTile GrowPathSouth => GrowPathTo(_south, Direction.North);
    public GameTile GrowPathEast => GrowPathTo(_east, Direction.West);
    public GameTile GrowPathWest => GrowPathTo(_west, Direction.East);

    public void ShowPath() // отвечает за правильно отображение стрелок
    {
        if (_distance == 0)
        {
            _arrow.gameObject.SetActive(false);
            return;
        }
        _arrow.gameObject.SetActive(true);
        _arrow.localRotation =
            _nextOnPath == _north ? _northRotation :
            _nextOnPath == _east ? _eastRotation :
            _nextOnPath == _south ? _southRotation :
             _westRotation;

    }
    
    private GameTile GrowPathTo(GameTile neighbor, Direction direction) // вызывается только для клеток, имеющих _distance
    {
        if (!HasPath || neighbor == null || neighbor.HasPath)
            return null;
        
        neighbor._distance = _distance + 1;
        neighbor._nextOnPath = this;
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector(); 
        neighbor.PathDirection = direction;
        return neighbor.Content.Type != GameTileContentTipe.Wall ? neighbor : null; // не добавляем клетки со стенами в границу поиска, чтобы стены блокировали путь
    }
}
