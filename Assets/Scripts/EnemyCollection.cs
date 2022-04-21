using System;
using System.Collections.Generic;

// ведёт список живых врагов и удаляет мёртвых
[Serializable]
public class EnemyCollection
{
    private List<Enemy> _enemies = new List<Enemy>();

    public void Add(Enemy enemy) //для добавления врага
    {
        _enemies.Add(enemy);
    }

    public void GameUpdate() //для обновления всей коллекции
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (!_enemies[i].GameUpdate())
            {
                int lastIndex = _enemies.Count - 1;
                _enemies[i] = _enemies[lastIndex];
                _enemies.RemoveAt(lastIndex);
                i -= 1;
            }
        }
    }
}