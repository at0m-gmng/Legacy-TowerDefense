using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnSequence
{
    [SerializeField] private EnemyFactory _enemyFactory; // возможность создавать разные фабрики для врагов 
    [SerializeField] private EnemyType _enemyType;
    [SerializeField, Range(1, 100)] private int _amount = 1; // кол-во
    [SerializeField, Range(0.1f, 10f)] private float _cooldown = 1f;

    // обновляется и создает врагов
    [Serializable]
    public struct State
    {
        private EnemySpawnSequence _sequence;
        private int _countEnemy;
        private float _cooldown;

        public State(EnemySpawnSequence sequence)
        {
            _sequence = sequence;
            _countEnemy = 0;
            _cooldown = sequence._cooldown;
        }

        public float Progress(float deltaTime) 
        {
            _cooldown += deltaTime;
            while (_cooldown >= _sequence._cooldown) // можно ли спавнить нового врага
            {
                _cooldown -= _sequence._cooldown;
                if(_countEnemy >= _sequence._amount) // если счетчик привысил настроенный парметр возращаем + число, это будет сигнал брать следующую sequence
                {
                    return _countEnemy;
                }
                _countEnemy++;
                Game.SpawnEnemy(_sequence._enemyFactory, _sequence._enemyType);

            }

            return -1f; // если врагов спавнить еще можно, то возвращаем отрицательное значение
        }
    }
    public State Begin() => new State(this); // создаем State
}
