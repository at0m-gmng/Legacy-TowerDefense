using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)] protected float _targetingRange = 1.5f;
    

    public abstract TowerType Type { get; }

    protected bool IsAcquireTarget(out TargetPoint target)
    {
        // Collider[] targets = Physics.OverlapSphere(transform.localPosition, _targetingRange, ENEMY_LAYER_MASK);
        if (TargetPoint.FillBuffer(transform.localPosition, _targetingRange))
        {
            target = TargetPoint.GetBuffered(0);
            return true;
        }

        target = null;
        return false;
    }

    protected bool IsTargetTracked(ref TargetPoint target)
    {
        if (target == null)
        {
            return false;
        }

        Vector3 myPosition = transform.localPosition;
        Vector3 targetPosition = target.Position;
        if (Vector3.Distance(myPosition, targetPosition) > _targetingRange + target.ColliderSize * target.Enemy.Scale || target.IsEnabled == false)
        {
            target = null;
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, _targetingRange);
    }
}
