/// <summary>
/// Контракт только для чтения состояния боевого режима сущности.
/// </summary>
public interface IBattleModeReadonly
{
    bool IsBattleMode { get; }
}
