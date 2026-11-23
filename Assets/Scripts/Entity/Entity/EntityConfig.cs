using UnityEngine;

/// <summary>
/// Абстрактная конфигурация для настройки существ в инспекторе
/// </summary>
public abstract class EntityConfig : ScriptableObject
{
    // _localizationKey будет использоваться для локализации. Если нужно имя то использовать имя файла *.name
    [Header("Системные настройки")]
    [SerializeField] private string _localizationKey;
    public string LocalizationKey => _localizationKey;

    [Header("Настройки передвижения")]
    [Range(0f, 50f)][SerializeField] private float _maxWalkingSpeed;
    public float MaxWalkingSpeed => _maxWalkingSpeed;

    [Range(0f, 50f)][SerializeField] private float _maxRunningSpeed;
    public float MaxRunningSpeed => _maxRunningSpeed;

    [Range(0f, 50f)][SerializeField] private float _maxSprintingSpeed;
    public float MaxSprintingSpeed => _maxSprintingSpeed;

    [Range(0f, 50f)][SerializeField] private float _maxJumpHeight;
    public float MaxJumpHeight => _maxJumpHeight;

    [Header("Общие характеристики")]
    [SerializeField] private int _maxHP;
    public int MaxHP => _maxHP;

    [SerializeField] private float _maxStamina;
    public float MaxStamina => _maxStamina;
}
