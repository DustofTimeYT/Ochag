/// <summary>
/// Контракт для взаимодействия контроллера с моделью врага
/// (<see cref="EnemyModel{TSessionData, TGameLogic}"/>).
/// </summary>
public interface IEnemyControl
{
    /// <summary>
    /// Дистанция обнаружения игрока
    /// </summary>
    public int DetectionRange { get; }

    /// <summary>
    /// Угол обнаружения игрока по горизонтали
    /// </summary>
    public int DetectionAngleXZ { get; }

    /// <summary>
    /// Угол обнаружения игрока по вертикали
    /// </summary>
    public int DetectionAngleXY { get; }
}