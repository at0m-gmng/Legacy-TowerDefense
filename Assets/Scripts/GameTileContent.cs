using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameTileContentTipe
{
    Empty,
    Destination,
    Wall
}

public class GameTileContent : MonoBehaviour
{
    [SerializeField] private GameTileContentTipe _type;
    
    public GameTileContentTipe Type => _type;
    public GameTileContentFactory OriginFactory { get; set; }

    public void Recycle()
    {
        OriginFactory.Reclaim(this);
    }
    
}
