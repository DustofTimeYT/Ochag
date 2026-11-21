using UnityEngine;

/// <summary>
/// Абстрактная конфигурация для настройки существ в инспекторе
/// </summary>
public abstract class EntityConfig : ScriptableObject
{
    // _localizationKey будет использоваться для локализации. Если нужно имя то использовать имя файла *.name
    [SerializeField] private string _localizationKey;
    public string LocalizationKey => _localizationKey;

    [SerializeField] private int _maxHP;
    public int MaxHP => _maxHP;

    [SerializeField] private float _maxWalkingSpeed;
    public float MaxWalkingSpeed => _maxWalkingSpeed;

    [SerializeField] private float _maxRunningSpeed;
    public float MaxRunningSpeed => _maxRunningSpeed;

    [SerializeField] private float _maxSprintingSpeed;
    public float MaxSprintingSpeed => _maxSprintingSpeed;

    [SerializeField] private float _maxJumpHeight;
    public float MaxJumpHeight => _maxJumpHeight;
}
