using UnityEngine;
using Random = UnityEngine.Random;

public class Knife : FpsWeapon
{
    protected override float MaxDistance => 2f;
    protected override float ShotForce => 50f;
    protected override float Damage => 100f;

    public override void Hit()
    {
        _animator.SetFloat("Blend", GetRandom());
        _animator.SetTrigger(AnimationTrigger.Hit);
    }

    private float GetRandom()
    {
        return Random.value > 0.5f ? 1 : 0;
    }
}
