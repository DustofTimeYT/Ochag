using UnityEngine;

namespace Entity
{
    /// <summary>
    /// Абстрактный класс, хранящий данные игровой сессии, присущие всем игровым и неигровым существам.
    /// </summary>
    public abstract class EntitySessionData
    {
        public string Name { get; set; }

        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }

        public float MaxWalkingSpeed { get; set; }
        public float MaxRunningSpeed { get; set; }
        public float MaxSprintingSpeed { get; set; }
        public float MaxJumpHeight { get; set; }
    }
}