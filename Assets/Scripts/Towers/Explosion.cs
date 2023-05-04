using System;
using UnityEngine;

public class Explosion: WarEntity
{
    [SerializeField, Range(0f, 1f)] private float _duration = 0.5f; // радиус взрыва
    [SerializeField] private AnimationCurve _scaleCurve;
    [SerializeField] private AnimationCurve _colorCurve;

    private float _age; // время жизни
    private float _scale;
    private MeshRenderer _meshRenderer;
    private static int _colorPropID = Shader.PropertyToID("_Color");
    private static MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Initialize(Vector3 position, float blastRange, float damage = 0f)
    {
        if (damage > 0f)
        {
            TargetPoint.FillBuffer(position, blastRange);
            for (int i = 0; i < TargetPoint.BufferedCount; i++)
            {
                TargetPoint.GetBuffered(i).Enemy.TakeDamage(damage);
            } 
        }

        transform.localPosition = position;
        _scale = 2f * blastRange;
    }

    public override bool GameUpdate()
    {
        _age += Time.deltaTime;
        if (_age >= _duration)
        {
            OriginFactory.Reclaim(this);
            return false;
        }

        if (_propertyBlock == null)
            _propertyBlock = new MaterialPropertyBlock();

        float t = _age / _duration;
        Color color = Color.yellow;
        color.a = _colorCurve.Evaluate(t);
        _propertyBlock.SetColor(_colorPropID, color);
        _meshRenderer.SetPropertyBlock(_propertyBlock);
        transform.localScale = Vector3.one* (_scale*_scaleCurve.Evaluate(t));

        return true;
    }
}