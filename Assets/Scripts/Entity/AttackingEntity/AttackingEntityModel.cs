using AttackingEntity;

/// <summary>
/// Абстрактный фасад для связи логики и данных сессии атакующих существ с остальной системой.
/// Наследует <see cref="EntityModel{TSessionData, TGameLogic}"/>.
/// Реализует <see cref="IAttackingEntityControl"/>.
/// </summary>
/// <typeparam name="TSessionData">Тип данных сессии (наследник <see cref="AttackingEntitySessionData"/>).</typeparam>
/// <typeparam name="TGameLogic">Тип игровой логики (наследник <see cref="AttackingEntityGameLogic"/>).</typeparam>
public abstract class AttackingEntityModel<TSessionData, TGameLogic> : EntityModel<TSessionData, TGameLogic>, IAttackingEntityControl
    where TSessionData : AttackingEntitySessionData
    where TGameLogic : AttackingEntityGameLogic
{
    protected AttackingEntityModel(TSessionData sessionData, TGameLogic gameLogic) : base(sessionData, gameLogic)
    {
        _gameLogic.CalculateMaxStamina(sessionData, 10);
    }

    public int Armor => _sessionData.Armor;

    public int Strength => _sessionData.Strength;

    public int Agility => _sessionData.Agility;

    public int MaxStamina => _sessionData.MaxStamina;

    public int Stamina => _sessionData.Stamina;

    public void AddAgility(int amount)
    {
        _gameLogic.AddAgility(_sessionData, amount);
    }

    public void AddArmor(int amount)
    {
        _gameLogic.AddArmor(_sessionData, amount);
    }

    public void AddStrength(int amount)
    {
        _gameLogic.AddStrength(_sessionData, amount);
    }
}
