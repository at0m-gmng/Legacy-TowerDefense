using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameObjectFactory: ScriptableObject
{
    private Scene _scene; // фабрика будет содержать контент в отдельной сцене, которая будет подгружена в основную
    // метод обобщённый, передаём только типы, наследованные от MonoBehaviour
    protected T CreateGameObjectInstance<T>(T prefab) where T : MonoBehaviour
    {
        if (!_scene.isLoaded)
        {
            if (Application.isEditor) // прежде чем создавать сцену, проверим, существует ли она
            {
                _scene = SceneManager.GetSceneByName(name);
                if (!_scene.isLoaded)
                {
                    _scene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                _scene = SceneManager.CreateScene(name);
            }
        }

        T instance = Instantiate(prefab);
        SceneManager.MoveGameObjectToScene(instance.gameObject, _scene);
        return instance;
    }
}