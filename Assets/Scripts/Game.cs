using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//отвечает за основной функционал и управляет игрой
public class Game : MonoBehaviour
{
    [SerializeField] private Vector2Int _boardSize; // задаём игровое поле
    [SerializeField] private GameBoard _board; // ссылка на поле
    [SerializeField] private Camera _mainCamera; // ссылка на главную камеру
    [SerializeField] private GameTileContentFactory _contentFactory; // ссылка на фабрику
    [SerializeField] private EnemyFactory _enemyFactory;
    [SerializeField, Range(0.1f, 10f)] private float _spawnSpeed; // скорость появления врагов

    private EnemyCollection _enemies = new EnemyCollection();
    private float _spawnProgress; 
    private Ray TouchRay => _mainCamera.ScreenPointToRay(Input.mousePosition); // конвертируем позицию мыши в луч
    private void Start()
    {
        _board.Init(_boardSize, _contentFactory);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        } 
        else if (Input.GetMouseButtonDown(1))
        {
            HandleAlternativeTouch();
        }

        _spawnProgress += _spawnSpeed * Time.deltaTime;
        while (_spawnProgress >= 1f)
        {
            _spawnProgress -= 1f;
            SpawnEnemy();
        }
        _enemies.GameUpdate();
    }

    private void SpawnEnemy()
    {
        GameTile spawnPoint = _board.GetSpawnPoint(Random.Range(0, _board.SpawnPointCount)); // берём случайную точку спавна
        Enemy enemy = _enemyFactory.Get(); // создаём врага
        enemy.SpawnOn(spawnPoint); // далее передаём эту точку врагу как стартовую позицию
        _enemies.Add(enemy);
    }

    private void HandleTouch() // берём тайл по лучу, если не нулл, присваиваем контент из фабрики
    {
        GameTile tile = _board.GetTile(TouchRay);
        if (tile != null)
        {
            _board.ToggleWall(tile);
        }
    }

    private void HandleAlternativeTouch()
    {
        GameTile tile = _board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _board.ToggleDestination(tile);   
            }
            else
            {
                _board.ToggleSpawnPoint(tile);
            }
        }
    }
}
