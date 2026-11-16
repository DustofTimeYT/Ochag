using Entity;
using UnityEngine;

namespace AttackingEntity
{
    public abstract class AttackingEntitySessionData : EntitySessionData
    {
        public int Armor { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }

        public int MaxStamina { get; set; }
        public int Stamina { get; set; }
    }
}