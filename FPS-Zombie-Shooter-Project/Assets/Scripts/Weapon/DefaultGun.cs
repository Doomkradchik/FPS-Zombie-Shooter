using System;
using System.Collections;
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
    [SerializeField]
    private int _maxBullets;
    public int Bullets { get; private set; }
    public int MaxBullets => _maxBullets;

    private static readonly int _idleState
            = Animator.StringToHash("Idle");
    private static readonly int _realoadState
            = Animator.StringToHash("Reload");

    protected override float MaxDistance => 100f;
    protected override float ShotForce => 150f;
    protected override float Damage => 200f; // to do

    private readonly float _volume = 0.5f;

    protected override void OnEnable()
    {
        base.OnEnable();

        Bullets = _maxBullets;
    }
    public override void Unhide()
    {
        base.Unhide();
        var performing = _audioDatas
            .Find(audio => audio.Kind == AudioDataClip.AudioKind.Perform);

        if (performing != null && performing.Clip != null)
            _gunAudioSouce.PlayOneShot(performing.Clip, _volume);    
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
        SpawnMuzzleFlash();

        var shootAudio = _audioDatas
            .Find(audio => audio.Kind == AudioDataClip.AudioKind.Shoot);

        if (shootAudio != null && shootAudio.Clip != null)
            _gunAudioSouce.PlayOneShot(shootAudio.Clip, _volume);

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

        if (Bullets + 1 > _maxBullets)
            return;

        _animator.SetTrigger(AnimationTrigger.Reload);
    }

    public void OnReloaded()
    {
        Bullets = _maxBullets;
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


[Serializable]
public sealed class AudioDataClip
{
    [SerializeField]
    private AudioKind _kind;
    [SerializeField]
    private AudioClip _clip;
    public AudioKind Kind => _kind;
    public AudioClip Clip => _clip;

    public enum AudioKind
    {
        Shoot, 
        Perform
    }
}
