using UnityEngine;
using Random = UnityEngine.Random;

public class Knife : FpsWeapon
{
    protected override float MaxDistance => 2f;

    public override void Hit()
    {
        _animator.SetFloat("Blend", GetRandom());
        _animator.SetTrigger(AnimationTrigger.Hit);

        //if (_aimTargetFinder.ThrowRay(MaxDistance, out RaycastHit hit) == false)
        //    return;
    }

    private float GetRandom()
    {
        return Random.value > 0.5f ? 1 : 0;
    }
}
