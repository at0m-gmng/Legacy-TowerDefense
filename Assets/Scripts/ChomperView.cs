using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Вьюшка анимации чомпера (маленького врага).
/// </summary>
public class ChomperView : EnemyView
{
    /// <summary>
    /// Возвращает врага в фабрику при окончании анимации.
    /// </summary>
    public void OnDieAnimationFinished()
    {
        _enemy.Recycle();
    }
}
