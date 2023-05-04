using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

//отвечает за основной функционал и управляет игрой
public class Game : MonoBehaviour
{
    [SerializeField] private Vector2Int _boardSize; // задаём игровое поле
    [SerializeField] private GameBoard _board; // ссылка на поле
    [SerializeField] private Camera _mainCamera; // ссылка на главную камеру
    [SerializeField] private GameTileContentFactory _contentFactory; // ссылка на фабрику
    [SerializeField] private WarFactory _warFactory; // для передачи снаряда от мортиры к nonEnemies
    [SerializeField] private GameScenario _scenario;
    
    [SerializeField, Range(10, 100)] private int _startingPlayerHealth;
    private int _currentPlayerHealth;

    [SerializeField, Range(5f, 30f)] private float _prepareTime = 10f;
    private bool _isScenarioProcess;

    private GameBehaviourCollection _enemies = new GameBehaviourCollection();
    private GameBehaviourCollection _nonEnemies = new GameBehaviourCollection();
    private Ray TouchRay => _mainCamera.ScreenPointToRay(Input.mousePosition); // конвертируем позицию мыши в луч
    private TowerType _currentTowerType;
    private GameScenario.State _activeScenario;
    private bool _isPaused;

    private static Game _instance;

    //[SerializeField] private EnemyFactory _enemyFactory;
    //[SerializeField, Range(0.1f, 10f)] private float _spawnSpeed; // скорость появления врагов
    //private float _spawnProgress; 

    private void OnEnable()
    {
        _instance = this;
    }

    private void Start()
    {
        _board.Init(_boardSize, _contentFactory);
        //_activeScenario = _scenario.Begin();
        BeginNewGame();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            BeginNewGame();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentTowerType = TowerType.Laser;
        }
        else  if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _currentTowerType = TowerType.Mortar;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        } 
        else if (Input.GetMouseButtonDown(1))
        {
            HandleAlternativeTouch();
        }

        if(_isScenarioProcess)
        {
            if (_currentPlayerHealth <= 0)
            {
                Debug.Log("Defeated!");
                BeginNewGame();
            }

            if (!_activeScenario.Progress() && _enemies.IsEmpty)
            {
                Debug.Log("Victory!");
                BeginNewGame();
                _activeScenario.Progress();
            }
        }

        // Скрипт Game больше не выбирает, когда ему создавать врагов, отключаем логику
        //_spawnProgress += _spawnSpeed * Time.deltaTime;
        //while (_spawnProgress >= 1f)
        //{
        //    _spawnProgress -= 1f;
        //    SpawnEnemy();
        //}

        //_activeScenario.Progress();

        _enemies.GameUpdate();
        Physics.SyncTransforms(); // синхронизируем физику
        _board.GameUpdate();
        _nonEnemies.GameUpdate();
    }

    public static Shell SpawnShell()
    {
        Shell shell = _instance._warFactory.Shell;
        _instance._nonEnemies.Add(shell);
        return shell;
    }
    public static Explosion SpawnExplosion()
    {
        Explosion explosion = _instance._warFactory.Explosion;
        _instance._nonEnemies.Add(explosion);
        return explosion;
    }

    // модифициурем в статику
    // добавляем фабрику, тип врага
    // вызов идет через собственную статическую ссылку
    public static void SpawnEnemy(EnemyFactory enemyFactory, EnemyType enemyType)
    {
        GameTile spawnPoint = _instance._board.GetSpawnPoint(Random.Range(0, _instance._board.SpawnPointCount)); // берём случайную точку спавна
        Enemy enemy = enemyFactory.Get(enemyType); // создаём врага
        enemy.SpawnOn(spawnPoint); // далее передаём эту точку врагу как стартовую позицию
        _instance._enemies.Add(enemy);
    }

    private void HandleTouch() // берём тайл по лучу, если не нулл, присваиваем контент из фабрики
    {
        GameTile tile = _board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _board.ToggleTower(tile, _currentTowerType);
            }
            else
            {
                _board.ToggleWall(tile);
            }
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

    private void BeginNewGame()
    {
        _isScenarioProcess = false;
        if (_prepareRoutine != null)
        {
            StopCoroutine(_prepareRoutine);
        }
        _enemies.Clear();
        _nonEnemies.Clear();
        _board.Clear();
        _currentPlayerHealth = _startingPlayerHealth;
        _prepareRoutine = StartCoroutine(PrepareRoutine());
    }

    public static void EnemyReachedDestination()
    {
        _instance._currentPlayerHealth--;
    }

    private Coroutine _prepareRoutine;

    private IEnumerator PrepareRoutine()
    {
        yield return new WaitForSeconds(_prepareTime);
        _activeScenario = _scenario.Begin();
        _isScenarioProcess = true;
    }
}
