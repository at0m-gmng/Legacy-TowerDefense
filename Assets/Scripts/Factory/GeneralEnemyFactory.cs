using UnityEngine;

public class GeneralEnemyFactory : EnemyFactory
{
    [SerializeField] private EnemyConfig _small, _medium, _large;

    // Получение конфигурации на основе типа врага
    protected override EnemyConfig GetConfig(EnemyType type )
    {
        switch(type)
        {
            case EnemyType.Large:
                return _large;
            case EnemyType.Medium:
                return _medium;
            case EnemyType.Small:
                return _small;
        }
        Debug.LogError($"No config for {type}");
        return _medium;
    }
}
