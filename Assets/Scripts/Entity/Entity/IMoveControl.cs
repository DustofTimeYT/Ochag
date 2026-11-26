/// <summary>
/// Контракт для управления параметрами передвижения сущности и проверки возможности ускорения.
/// </summary>
public interface IMoveControl
{
    /// <summary>
    /// Максимальная скорость передвижения шагом (м/с).
    /// </summary>
    public float MaxWalkingSpeed { get; }

    /// <summary>
    /// Максимальная скорость обычного бега (м/с).
    /// </summary>
    public float MaxRunningSpeed { get; }

    /// <summary>
    /// Максимальная скорость спринта (м/с).
    /// </summary>
    public float MaxSprintingSpeed { get; }

    /// <summary>
    /// Максимальная высота прыжка (м).
    /// </summary>
    public float MaxJumpHeight { get; }

    /// <summary>
    /// Пытается списать выносливость для поддержания спринта в текущем кадре.
    /// </summary>
    /// <param name="deltaTime">Время, прошедшее с прошлого кадра (<see cref="UnityEngine.Time.deltaTime"/>).</param>
    /// <returns><see langword="true"/>, если выносливости хватило и спринт разрешён; иначе — <see langword="false"/>.</returns>
    bool TrySprint(float deltaTime);
}
