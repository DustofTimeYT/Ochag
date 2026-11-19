using UnityEngine;
using CommonEnemy;

/// <summary>
/// Фасад для связи логики и данных сессии обычного врага с остальной системой.
/// Наследует <see cref="EnemyModel{TSessionData, TGameLogic}"/>.
/// Реализует <see cref="ICommonEnemyControl"/>.
/// </summary>
public class CommonEnemyModel : EnemyModel<CommonEnemySessionData, CommonEnemyGameLogic>, ICommonEnemyControl
{
    public CommonEnemyModel(CommonEnemySessionData sessionData, CommonEnemyGameLogic gameLogic) : base(sessionData, gameLogic)
    {
    }
}
