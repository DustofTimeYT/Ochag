using UnityEngine;

namespace Entity
{
    public abstract class EntitySessionData
    {
        public string Name { get; set; }

        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }

        public float MaxSpeed { get; set; }

        public float MaxJumpHeight { get; set; }
    }
}