using Entity;
using System;
using UnityEngine;

/// <summary>
/// Абстрактный фасад для связи логики и данных сессии существ с остальной системой.
/// Реализует <see cref="IMoveControl"/>, <see cref="IHPReadonly"/>.
/// </summary>
/// <typeparam name="TSessionData">Тип данных сессии (наследник <see cref="EntitySessionData"/>).</typeparam>
/// <typeparam name="TGameLogic">Тип игровой логики (наследник <see cref="EntityGameLogic"/>).</typeparam>
public abstract class EntityModel<TSessionData, TGameLogic> : IMoveControl, IBattleModeReadonly, IBattleModeControl, ITickable, IHPReadonly, IDodgeControl
    where TSessionData : EntitySessionData
    where TGameLogic : EntityGameLogic
{
    protected TSessionData _sessionData;
    protected TGameLogic _gameLogic;

    protected EntityModel(TSessionData sessionData, TGameLogic gameLogic)
    {
        _sessionData = sessionData ?? throw new ArgumentNullException(nameof(sessionData));
        _gameLogic = gameLogic ?? throw new ArgumentNullException(nameof(gameLogic));
    }

    public int MaxHP => _sessionData.MaxHP;
    public int CurrentHP => _sessionData.CurrentHP;

    public float MaxWalkingSpeed => _sessionData.MaxWalkingSpeed;
    public float MaxRunningSpeed => _sessionData.MaxRunningSpeed;
    public float MaxSprintingSpeed => _sessionData.MaxSprintingSpeed;

    public float MaxJumpHeight => _sessionData.MaxJumpHeight;

    public bool IsBattleMode => _sessionData.IsBattleMode;

    public float DodgeDistance => _sessionData.DodgeDistance;
    public float DodgeDuration => _sessionData.DodgeDuration;
    public float DodgeCooldown => _sessionData.DodgeCooldown;

    public bool TryStartDodge() => _gameLogic.TryStartDodge(_sessionData);
    public void SetDodging(bool isDodging) => _gameLogic.SetDodging(_sessionData, isDodging);

    public void SetBattleMode(bool isBattleMode)
    {
        _sessionData.IsBattleMode = isBattleMode;
    }

    public void Tick(float deltaTime)
    {
        _gameLogic.UpdateStaminaRegen(_sessionData, deltaTime);
    }

    public bool TrySprint(float deltaTime)
    {
        return _gameLogic.TryConsumeSprintStamina(_sessionData, deltaTime);
    }
}
