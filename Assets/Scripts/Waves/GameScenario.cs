using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameScenario: ScriptableObject
{
    [SerializeField] private EnemyWave[] _waves;

    // обновляет очереди и проверяет, когда они закончились
    [Serializable]
    public struct State 
    {
        private GameScenario _scenario;
        private int _index;
        private EnemyWave.State _wave;

        public (int currentWave, int wavesCount) GetWaves()
        {
            return (_index + 1, _scenario._waves.Length + 1);
        }
        public State(GameScenario scenario)
        {
            _scenario = scenario;
            _index = 0;
            _wave = scenario._waves[0].Begin(); // запускаем первую волну
        }

        public bool Progress() // говорит о том, завершен сценарий или нет
        {
            float deltaTime = _wave.Progress(Time.deltaTime);
            while (deltaTime >= 0f)
            {
                if (++_index >= _scenario._waves.Length) 
                {
                    return false;
                }
                _wave = _scenario._waves[_index].Begin(); 
                deltaTime = _wave.Progress(deltaTime);
            }
            return true;
        }
    }

    public State Begin() => new State(this);
}
