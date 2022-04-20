using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Vector2Int _boardSize;
    [SerializeField] private GameBoard _board;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameTileContentFactory _contentFactory;
    private Ray TouchRay => _mainCamera.ScreenPointToRay(Input.mousePosition);
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
    }

    private void HandleTouch()
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
            _board.ToggleDestination(tile);
        }
    }
}
