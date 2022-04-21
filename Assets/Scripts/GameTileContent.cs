using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// типы клеток/контента
public enum GameTileContentTipe
{
    Empty,
    Destination,
    Wall,
    SpawnPoint
}
// отвечает за тип ячейки
public class GameTileContent : MonoBehaviour
{
    [SerializeField] private GameTileContentTipe _type;
    
    public GameTileContentTipe Type => _type;
    public GameTileContentFactory OriginFactory { get; set; } //ссылка на фабрику

    public void Recycle() // возвращает себя за ненадобностью
    {
        OriginFactory.Reclaim(this);
    }
    
}
