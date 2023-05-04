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
    [SerializeField] private Tower[] _towerPrefs;
    public void Reclaim(GameTileContent content) // передаётся контент, который уже не нужен
    {
        Destroy(content.gameObject);
    }

    public GameTileContent Get(GameTileContentTipe type) // вызываем Get с нужным префабом по типу
    {
        switch (type)
        {   
            case GameTileContentTipe.Destination :
                return Get(_destinationPref);
            case GameTileContentTipe.Empty :
                return Get(_emptyPref);
            case GameTileContentTipe.Wall :
                return Get(_wall);
            case GameTileContentTipe.SpawnPoint :
                return Get(_spawnPointPref);
            
        }

        return null;
    }

    public Tower Get(TowerType type)
    {
        Tower prefab = _towerPrefs[(int) type];
        return Get(prefab);
    }

    private T Get<T>(T prefab) where T : GameTileContent// принимает префаб и создаёт объект контента
    {
        T instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance; // возвращаем объект контента
    }

}
