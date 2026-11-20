using Enemy;

/// <summary>
/// Абстрактный фасад для связи логики и данных сессии врагов с остальной системой.
/// Наследует <see cref="AttackingEntityModel{TSessionData, TGameLogic}"/>.
/// Реализует <see cref="IEnemyControl"/>.
/// </summary>
/// <typeparam name="TSessionData">Тип данных сессии (наследник <see cref="EnemySessionData"/>).</typeparam>
/// <typeparam name="TGameLogic">Тип игровой логики (наследник <see cref="EnemyGameLogic"/>).</typeparam>
public abstract class EnemyModel<TSessionData, TGameLogic> : AttackingEntityModel<TSessionData, TGameLogic>, IEnemyControl
    where TSessionData : EnemySessionData
    where TGameLogic : EnemyGameLogic
{
    public EnemyModel(TSessionData sessionData, TGameLogic gameLogic) : base(sessionData, gameLogic)
    {
    }

    /// <summary>
    /// Дистанция обнаружения игрока
    /// </summary>
    public int DetectionRange => _sessionData.DetectionRange;

    /// <summary>
    /// Угол обнаружения игрока по горизонтали
    /// </summary>
    public int DetectionAngleXZ => _sessionData.DetectionAngleXZ;

    /// <summary>
    /// Угол обнаружения игрока по вертикали
    /// </summary>
    public int DetectionAngleXY => _sessionData.DetectionAngleXY;
}
