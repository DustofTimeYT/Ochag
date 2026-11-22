using Character;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

public class SceneInstaller : MonoBehaviour, IInstaller
{
    public CharacterConfig characterConfig;
    public PlayerController playerController;

    public void InstallBindings(ContainerBuilder builder)
    {
        // 1. Создаём данные сессии (позже их можно будет брать из Config или сейва)
        var sessionData = new CharacterSessionData
        {
            MaxHP = characterConfig.MaxHP,
            CurrentHP = characterConfig.MaxHP,
            MaxStamina = characterConfig.MaxStamina,
            MaxWalkingSpeed = characterConfig.MaxWalkingSpeed,
            MaxRunningSpeed = characterConfig.MaxRunningSpeed,
            MaxSprintingSpeed = characterConfig.MaxSprintingSpeed,
            MaxJumpHeight = characterConfig.MaxJumpHeight,
        };

        CharacterGameLogic characterGameLogic = new();

        // 2. Регистрируем SessionData и EntityModel как синглтоны внутри этой сцены
        // Указываем, по каким интерфейсам (контрактам) Reflex должен отдавать EntityModel
        builder.RegisterValue(sessionData);
        builder.RegisterValue(characterGameLogic);
        builder.RegisterType(
           typeof(CharacterModel),
           new[] { typeof(IMoveControl), typeof(ITickable)},
           Lifetime.Singleton,
           Reflex.Enums.Resolution.Lazy
       );
        builder.RegisterValue(playerController);
    }
}
