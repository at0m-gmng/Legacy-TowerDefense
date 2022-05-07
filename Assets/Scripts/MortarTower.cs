using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTower : Tower
{
    [SerializeField, Range(0.5f, 2f)] private float _shootPerSecond = 1f;
    [SerializeField, Range(0.5f, 3f)] private float _shellBlastRadius = 1f;
    [SerializeField, Range(1f, 100f)] private float _shellDamage = 1f;
    [SerializeField] private Transform _mortar;

    private float _launchSpeed;
    private float _launchProgress; // стрельба раз в какое-то время 
    
    public override TowerType Type => TowerType.Mortar;

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        float x = _targetingRange + 0.251f;
        float y = -_mortar.position.y;
        _launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
    }

    public override void GameUpdate()
    {
        _launchProgress += Time.deltaTime * _shootPerSecond;
        while (_launchProgress>1)
        {
            if (IsAcquireTarget(out TargetPoint target))
            {
                Launch(target);
                _launchProgress -= 1f;
            }
            else
            {
                _launchProgress = 0.999f;
            }
        }
        // Launch(new Vector3( 3f, 0f, 0f));
        // Launch(new Vector3( 0f, 0f, 1f));
        // Launch(new Vector3( 1f, 0f, 1f));
        // Launch(new Vector3( 3f, 0f, 1f));
    }

    private void Launch(TargetPoint target) //отвечает за запуск 
    {
        Vector3 launchPoint = _mortar.position;
        Vector3 targetPoint = target.Position;
        targetPoint.y = 0f;
        // new Vector3(launchPoint.x + 3f, 0f, launchPoint.z);
        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;
            
            
        float x = dir.magnitude;
        float y = -launchPoint.y;
        dir /= x; // нормализация вектора xz

        float g = 9.81f;
        float s = _launchSpeed;
        float s2 = s * s;

        float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        float tanAngle = (s2 + Mathf.Sqrt(r)) / (g * x);
        float cosAngle = Mathf.Cos(Mathf.Atan(tanAngle));
        float sinAngle = cosAngle * tanAngle;
        
        _mortar.localRotation = Quaternion.LookRotation(new Vector3(dir.x, tanAngle, dir.y)); // вращение башни 
        
        Game.SpawnShell().Initialize(launchPoint, targetPoint,
             new Vector3(s*cosAngle*dir.x, s*sinAngle, s*cosAngle*dir.y), _shellBlastRadius, _shellDamage); // вращение снаряда

        // отрисовка полёта снаряда
        // Vector3 prevPos = launchPoint, next;
        // for (int i = 1; i <= 10; i++)
        // {
        //     float t = i / 9.225f;
        //     float dx = s * cosAngle * t;
        //     float dy = s * sinAngle * t - 0.5f * g * t * t;
        //     next = launchPoint + new Vector3(dir.x * dx, dy, dir.y * dx);
        //     Debug.DrawLine(prevPos, next, Color.blue);
        //     prevPos = next;
        // }       
        //
        // Debug.DrawLine(launchPoint, targetPoint, Color.yellow);
        // Debug.DrawLine(new Vector3(launchPoint.x, 0.01f, launchPoint.z), 
        //     new Vector3(launchPoint.x + dir.x*x, 0.01f, launchPoint.z + dir.y * x), Color.white);
    }
 
}
