using UnityEngine;
using Character;

public class CharacterModel : AttackingEntityModel<CharacterSessionData, CharacterGameLogic>
{
    public CharacterModel()
    {
        _sessionData = new CharacterSessionData();
        _gameLogic = new CharacterGameLogic();
    }
}
