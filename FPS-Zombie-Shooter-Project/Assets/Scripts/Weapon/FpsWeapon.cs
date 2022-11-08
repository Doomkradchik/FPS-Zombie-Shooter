using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public abstract class FpsWeapon : MonoBehaviour
{
    [SerializeField]
    protected Animator _animator;
    [SerializeField]
    protected AudioSource _gunAudioSouce;
    [SerializeField]
    protected List<AudioDataClip> _audioDatas;


    private readonly int _animatorLayerIndex = 0;
    protected AnimatorStateInfo CurrentState =>
        _animator.GetCurrentAnimatorStateInfo(_animatorLayerIndex);

    private List<Transform> _children = null;

    protected ClientRaycaster _raycaster;
    protected bool _inited = false;
    protected Action<FpsWeapon> _onDataChanged;

    protected abstract float MaxDistance { get; }
    protected abstract float ShotForce { get; }
    protected abstract float Damage { get; }

    protected virtual void OnEnable()
    {
        if(_inited == false)
        {
            enabled = false;
            return;
        }

        _children = GetComponentsInChildren<Transform>()
            .Skip(1)
            .ToList();
    }

    public void Init(ClientRaycaster aimTargetFinder, Action<FpsWeapon> onDataChanged)
    {
        _raycaster = aimTargetFinder;
        _onDataChanged = onDataChanged;
        _inited = true;
        enabled = true;
    }

    public virtual void Hide()
    {
        if (_inited == false)
            return;

        _children
            .ForEach(ch => ch.gameObject.SetActive(false));
    }

    public virtual void Unhide()
    {
        if (_inited == false)
            return;

        _children
            .ForEach(ch => ch.gameObject.SetActive(true));

        _animator.SetTrigger("Perform");
    }

    public void Hit()
    {
        OnHit();

        if (_raycaster.TryThrowRay(MaxDistance, out (Zombie, Vector3) data))
            OnZombieHit(data.Item1, data.Item2);
    }
    public virtual void OnHit() { }
    protected void OnZombieHit(Zombie zombie, Vector3 hitPoint)
    {
        zombie.OnHit(_raycaster.Ray.direction.normalized * ShotForce, hitPoint);
    }
}
