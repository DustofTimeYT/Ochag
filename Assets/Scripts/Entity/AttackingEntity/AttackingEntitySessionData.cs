using Entity;
using UnityEngine;

namespace AttackingEntity
{
    /// <summary>
    /// Абстрактный класс, хранящий данные игровой сессии, присущие атакующим существам.
    /// Наследует <see cref="EntitySessionData"/>.
    /// </summary>
    public abstract class AttackingEntitySessionData : EntitySessionData
    {
        public int Armor { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }

        public int MaxStamina { get; set; }
        public int Stamina { get; set; }
    }
}