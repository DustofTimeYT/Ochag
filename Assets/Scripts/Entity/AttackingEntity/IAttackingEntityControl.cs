/// <summary>
/// Контракт для взаимодействия контроллера с моделью атакующей сущности
/// (<see cref="AttackingEntityModel{TSessionData, TGameLogic}"/>).
/// </summary>
public interface IAttackingEntityControl
{
    public void AddArmor(int amount);
    public void AddStrength(int amount);
    public void AddAgility(int amount);
}
