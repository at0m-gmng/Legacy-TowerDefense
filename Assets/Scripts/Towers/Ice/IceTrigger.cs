using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTrigger : MonoBehaviour
{
    public event Action<TargetPoint> OnEnter = delegate {  };
    public event Action<TargetPoint> OnExited = delegate {  };

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out TargetPoint targetPoint))
        {
            OnEnter(targetPoint);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out TargetPoint targetPoint))
        {
            OnExited(targetPoint);
        }
    }
}
