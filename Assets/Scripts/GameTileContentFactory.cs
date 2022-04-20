using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// фабрика для контент тайлов
[CreateAssetMenu]
public class GameTileContentFactory : ScriptableObject
{
    [SerializeField] private GameTileContent _destinationPref; // объекты типо контента
    [SerializeField] private GameTileContent _emptyPref;
    [SerializeField] private GameTileContent _wall;
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
            
        }

        return null;
    }

    private GameTileContent Get(GameTileContent prefab) // принимает префаб и создаёт объект контента
    {
        GameTileContent instance = Instantiate(prefab);
        instance.OriginFactory = this;
        MoveToFactoryScene(instance.gameObject);
        return instance; // возвращаем объект контента
    }
    // фабрика будет содержать контент в отдельной сцене, которая будет подгружена в основную
    private Scene _contentScene;

    private void MoveToFactoryScene(GameObject o)
    {
        if (!_contentScene.isLoaded)
        {
            if (Application.isEditor) // прежде чем создавать сцену, проверим, существует ли она
            {
                _contentScene = SceneManager.GetSceneByName(name);
                if (!_contentScene.isLoaded)
                {
                    _contentScene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                _contentScene = SceneManager.CreateScene(name);  
            }
        }
        
        SceneManager.MoveGameObjectToScene(o, _contentScene);
    }
}
