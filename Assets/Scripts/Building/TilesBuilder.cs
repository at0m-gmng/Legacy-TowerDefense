using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Windows.WebCam;

public class TilesBuilder : MonoBehaviour
{
    [SerializeField]
    private List<BuildButton> _buttons = new List<BuildButton>();

    private GameTileContentFactory _contentFactory = default;
    private Camera _camera = default;
    private GameBoard _gameBoard = default;

    private bool _isEnabled = default;
    private GameTileContent _pendingTile = null;
    private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);
    private bool _isDestroyAllowed;

    private void Awake()
    {
        _buttons.ForEach(x => x.AddListener(OnBuildingSelected));
    }

    public void Initialize(GameTileContentFactory contentFactory, Camera camera, GameBoard gameBoard)
    {
        _contentFactory = contentFactory;
        _camera = camera;
        _gameBoard = gameBoard;
    }

    private void Update()
    {
        if(_isEnabled == false || _pendingTile == null)
            return;

        var plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(TouchRay, out var position))
        {
            _pendingTile.transform.position = TouchRay.GetPoint(position);
        }
            
        if (IsPointerUp())
        {
            var tile = _gameBoard.GetTile(TouchRay);
            if (tile != null && tile.Content.Type == GameTileContentType.Empty)
                _gameBoard.Build(tile, _pendingTile.Type);
                
            Destroy(_pendingTile.gameObject);
            _pendingTile = null;
        }
    }

    private bool IsPointerUp()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        return Input.GetMouseButtonUp(0);
#else
        return Input.touches.Length = 0;
#endif
    }

    public void Enabled() => _isEnabled = true;
    
    public void Disabled() => _isEnabled = false;

    private void OnBuildingSelected(GameTileContentType type)
    {
        //TODO check money
        
        _pendingTile = _contentFactory.Get(type);
    }
    
}
