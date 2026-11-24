using UnityEngine;

/// <summary>
/// Справочник параметров аниматора.
/// </summary>
public static class AnimatorParams
{
    // Движение и стойки
    public static readonly int BattleMode = Animator.StringToHash("BTL");
    public static readonly int Horizontal = Animator.StringToHash("H");
    public static readonly int Vertical = Animator.StringToHash("V");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    public static readonly int Sprint = Animator.StringToHash("Sprint");
    public static readonly int Dodge = Animator.StringToHash("Dodge");

    // Защита и получение урона
    public static readonly int Block = Animator.StringToHash("Block");
    public static readonly int BlockReact = Animator.StringToHash("Block_React");
    public static readonly int Parry = Animator.StringToHash("Parry");
    public static readonly int HitLeft = Animator.StringToHash("Hit L");
    public static readonly int HitRight = Animator.StringToHash("Hit R");

    // Комбо-атаки (удобно обращаться по индексу удара в комбо)
    public static readonly int[] LightAttacks =
    {
        Animator.StringToHash("ATK L 1"),
        Animator.StringToHash("ATK L 2"),
        Animator.StringToHash("ATK L 3"),
        Animator.StringToHash("ATK L 4"),
        Animator.StringToHash("ATK L 5")
    };

    public static readonly int[] HeavyAttacks =
    {
        Animator.StringToHash("ATK H 1"),
        Animator.StringToHash("ATK H 2"),
        Animator.StringToHash("ATK H 3")
    };

    public static readonly int JumpAttack = Animator.StringToHash("Jump ATK");
    public static readonly int DodgeAttack = Animator.StringToHash("Dodge ATK");
}