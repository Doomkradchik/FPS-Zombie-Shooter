using UnityEngine;

public class DefaultGun : FpsWeapon
{
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private GameObject _holePrefab;
    [SerializeField]
    private Transform _gunNozzle;
    [SerializeField]
    private GameObject _muzzleFlashPrefab;
    public int Bullets { get; private set; }

    private static readonly int _idleState
            = Animator.StringToHash("Idle");
    private static readonly int _realoadState
            = Animator.StringToHash("Reload");

    public int MaxBullets => 20;
    protected override float MaxDistance => 100f;
    protected override float ShotForce => 150f;
    protected override float Damage => 100f; // to do

    protected override void OnEnable()
    {
        base.OnEnable();

        Bullets = MaxBullets;
    }
    public override void Unhide()
    {
        base.Unhide();

        AudioManager.Instance.PlaySound(AudioData.Kind.Perform);
    }

    public override void Hit()
    {
        if (_inited == false)
            throw new System.InvalidOperationException();

        if (CurrentState.shortNameHash != _idleState)
            return;

        if (Bullets < 1)
        {
            Reload();
            return;
        }

        Bullets--;
        _onDataChanged?.Invoke(this);
        _animator.SetTrigger(AnimationTrigger.Shoot);
        AudioManager.Instance.PlaySound(AudioData.Kind.Shoot);
        SpawnMuzzleFlash();

        if (_raycaster.TryThrowRay(MaxDistance, out RaycastHit hit, _layerMask))
            Instantiate(_holePrefab, hit.point, Quaternion.LookRotation(hit.normal));

        if (_raycaster.TryThrowRay(MaxDistance, out (Zombie, Vector3) data))
            OnZombieHit(data.Item1, data.Item2);

        if (_raycaster.TryThrowRay(MaxDistance, out BarrelFuel barrel))
            barrel.Explode();
    }

    public void Reload()
    {
        if (_inited == false)
            throw new System.InvalidOperationException();

        if (CurrentState.shortNameHash == _realoadState)
            return;

        if (Bullets + 1 > MaxBullets)
            return;

        _animator.SetTrigger(AnimationTrigger.Reload);
    }

    public void OnReloaded()
    {
        Bullets = MaxBullets;
        _onDataChanged?.Invoke(this);
    }


    private void SpawnMuzzleFlash()
    {
        var muzzleFlash = Instantiate(_muzzleFlashPrefab);
        muzzleFlash.transform.SetParent(_gunNozzle);
        muzzleFlash.transform.localPosition = Vector3.zero;
        muzzleFlash.transform.localRotation = Quaternion.identity;
    }
}

public class AnimationTrigger
{
    public static readonly string Shoot = "Shoot";
    public static readonly string Reload = "Reload";
    public static readonly string Perform = "Perform";
    public static readonly string Hit = "Hit";
    public static readonly string CanShoot = "CanShoot";
}

