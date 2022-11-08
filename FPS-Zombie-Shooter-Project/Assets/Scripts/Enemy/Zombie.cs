using System;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    private Rigidbody[] _bodyParts;
    private CharacterController _controller;
    private Animator _animator;

    private ZombieStateMachine _stateMachine;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _bodyParts = GetComponentsInChildren<Rigidbody>();
        _stateMachine = new ZombieStateMachine();
        _stateMachine.ChangeState(new WalkingState(EnableRagdoll, DisableRagdoll, _controller, _target));
        DisableRagdoll();
    }

    private void Update() => _stateMachine.UpdateCurrentState(Time.deltaTime);

    [ContextMenu("ChangeToRagdollState")]
    public void ChangeState() => _stateMachine.ChangeState(new NullState());

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
    public abstract void Exit();
}

public class ZombieStateMachine
{
    private ZombieState currentState;

    public void ChangeState(ZombieState newState)
    {
        if (currentState != null)
            currentState.Exit();

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
    public WalkingState(Action enable, Action disable, CharacterController controller,
        Transform target)  
    {
        this.enable = enable;
        this.disable = disable;
        this.controller = controller;
        this.target = target;
    }

    private readonly Action enable;
    private readonly Action disable;
    private readonly CharacterController controller;
    private readonly Transform target;

    public override void Enter() => disable?.Invoke();
    public override void Exit() => enable?.Invoke();

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

public sealed class NullState : ZombieState
{
    public override void Enter() { }

    public override void Exit() { }

    public override void UpdateState(float deltaTime) { }
}

