using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField] private Enemy _enemyPref; // ссылка на префаб врага
    [SerializeField, FloatRangeSlider(0f, 1.5f)]
    private FloatRange _scale = new FloatRange(0.5f); // Раздомизация размеров
    [SerializeField, FloatRangeSlider(-0.4f, 0.4f)]
    private FloatRange _pathOffset = new FloatRange(0f); // смещение пути
    [SerializeField, FloatRangeSlider(0.2f, 3f)]
    private FloatRange _speedRange = new FloatRange(1f); // диапозон скорости врагов

    public Enemy Get() // создаёт врага
    {
        Enemy instance = CreateGameObjectInstance(_enemyPref);
        instance.OriginFactory = this;
        instance.Init(_scale.RandomValueRange, _pathOffset.RandomValueRange, _speedRange.RandomValueRange);
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
}