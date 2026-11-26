/// <summary>
/// Контракт управления боевым режимом сущности.
/// </summary>
public interface IBattleModeControl
{
    /// <summary>
    /// Устанавливает состояние боевого режима сущности.
    /// </summary>
    /// <param name="isBattleMode"><see langword="true"/>, если сущность в боевом режиме; иначе — <see langword="false"/>.</param>
    void SetBattleMode(bool isBattleMode);
}