using System;

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
                sessionData.CurrentStamina = Math.Min(sessionData.MaxStamina, sessionData.CurrentStamina + 0.5f * deltaTime);
            }

            // Ограничиваем возможность спринта, пока не восстановиться часть стамины
            if (sessionData.CurrentStamina > sessionData.MaxStamina * 0.6f)
            {
                sessionData.IsSprintTimeout = false;
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
            // Уменьшаем стамину во время спринта
            if (sessionData.CurrentStamina > 0 && !sessionData.IsSprintTimeout)
            {
                sessionData.CurrentStamina = Math.Max(sessionData.CurrentStamina - 4f * deltaTime, 0);
                sessionData.IsSprinting = true;
                return true;
            }

            // После уменьшения стамины в ноль активируем блокировку спринта
            sessionData.IsSprintTimeout = true;
            return false;
        }
    }
}