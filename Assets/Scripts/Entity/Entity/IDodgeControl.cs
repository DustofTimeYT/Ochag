/// <summary>
/// Контракт управления уклонением (перекатом) сущности.
/// </summary>
public interface IDodgeControl
{
    /// <summary>
    /// Дистанция рывка при уклонении (м).
    /// </summary>
    float DodgeDistance { get; }

    /// <summary>
    /// Длительность фазы уклонения (сек).
    /// </summary>
    float DodgeDuration { get; }

    /// <summary>
    /// Время перезарядки между уклонениями (сек).
    /// </summary>
    float DodgeCooldown { get; }

    /// <summary>
    /// Пытается инициировать уклонение (проверяет и списывает стамину в боевом режиме).
    /// </summary>
    bool TryStartDodge();

    /// <summary>
    /// Обновляет состояние активного уклонения в данных сессии (для неуязвимости / I-Frames).
    /// </summary>
    void SetDodging(bool isDodging);
}