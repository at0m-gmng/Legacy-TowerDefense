using System;

[Serializable] 
public class EnemyConfig
{
    public Enemy EnemyPref; // ссылка на префаб врага
    
    [FloatRangeSlider(0f, 1.5f)] 
    public FloatRange Scale = new FloatRange(0.5f); // Раздомизация размеров
    
    [FloatRangeSlider(-0.4f, 0.4f)] 
    public FloatRange PathOffset = new FloatRange(0f); // смещение пути
    
    [FloatRangeSlider(0.2f, 3f)] 
    public FloatRange SpeedRange = new FloatRange(1f); // диапозон скорости врагов
    
    [FloatRangeSlider(10f, 1000f)] 
    public FloatRange Health = new FloatRange(1f); // диапозон скорости врагов
}