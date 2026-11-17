using Entity;
using UnityEngine;

namespace AttackingEntity
{
    public abstract class AttackingEntityGameLogic : EntityGameLogic
    {
        public void AddAgility(AttackingEntitySessionData data, int amount)
        {
            data.Agility += amount;
        }

        public void AddArmor(AttackingEntitySessionData data, int amount)
        {
            data.Armor += amount;
        }

        public void AddStrength(AttackingEntitySessionData data, int amount)
        {
            data.Strength += amount;
        }

        public void CalculateMaxStamina(AttackingEntitySessionData data, int configMaxStamina)
        {
            data.MaxStamina = configMaxStamina + data.Agility * 2;
        }
    }
}