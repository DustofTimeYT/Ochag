using UnityEngine;
using BossEnemy;

/// <summary>
/// Фасад для связи логики и данных сессии босса с остальной системой.
/// Наследует <see cref="EnemyModel{TSessionData, TGameLogic}"/>.
/// Реализует <see cref="IBossEnemyControl"/>.
/// </summary>
public class BossEnemyModel : EnemyModel<BossEnemySessionData, BossEnemyGameLogic>, IBossEnemyControl
{
    public BossEnemyModel(BossEnemySessionData sessionData, BossEnemyGameLogic gameLogic) : base(sessionData, gameLogic)
    {
    }
}
