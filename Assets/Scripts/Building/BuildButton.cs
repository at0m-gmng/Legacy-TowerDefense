using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private GameTileContentType _type = default;

    private Action<GameTileContentType> _listenAction = delegate { };

    public void AddListener(Action<GameTileContentType> listenerAction) 
        => _listenAction = listenerAction;

    public void OnPointerDown(PointerEventData eventData) 
        => _listenAction?.Invoke(_type);
}
