using Entity;
using System;
using UnityEngine;

public abstract class EntityModel<TSessionData, TGameLogic> : IEntityControl 
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

    public float MaxSpeed => _sessionData.MaxSpeed;    

    public float MaxJumpHeight => _sessionData.MaxJumpHeight;

}
