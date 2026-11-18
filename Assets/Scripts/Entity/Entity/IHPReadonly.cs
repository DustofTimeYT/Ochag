
/// <summary>
/// Контракт для чтения данных о здоровье сущности.
/// Реализуется моделью <see cref="EntityModel{TSessionData, TGameLogic}"/>.
/// </summary>
public interface IHPReadonly
{
    public int MaxHP { get; }

    public int CurrentHP { get; }
}
