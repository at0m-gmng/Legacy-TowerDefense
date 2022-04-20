using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//отвечает за основной функционал и управляет игрой
public class Game : MonoBehaviour
{
    [SerializeField] private Vector2Int _boardSize; // задаём игровое поле
    [SerializeField] private GameBoard _board; // ссылка на поле
    [SerializeField] private Camera _mainCamera; // ссылка на главную камеру
    [SerializeField] private GameTileContentFactory _contentFactory; // ссылка на фабрику
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
            _board.ToggleDestination(tile);
        }
    }
}
