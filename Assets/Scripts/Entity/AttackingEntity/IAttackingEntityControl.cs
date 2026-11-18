
/// <summary>
/// Контракт для взаимодействия контроллера с моделью атакующей сущности
/// (<see cref="AttackingEntityModel{TSessionData, TGameLogic}"/>).
/// Наследует <see cref="IStatsReadonly"/>.
/// </summary>
public interface IAttackingEntityControl : IStatsReadonly
{
    public void AddArmor(int amount);
    public void AddStrength(int amount);
    public void AddAgility(int amount);
}
