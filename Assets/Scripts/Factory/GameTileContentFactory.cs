using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// фабрика для контент тайлов
[CreateAssetMenu]
public class GameTileContentFactory : GameObjectFactory
{
    [SerializeField] private GameTileContent _destinationPref; // объекты типо контента
    [SerializeField] private GameTileContent _emptyPref;
    [SerializeField] private GameTileContent _wall;
    [SerializeField] private GameTileContent _spawnPointPref;
    [SerializeField] private Tower _laserTower;
    [SerializeField] private Tower _mortarTower;
    
    [SerializeField] private IceObstacle _iceObstacle;
    
    public void Reclaim(GameTileContent content) // передаётся контент, который уже не нужен
    {
        Destroy(content.gameObject);
    }

    public GameTileContent Get(GameTileContentType type) // вызываем Get с нужным префабом по типу
    {
        switch (type)
        {   
            case GameTileContentType.Destination:
                return Get(_destinationPref);
            case GameTileContentType.Empty:
                return Get(_emptyPref);
            case GameTileContentType.Wall:
                return Get(_wall);
            case GameTileContentType.SpawnPoint:
                return Get(_spawnPointPref);
            case GameTileContentType.LaserTower:
                return Get(_laserTower);
            case GameTileContentType.MortarTower:
                return Get(_mortarTower);
            case GameTileContentType.Ice:
                return Get(_iceObstacle);
        }

        return null;
    }

    private T Get<T>(T prefab) where T : GameTileContent// принимает префаб и создаёт объект контента
    {
        T instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance; // возвращаем объект контента
    }

}
