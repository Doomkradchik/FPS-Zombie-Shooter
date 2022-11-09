using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private HitBox[] _hitBoxes;

    private Rigidbody[] _bodyParts;
    private NavMeshAgent _controller;
    private Animator _animator;

    private ZombieStateMachine _stateMachine;

    public float Health { get; private set; } = 100f;

    private const float TOP_RATIO = 1f;
    private const float MIDDLE_RATIO = 0.5f;
    private const float DOWN_RATIO = 0.25f;

    private void Awake()
    {
        _controller = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _bodyParts = GetComponentsInChildren<Rigidbody>();
        _stateMachine = new ZombieStateMachine();
        _stateMachine.ChangeState(new WalkingState(DisableRagdoll, _controller, _target));
        DisableRagdoll();
    }

    private void Update() => _stateMachine.UpdateCurrentState(Time.deltaTime);
    public void OnHit(Vector3 force, Vector3 hitPoint, float damage)
    {
        var rigidbodyHit = _bodyParts
            .OrderBy(rigidbody => Vector3.Distance(rigidbody.position, hitPoint))
            .First();

        var part = HitBox.BodyPart.None;

        foreach (var hitBox in _hitBoxes)
            if (rigidbodyHit.GetComponent<Collider>() == hitBox.HitCollider)
            {
                part = hitBox.Part;
                break;
            }

        TakeDamage(damage, part, out bool isDied);

        if (isDied)
            OnDied(rigidbodyHit, force, hitPoint);
    }

    private void TakeDamage(float damage, HitBox.BodyPart part, out bool isDied)
    {
        float ratio;
        isDied = false;
        if (damage < 0)
            throw new InvalidOperationException("Try to heal instead of giving damage");
        switch (part)
        {
            case HitBox.BodyPart.Down:
                ratio = DOWN_RATIO;
                break;
            case HitBox.BodyPart.Middle:
                ratio = MIDDLE_RATIO;
                break;
            case HitBox.BodyPart.Top:
                ratio = TOP_RATIO;
                break;
            default:
                return;
        }

        Health -= damage * ratio;
        if (Health <= 0)
            isDied = true;
    }

    private void OnDied(Rigidbody part, Vector3 force, Vector3 hitPoint)
    {
        _stateMachine.ChangeState(new DiedState(EnableRagdoll));
        part.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }


    private void EnableRagdoll()
    {
        foreach (var part in _bodyParts)
            part.isKinematic = false;

        _animator.enabled = false;
        _controller.enabled = false;
    }

    private void DisableRagdoll()
    {
        foreach (var part in _bodyParts)
            part.isKinematic = true;

        _animator.enabled = true;
        _controller.enabled = true;
    }
}
public abstract class ZombieState
{
    public abstract void Enter();
    public abstract void UpdateState(float deltaTime);
}

public class ZombieStateMachine
{
    private ZombieState currentState;

    public void ChangeState(ZombieState newState)
    {
        currentState = newState;
        currentState.Enter();
    }

    public void UpdateCurrentState(float deltaTime)
    {
        if (currentState != null) 
            currentState.UpdateState(deltaTime);
    }
}

public sealed class WalkingState : ZombieState
{
    public WalkingState(Action disable, NavMeshAgent controller,
        Transform target)  
    {
        this.disable = disable;
        this.controller = controller;
        this.target = target;
    }

    private readonly Action disable;
    private readonly NavMeshAgent controller;
    private readonly Transform target;

    public override void Enter() => disable?.Invoke();

    private const float DELTA_ANGLE = 120f;
    private const float SPEED = 0.5f;

    public override void UpdateState(float deltaTime)
    {
        var direction = target.position - controller.transform.position;
        direction = new Vector3(direction.x, 0f, direction.z);
        LookAt(direction, deltaTime);
        Move();
    }

    private void LookAt(Vector3 direction, float deltaTime)
    {
        var toRotation = Quaternion.LookRotation(direction, Vector3.up);
        var transform = controller.transform;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, DELTA_ANGLE * deltaTime);
    }

    private void Move()
    {
        controller.SetDestination(target.position);
    }
}

public sealed class DiedState : ZombieState
{
    public DiedState(Action enable)
    {
        this.enable = enable;
    }
    private readonly Action enable;

    public override void Enter() => enable?.Invoke();

    public override void UpdateState(float deltaTime) { }
}

[Serializable]
public sealed class HitBox
{
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private BodyPart _part;

    public Collider HitCollider => _collider;
    public BodyPart Part => _part;
    public enum BodyPart
    {
        None,
        Down,
        Middle,
        Top
    }

}
