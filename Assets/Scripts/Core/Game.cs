using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Core.UI;
using GameResult;
using Loading;

//отвечает за основной функционал и управляет игрой
public class Game : MonoBehaviour, ICleanUp
{    
    public IEnumerable<GameObjectFactory> Factories => new GameObjectFactory[]{_contentFactory, 
        _warFactory, _enemyFactory};

    public string SceneName => Constants.Scenes.QUICK_GAME;
    
    private int PlayerHealth
    {
        get => _playerHealth;
        set
        {
            _playerHealth = Mathf.Max(0, value);
            _defenderHud.UpdatePlayerHealth(_playerHealth, _startingPlayerHealth);
        }
    }
    
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

    [SerializeField]
    private TilesBuilder _tilesBuilder = default;
    [SerializeField] 
    private DefenderHud _defenderHud;
    [SerializeField] 
    private GameResultWindow _gameResultWindow;
    [SerializeField] 
    private EnemyFactory _enemyFactory;
    [SerializeField]
    private Camera _camera;
    [SerializeField] 
    private PrepareGamePanel _prepareGamePanel;
    
    private CancellationTokenSource _prepareCancellation;
    private GameBehaviourCollection _enemies = new GameBehaviourCollection();
    private GameBehaviourCollection _nonEnemies = new GameBehaviourCollection();
    private Ray TouchRay => _mainCamera.ScreenPointToRay(Input.mousePosition); // конвертируем позицию мыши в луч
    private GameTileContentType _currentTowerType;
    private GameScenario.State _activeScenario;
    private bool _isPaused;
    private int _playerHealth;
    
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
        _defenderHud.PauseClicked += OnPauseClicked;
        _defenderHud.QuitGame += GoToMainMenu;
        _board.Init(_boardSize, _contentFactory);
        _tilesBuilder.Initialize(_contentFactory, _camera, _board);
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

        if(_isScenarioProcess)
        {
            var waves = _activeScenario.GetWaves();
            if (waves.currentWave <= waves.wavesCount)
            {
                _defenderHud.UpdateScenarioWaves(waves.currentWave, waves.wavesCount);
            }
            if (PlayerHealth <= 0)
            {
                _isScenarioProcess = false;
                _gameResultWindow.Show(GameResultType.Defeat, BeginNewGame, GoToMainMenu);
            }
            if (_activeScenario.Progress() == false && _enemies.IsEmpty)
            {
                _isScenarioProcess = false;
                _gameResultWindow.Show(GameResultType.Victory, BeginNewGame, GoToMainMenu);
            }
        }

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

    public static void EnemyReachedDestination()
    {
        _instance.PlayerHealth--;
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

    public void Cleanup()
    {
        _tilesBuilder.Disabled();
        _isScenarioProcess = false;
        _prepareCancellation?.Cancel();
        _prepareCancellation?.Dispose();
        _enemies.Clear();
        _nonEnemies.Clear();
        _board.Clear();
    }
    
    private async void BeginNewGame()
    {
        Cleanup();
        _tilesBuilder.Enabled();
        PlayerHealth = _startingPlayerHealth;

        try
        {
            _prepareCancellation?.Dispose();
            _prepareCancellation = new CancellationTokenSource();
            if (await _prepareGamePanel.Prepare(_prepareTime, _prepareCancellation.Token))
            {
                _activeScenario = _scenario.Begin();
                _isScenarioProcess = true;
            }
        }
        catch (TaskCanceledException e) {}
    }

    private void OnPauseClicked(bool isPause)
    {
        Time.timeScale = isPause ? 0f : 1f;
    }

    private void GoToMainMenu()
    {
        var operations = new Queue<ILoadingOperation>();
        operations.Enqueue(new ClearGameOperation(this));
        LoadingScreen.Instance.Load(operations);
    }

    private void OnDestroy()
    {
        _defenderHud.PauseClicked -= OnPauseClicked;
        _defenderHud.QuitGame -= GoToMainMenu;
    }
}
