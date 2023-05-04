using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnSequence
{
    [SerializeField] private EnemyFactory _enemyFactory; // ����������� ��������� ������ ������� ��� ������ 
    [SerializeField] private EnemyType _enemyType;
    [SerializeField, Range(1, 100)] private int _amount = 1; // ���-��
    [SerializeField, Range(0.1f, 10f)] private float _cooldown = 1f;

    // ����������� � ������� ������
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
            while (_cooldown >= _sequence._cooldown) // ����� �� �������� ������ �����
            {
                _cooldown -= _sequence._cooldown;
                if(_countEnemy >= _sequence._amount) // ���� ������� �������� ����������� ������� ��������� + �����, ��� ����� ������ ����� ��������� sequence
                {
                    return _countEnemy;
                }
                _countEnemy++;
                Game.SpawnEnemy(_sequence._enemyFactory, _sequence._enemyType);

            }

            return -1f; // ���� ������ �������� ��� �����, �� ���������� ������������� ��������
        }
    }
    public State Begin() => new State(this); // ������� State
}
