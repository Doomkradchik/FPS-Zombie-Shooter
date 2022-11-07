﻿using System;
using UnityEngine;

public class DefaultGun : FpsWeapon
{
    [SerializeField]
    private GameObject _holePrefab;
    [SerializeField]
    private int _maxBullets;
    public int Bullets { get; private set; }
    public int MaxBullets => _maxBullets;

    private static readonly int _idleState
            = Animator.StringToHash("Idle");

    protected override float MaxDistance => 100f;
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

        var shootAudio = _audioDatas
            .Find(audio => audio.Kind == AudioDataClip.AudioKind.Shoot);

        if (shootAudio != null && shootAudio.Clip != null)
            _gunAudioSouce.PlayOneShot(shootAudio.Clip, _volume);

        if (_aimTargetFinder.ThrowRay(MaxDistance, out RaycastHit hit))
            OnTargeted(hit);

    }

    public void Reload()
    {
        if (_inited == false)
            throw new System.InvalidOperationException();

        if (Bullets + 1 > _maxBullets)
            return;

        Bullets = _maxBullets;
        _animator.SetTrigger(AnimationTrigger.Reload);
        _onDataChanged?.Invoke(this);
    }

    private void OnTargeted(RaycastHit hit)
    {
        Instantiate(_holePrefab, hit.point, Quaternion.LookRotation(hit.normal));
    }
}

public class AnimationTrigger
{
    public static readonly string Shoot = "Shoot";
    public static readonly string Reload = "Reload";
    public static readonly string Perform = "Perform";
    public static readonly string Hit = "Hit";
}


public class AimTargetFinder
{
    public AimTargetFinder(RectTransform aimTransform, Camera camera)
    {
        _aimTransform = aimTransform;
        _camera = camera;
    }
    private readonly RectTransform _aimTransform;
    private readonly Camera _camera;
    private Vector2 AimPosition =>
       new Vector2(Screen.width, Screen.height) * 0.5f + _aimTransform.anchoredPosition;

    public bool ThrowRay(float distance, out RaycastHit hit)
    {
        var ray = _camera.ScreenPointToRay(AimPosition);
        return Physics.Raycast(ray, out hit, distance);
    }
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
