using UnityEngine;
using Character;

public class CharacterModel : AttackingEntityModel<CharacterSessionData, CharacterGameLogic>, ICharacterControl
{
    public CharacterModel(CharacterSessionData sessionData, CharacterGameLogic gameLogic) : base(sessionData, gameLogic)
    {
    }
}
