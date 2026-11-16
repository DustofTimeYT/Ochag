
public interface IEntityControl: IHPReadonly
{
    public float MaxSpeed { get; }

    public float MaxJumpHeight { get; }
    /*
    public void TakeDamage(int amount); // надо решить что будет передаваться только атака/урон и список эффектов/ урон и атака

    public void Heal(int amount); // либо эфектом либо числом
    */
}
