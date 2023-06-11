using UnityEngine;

[CreateAssetMenu]
public abstract class EnemyFactory : GameObjectFactory
{
    public Enemy Get(EnemyType type) // создаёт врага
    {
        var config = GetConfig(type);
        Enemy instance = CreateGameObjectInstance(config.EnemyPref);
        instance.OriginFactory = this;
        instance.Init(config.Scale.RandomValueRange, config.PathOffset.RandomValueRange, config.SpeedRange.RandomValueRange, config.Health.RandomValueRange);
        return instance;
    }

    public void Reclaim(Enemy enemy) => Destroy(enemy.gameObject);
    
    protected abstract EnemyConfig GetConfig(EnemyType type);
}