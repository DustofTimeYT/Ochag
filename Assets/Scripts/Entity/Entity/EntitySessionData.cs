using UnityEngine;

namespace Entity
{
    public abstract class EntitySessionData
    {
        public string Name { get; set; }

        public int MaxHP { get; set; }
        public int HP { get; set; }

        public float MaxSpeed { get; set; }
        public float Speed { get; set; }

        public float JumpHeight { get; set; }
    }
}