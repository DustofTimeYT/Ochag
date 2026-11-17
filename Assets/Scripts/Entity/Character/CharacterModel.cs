using UnityEngine;
using Character;

/// <summary>
/// Фасад для связи логики и данных сессии персонажа с остальной системой.
/// Наследует <see cref="AttackingEntityModel{TSessionData, TGameLogic}"/>.
/// Реализует <see cref="ICharacterControl"/>.
/// </summary>
public class CharacterModel : AttackingEntityModel<CharacterSessionData, CharacterGameLogic>, ICharacterControl
{
    public CharacterModel(CharacterSessionData sessionData, CharacterGameLogic gameLogic) : base(sessionData, gameLogic)
    {
    }
}
