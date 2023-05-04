using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class EnemyWave: ScriptableObject
{
    [SerializeField] private EnemySpawnSequence[] _spawnSequences; // каждая волна содержит массив последовательности врагов

    // обновляет очереди и проверяет, когда они закончились
    [Serializable]
    public struct State // хранит текущую волну, её состояние и индекс 
    {
        private EnemyWave _wave;
        private int _index;
        private EnemySpawnSequence.State _sequence;

        public State(EnemyWave wave) 
        {
            _wave = wave;
            _index = 0;
            _sequence = _wave._spawnSequences[0].Begin(); // запускаем первую волну
        }

        public float Progress(float deltaTime) // 
        {
            deltaTime = _sequence.Progress(deltaTime);
            while(deltaTime>=0f)
            {
                if(++_index >= _wave._spawnSequences.Length) // если очередь вернула положит число
                {
                    return deltaTime;
                }
                _sequence = _wave._spawnSequences[_index].Begin(); // двигаемся к следующей волне
                deltaTime = _sequence.Progress(deltaTime);
            }
            return -1f;
        }
    }

    public State Begin() => new State(this);
}
