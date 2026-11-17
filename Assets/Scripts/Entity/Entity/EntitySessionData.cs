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

        public float MaxSpeed { get; set; }

        public float MaxJumpHeight { get; set; }
    }
}