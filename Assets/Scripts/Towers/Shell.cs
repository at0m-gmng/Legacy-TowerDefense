using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : WarEntity
{
    private Vector3 _launchPoint, _targetPoint, _launchVelocity;
    private float _age; // время с начала запуска снаряда
    private float _shellBlastRadius, _shellDamage;
    
    public void Initialize(Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity, float shellBlastRadius, float shellDamage)
    {
        _launchPoint = launchPoint;
        _targetPoint = targetPoint;
        _launchVelocity = launchVelocity;
        _shellBlastRadius = shellBlastRadius;
        _shellDamage = shellDamage;
    }

    public override bool GameUpdate()
    {
        _age += Time.deltaTime;
        Vector3 position = _launchPoint + _launchVelocity * _age;
        position.y -= 0.5f * 9.81f * _age * _age;

        if (position.y <= 0)
        {
            Game.SpawnExplosion().Initialize(_targetPoint, _shellBlastRadius, _shellDamage);
            OriginFactory.Reclaim(this);
            return false;
        }
        
        transform.localPosition = position;

        Vector3 direction = _launchVelocity;
        direction.y = 9.81f * _age;
        transform.localRotation = Quaternion.LookRotation(direction);
        
        Game.SpawnExplosion().Initialize(position, 0.1f);
            
        return true;
    }
}
