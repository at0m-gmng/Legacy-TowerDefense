using UnityEngine;

public class Enemy : GameBehaviour
{
    [SerializeField] private Transform _model; // делаем так, чтобы враг двигался по 1/4 окружности за счёт своей модели
    [SerializeField] private EnemyView _enemyView;
    
    private GameTile _tileFrom, _tileTo;
    private Vector3 _positionFrom, _positionTo;
    private float _progress, _progressFactor;
    private Direction _direction;
    private DirectionChange _directionChange;
    private float _directionAngleFrom, _directionAngleTo;
    private float _pathOffset, _speedRange, _originalSpeed;

    public float Scale { get; private set; }
    public float Health { get; private set; }
    public EnemyFactory OriginFactory { get; set; }
    
    public void Init(float scale, float pathOffset, float speedRange, float health)
    {
        _originalSpeed = speedRange;
        _model.localScale = new Vector3(scale, scale, scale);
        _pathOffset = pathOffset;
        _speedRange = speedRange;
        Scale = scale;
        Health = health;
        _enemyView.Init(this);
    }

    public void SpawnOn(GameTile tile)
    {
        transform.localPosition = tile.transform.localPosition;
        _tileFrom = tile; // начало пути - точка спавна
        _tileTo = tile.NextTileOnPath;
        _progress = 0f;
        PrepareIntro();
    }

    public override bool GameUpdate() // жив ли враг, пока всегда истина
    {
        if (_enemyView.IsInited == false)
        {
            return true;
        }
        
        if (Health <= 0f)
        {
            DisableView();
            _enemyView.Die();
            return false;
        }
        
        _progress += Time.deltaTime * _progressFactor; // скорость 1 tile в сек
        while (_progress >= 1f)
        {
            if (_tileTo == null)
            {
                Game.EnemyReachedDestination();
                Recycle();
                return false;
            }
            _progress = (_progress - 1f) / _progressFactor;
            PrepareNextState();
            _progress *= _progressFactor;
        }

        if (_directionChange == DirectionChange.None)
        {
            transform.localPosition = Vector3.LerpUnclamped(_positionFrom, _positionTo, _progress);
        }
        else
        {
            float angle = Mathf.LerpUnclamped(_directionAngleFrom, _directionAngleTo, _progress);
            transform.localRotation = Quaternion.Euler(0f,angle,0f);
        }
        return true;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
    }

    public void SetSpeed(float factor)
    {
        _speedRange = _originalSpeed * factor;
        HandleDirection();
        _enemyView.SetSpeedFactor(factor);
    }

    public override void Recycle()
    {
        OriginFactory.Reclaim(this);
    }

    // поворот зависит от смены направления
    private void PrepareForward()
    {
        transform.localRotation = _direction.GetRotation();
        _directionAngleTo = _direction.GetAngle();
        _model.localPosition = new Vector3(_pathOffset, 0f);
            //Vector3.zero; // стандартное положение модели
        _progressFactor = _speedRange;
    }
    private void PrepareTurnRight()
    {
        _directionAngleTo = _directionAngleFrom + 90f;
        _model.localPosition = new Vector3(_pathOffset - 0.5f, 0f);
        transform.localPosition = _positionFrom + _direction.GetHalfVector();
        _progressFactor = _speedRange / (Mathf.PI * 0.5f * (0.5f - _pathOffset));
    }
    private void PrepareTurnLeft()
    {
        _directionAngleTo = _directionAngleFrom - 90f;
        _model.localPosition = new Vector3(_pathOffset + 0.5f, 0f);
        transform.localPosition = _positionFrom + _direction.GetHalfVector();
        _progressFactor = _speedRange / (Mathf.PI * 0.5f * (0.5f + _pathOffset));
    }
    private void PrepareTurnAround()
    {
        _directionAngleTo = _directionAngleFrom + (_pathOffset < 0f ? 180f : -180f);
        _model.localPosition = new Vector3(_pathOffset, 0f);
        transform.localPosition = _positionFrom;
        _progressFactor = _speedRange /(Mathf.PI*Mathf.Max(Mathf.Abs(_pathOffset), 0.2f));
    }

    private void PrepareNextState() // изменение состояния
    {            
        _tileFrom = _tileTo;
        _tileTo = _tileTo.NextTileOnPath; // устанавливаем следующую позицию
        _positionFrom = _positionTo;
        if (_tileTo == null)
        {
            PrepareOutro();
        }
        _positionTo = _tileFrom.ExitPoint; // используем точки выхода для перехода на след клетку
        _directionChange = _direction.GetDirectionChangeTo(_tileFrom.PathDirection);
        _direction = _tileFrom.PathDirection;
        _directionAngleFrom = _directionAngleTo;
        
        HandleDirection();
    }

    private void HandleDirection()
    {
        switch (_directionChange)
        {
            case DirectionChange.None: PrepareForward();
                break;
            case DirectionChange.TurnRight: PrepareTurnRight();
                break;
            case DirectionChange.TurnLeft: PrepareTurnLeft();
                break;
            default: PrepareTurnAround();
                break;
        }
    }
    
    private void PrepareIntro() //исходное состояние как вводное
    {
        //враг перемещается от центра к краю своего начального тайла, и углы одинаковые
        _positionFrom = _tileFrom.transform.localPosition;
        _positionTo = _tileFrom.ExitPoint;
        _direction = _tileFrom.PathDirection;
        _directionChange = DirectionChange.None;
        _directionAngleFrom = _directionAngleTo = _direction.GetAngle();
        _model.localPosition = new Vector3(_pathOffset, 0f);
        transform.localRotation = _direction.GetRotation();
        _progressFactor = 2f * _speedRange;
    }

    private void PrepareOutro() // конечное состояние
    {
        _positionTo = _tileFrom.transform.localPosition;
        _directionChange = DirectionChange.None;
        _directionAngleTo = _direction.GetAngle();
        _model.localPosition = new Vector3(_pathOffset, 0f);
        transform.localRotation = _direction.GetRotation();
        _progressFactor = 2f * _speedRange;
    }

    private void DisableView()
    {
        _enemyView.GetComponent<Collider>().enabled = false;
        _enemyView.GetComponent<TargetPoint>().IsEnabled = false;
    }
    
}