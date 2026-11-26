using UnityEngine;

namespace Entity
{
    /// <summary>
    /// Абстрактный класс, хранящий данные игровой сессии, присущие всем игровым и неигровым существам.
    /// </summary>
    public abstract class EntitySessionData
    {
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }

        public float MaxStamina { get; set; }
        public float CurrentStamina { get; set; }
        public float StaminaRegen { get; set; }
        public float SprintStaminaCost { get; set; }
        public float StaminaRecoveryThreshold { get; set; }

        public float MaxWalkingSpeed { get; set; }
        public float MaxRunningSpeed { get; set; }
        public float MaxSprintingSpeed { get; set; }

        public float MaxJumpHeight { get; set; }

        public bool IsSprinting { get; set; } = false;
        public bool IsSprintCooldown { get; set; } = false;
        public bool IsBattleMode { get; set; } = false;
    }
}