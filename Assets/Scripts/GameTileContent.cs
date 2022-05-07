using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// типы клеток/контента
public enum GameTileContentTipe
{
    Empty,
    Destination,
    Wall,
    SpawnPoint,
    Tower
}
// типы башен
public enum TowerType
{
    Laser,
    Mortar
}
// отвечает за тип ячейки
[SelectionBase]
public class GameTileContent : MonoBehaviour
{
    [SerializeField] private GameTileContentTipe _type;
    
    public GameTileContentTipe Type => _type;
    public GameTileContentFactory OriginFactory { get; set; } //ссылка на фабрику
    public bool IsBlockingPath => Type == GameTileContentTipe.Wall || Type == GameTileContentTipe.Tower;

    public virtual void GameUpdate()
    {
        
    }

    public void Recycle() // возвращает себя за ненадобностью
    {
        OriginFactory.Reclaim(this);
    }
    
}
