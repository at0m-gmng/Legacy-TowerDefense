using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceObstacle : GameTileContent
{
    [SerializeField] private IceTrigger _iceTrigger = default;

    private static readonly Dictionary<TargetPoint, Guid> _globalTargetStorage = new Dictionary<TargetPoint, Guid>();
    private readonly Dictionary<TargetPoint, Guid> _internalTargetStorage = new Dictionary<TargetPoint, Guid>();

    private void Awake()
    {
        _iceTrigger.OnEnter += OnTriggerEnterAction;
        _iceTrigger.OnExited += OnTriggerExitAction;
    }

    private void OnTriggerEnterAction(TargetPoint targetPoint)
    {
        var guild = Guid.NewGuid();
        _globalTargetStorage[targetPoint] = guild;
        _internalTargetStorage[targetPoint] = guild;
        targetPoint.Enemy.SetSpeed(0.1f);
    }

    private void OnTriggerExitAction(TargetPoint targetPoint)
    {
        Guid guildGlobal = default;
        Guid guildInternal = default;
        if (_globalTargetStorage.ContainsKey(targetPoint))
        {
            guildGlobal = _globalTargetStorage[targetPoint];
        }
        if (_internalTargetStorage.ContainsKey(targetPoint))
        {
            guildInternal =_internalTargetStorage[targetPoint];
        }
        
        if (guildGlobal != guildInternal)
        {
            return;
        }
        targetPoint.Enemy.SetSpeed(1f);
        _globalTargetStorage.Remove(targetPoint);
    }
    
    private void OnDestroy()
    {
        _iceTrigger.OnEnter -= OnTriggerEnterAction;
        _iceTrigger.OnExited -= OnTriggerExitAction;
    }
}
