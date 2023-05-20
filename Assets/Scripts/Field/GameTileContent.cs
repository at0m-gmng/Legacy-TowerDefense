using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// типы клеток/контента
public enum GameTileContentType
{
    Empty = 0,
    Destination = 1,
    SpawnPoint = 2,
    
    Ice = 10,
    Lava = 11,
    
    BeforeBlockers = 50,
    Wall = 51,
    
    BeforeAttackers = 100,
    LaserTower = 101,
    MortarTower = 102
}
// отвечает за тип ячейки
[SelectionBase]
public class GameTileContent : MonoBehaviour
{
    [SerializeField] private GameTileContentType _type;
    
    public GameTileContentType Type => _type;
    public GameTileContentFactory OriginFactory { get; set; } //ссылка на фабрику
    public bool IsBlockingPath =>  Type > GameTileContentType.BeforeBlockers;

    public virtual void GameUpdate()
    {
        
    }

    public void Recycle() // возвращает себя за ненадобностью
    {
        OriginFactory.Reclaim(this);
    }
    
}
