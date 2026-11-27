using System;
using UnityEngine;

namespace Entity
{
    /// <summary>
    /// Базовый абстрактный класс правил для существ.
    /// </summary>
    public abstract class EntityGameLogic
    {
        /// <summary>
        /// Восстанавливает выносливость, пока сущность не спринтует, и снимает блокировку усталости при достижении порога.
        /// </summary>
        /// <param name="sessionData">Игровые данные сущности.</param>
        /// <param name="deltaTime">Время, прошедшее с прошлого кадра (<see cref="UnityEngine.Time.deltaTime"/>).</param>
        public void UpdateStaminaRegen(EntitySessionData sessionData, float deltaTime)
        {
            // Восстанавливаем стамину, если энтини не спринтит
            if (sessionData.CurrentStamina < sessionData.MaxStamina && sessionData.IsSprinting == false)
            {
                sessionData.CurrentStamina = Math.Min(sessionData.MaxStamina, sessionData.CurrentStamina + sessionData.StaminaRegen * deltaTime);
            }

            // Ограничиваем возможность спринта, пока не восстановиться часть стамины
            if (sessionData.CurrentStamina > sessionData.MaxStamina * sessionData.StaminaRecoveryThreshold)
            {
                sessionData.IsSprintCooldown = false;
            }

            // Сбрасываем флаг в конце кадра: если в следующем кадре TrySprint не вызовется, пойдёт регенен
            sessionData.IsSprinting = false;
        }

        /// <summary>
        /// Пытается потратить выносливость на спринт в текущем кадре.
        /// При полном истощении активирует блокировку спринта до частичного восстановления.
        /// </summary>
        /// <param name="sessionData">Игровые данные сущности.</param>
        /// <param name="deltaTime">Время, прошедшее с прошлого кадра (<see cref="UnityEngine.Time.deltaTime"/>).</param>
        /// <returns><see langword="true"/>, если выносливости хватило; иначе — <see langword="false"/>.</returns>
        public bool TryConsumeSprintStamina(EntitySessionData sessionData, float deltaTime)
        {
            if (sessionData.IsSprintCooldown)
                return false;

            float cost = sessionData.SprintStaminaCost * deltaTime;
            if (TryConsumeStamina(sessionData, cost, requireFullAmount: false))
            {
                if (sessionData.IsBattleMode)
                    sessionData.IsSprinting = true;

                return true;
            }

            sessionData.IsSprintCooldown = true;
            return false;
        }

        /// <summary>
        /// Пытается потратить выносливость на уклонение (перекат).
        /// </summary>
        public bool TryStartDodge(EntitySessionData sessionData)
        {
            return TryConsumeStamina(sessionData, sessionData.DodgeStaminaCost, requireFullAmount: true);
        }

        public void SetDodging(EntitySessionData sessionData, bool isDodging)
        {
            sessionData.IsDodging = isDodging;
        }

        /// <summary>
        /// Единое правило расхода выносливости: вне боевого режима действия бесплатны.
        /// </summary>
        /// <param name="sessionData">Игровые данные сущности.</param>
        /// <param name="amount">Количество требуемой выносливости.</param>
        /// <param name="requireFullAmount">Требуется ли полный запас (для рывков/атак) или достаточно остатка > 0 (для потокового спринта).</param>
        private bool TryConsumeStamina(EntitySessionData sessionData, float amount, bool requireFullAmount)
        {
            if (!sessionData.IsBattleMode)
                return true;

            bool hasEnough = requireFullAmount
                ? sessionData.CurrentStamina >= amount
                : sessionData.CurrentStamina > 0f;

            if (!hasEnough)
                return false;

            sessionData.CurrentStamina = Math.Max(sessionData.CurrentStamina - amount, 0f);
            return true;
        }
    }
}