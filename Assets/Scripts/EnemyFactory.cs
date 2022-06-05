using System;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [Serializable] class EnemyConfig
    {
        public Enemy EnemyPref; // ссылка на префаб врага
        [FloatRangeSlider(0f, 1.5f)] public FloatRange Scale = new FloatRange(0.5f); // Раздомизация размеров
        [FloatRangeSlider(-0.4f, 0.4f)] public FloatRange PathOffset = new FloatRange(0f); // смещение пути
        [FloatRangeSlider(0.2f, 3f)] public FloatRange SpeedRange = new FloatRange(1f); // диапозон скорости врагов
        [FloatRangeSlider(10f, 1000f)] public FloatRange Health = new FloatRange(1f); // диапозон скорости врагов
    }

    [SerializeField] private EnemyConfig _small, _medium, _large;

    //[SerializeField] private Enemy _enemyPref; // ссылка на префаб врага
    //[SerializeField, FloatRangeSlider(0f, 1.5f)]
    //private FloatRange _scale = new FloatRange(0.5f); // Раздомизация размеров
    //[SerializeField, FloatRangeSlider(-0.4f, 0.4f)]
    //private FloatRange _pathOffset = new FloatRange(0f); // смещение пути
    //[SerializeField, FloatRangeSlider(0.2f, 3f)]
    //private FloatRange _speedRange = new FloatRange(1f); // диапозон скорости врагов

    public Enemy Get(EnemyType type) // создаёт врага
    {
        var config = GetConfig(type);
        Enemy instance = CreateGameObjectInstance(config.EnemyPref);
        instance.OriginFactory = this;
        instance.Init(config.Scale.RandomValueRange, config.PathOffset.RandomValueRange, 
                      config.SpeedRange.RandomValueRange, config.Health.RandomValueRange);
        return instance;
    }



    public void Reclaim(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    // Получение конфигурации на основе типа врага
    private EnemyConfig GetConfig(EnemyType type )
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