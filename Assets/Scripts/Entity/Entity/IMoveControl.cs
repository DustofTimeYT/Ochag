
/// <summary>
/// Контракт для предоставления возможности управления передвижением контроллеру
/// </summary>
public interface IMoveControl
{
    public float MaxWalkingSpeed { get; }
    public float MaxRunningSpeed { get; }
    public float MaxSprintingSpeed { get; }
    public float MaxJumpHeight { get; }
    public bool TryJump();
}
