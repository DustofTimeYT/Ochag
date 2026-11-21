/// <summary>
/// Контракт для реализации  <see cref="UnityEngine.MonoBehaviour">Update</see>-цикла в не <see cref="UnityEngine.MonoBehaviour"/> классах
/// </summary>
public interface ITickable
{
    public void Tick(float deltaTime);
}