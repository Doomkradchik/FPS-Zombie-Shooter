using System;
using System.Linq;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    private Rigidbody[] _bodyParts;
    private CharacterController _controller;
    private Animator _animator;

    private ZombieStateMachine _stateMachine;

    public void OnHit(Vector3 force, Vector3 hitPoint)
    {
        _stateMachine.ChangeState(new DiedState(EnableRagdoll)); // to do

        var rigidbodyHit = _bodyParts
            .OrderBy(rigidbody => Vector3.Distance(rigidbody.position, hitPoint))
            .First();

        rigidbodyHit.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _bodyParts = GetComponentsInChildren<Rigidbody>();
        _stateMachine = new ZombieStateMachine();
        _stateMachine.ChangeState(new WalkingState(DisableRagdoll, _controller, _target));
        DisableRagdoll();
    }

    private void Update() => _stateMachine.UpdateCurrentState(Time.deltaTime);


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
    public WalkingState(Action disable, CharacterController controller,
        Transform target)  
    {
        this.disable = disable;
        this.controller = controller;
        this.target = target;
    }

    private readonly Action disable;
    private readonly CharacterController controller;
    private readonly Transform target;

    public override void Enter() => disable?.Invoke();

    private const float DELTA_ANGLE = 120f;
    private const float SPEED = 0.5f;

    public override void UpdateState(float deltaTime)
    {
        var direction = target.position - controller.transform.position;
        direction = new Vector3(direction.x, 0f, direction.z);
        LookAt(direction, deltaTime);
        Move(direction, deltaTime);
    }

    private void LookAt(Vector3 direction, float deltaTime)
    {
        var toRotation = Quaternion.LookRotation(direction, Vector3.up);
        var transform = controller.transform;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, DELTA_ANGLE * deltaTime);
    }

    private void Move(Vector3 direction, float deltaTime)
    {
        controller.Move(direction * deltaTime * SPEED);
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

