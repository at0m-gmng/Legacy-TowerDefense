
/// <summary>
/// Вьюшка анимации голема.
/// </summary>
public class GolemView: EnemyView
{
    /// <summary>
    /// Возвращает врага в фабрику при окончании анимации.
    /// </summary>
    public void OnDieAnimationFinished()
    {
        _enemy.Recycle();
    }
}