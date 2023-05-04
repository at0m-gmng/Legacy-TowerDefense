using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Вьюшка анимации гренадера (среднего врага).
/// </summary>
public class GrenadierView : EnemyView
{
    /// <summary>
    /// Возвращает врага в фабрику при окончании анимации.
    /// </summary>
    public void OnDieAnimationFinished()
    {
        _enemy.Recycle();
    }
}
