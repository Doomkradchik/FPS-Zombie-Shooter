using UnityEngine;
using Random = UnityEngine.Random;

public class Knife : FpsWeapon
{
    protected override float MaxDistance => 1f;
    protected override float ShotForce => 200f;
    protected override float Damage => 200f;

    public override void Hit()
    {
        _animator.SetFloat("Blend", GetRandom());
        _animator.SetTrigger(AnimationTrigger.Hit);
        if (_raycaster.TryThrowRay(MaxDistance, out (Zombie, Vector3) data))
            OnZombieHit(data.Item1, data.Item2);
    }

    private float GetRandom()
    {
        return Random.value > 0.5f ? 1 : 0;
    }
}
