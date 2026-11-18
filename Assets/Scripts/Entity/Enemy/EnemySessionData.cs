using AttackingEntity;

namespace Enemy
{
    /// <summary>
    /// Абстрактный класс, хранящий данные игровой сессии, присущие врагам.
    /// Наследует <see cref="AttackingEntitySessionData"/>.
    /// </summary>
    public abstract class EnemySessionData : AttackingEntitySessionData
    {
        /// <summary>
        /// Дистанция обнаружения игрока
        /// </summary>
        public int DetectionRange {  get; set; }

        /// <summary>
        /// Угол обнаружения игрока по горизонтали
        /// </summary>
        public int DetectionAngleXZ { get; set; }

        /// <summary>
        /// Угол обнаружения игрока по вертикали
        /// </summary>
        public int DetectionAngleXY { get; set; }
    }
}