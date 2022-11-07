using UnityEngine;
using UnityEngine.InputSystem;

public class InputRouter
{
    public InputRouter(MovementSystem movementSystem, WeaponInventory inventory,
        float fixedDeltaTime)
    {
        _input = new PlayerInput2();
        _movementSystem = movementSystem;
        _inventory = inventory;
        _fixedDeltaTime = fixedDeltaTime;
    }

    private PlayerInput2 _input;
    private readonly MovementSystem _movementSystem;
    private readonly WeaponInventory _inventory;
    private readonly float _fixedDeltaTime;

    public void Enable()
    {
        _input.Enable();
        _input.Player.Hit.performed += OnHit;
        _input.Player.ChangeWeapon.performed += OnChangeWeapon;
        _input.Gun.Reload.performed += OnReload;
    }

    public void Disable()
    {
        _input.Disable();
        _input.Player.Hit.performed -= OnHit;
        _input.Player.ChangeWeapon.performed += OnChangeWeapon;
        _input.Gun.Reload.performed -= OnReload;
    }

    public void FixedUpdate()
    {
        var direction = _input.Player.Movement.ReadValue<Vector2>();
        var mouseDelta = _input.Player.MouseDelta.ReadValue<Vector2>();

        _movementSystem.Move(direction);
        _movementSystem.LookAt(mouseDelta.normalized);
    }

    private void OnReload(InputAction.CallbackContext obj)
    {
        if(_inventory.Current is DefaultGun dg)
        {
            dg.Reload();
        }
    }

    private void OnHit(InputAction.CallbackContext obj)
    {
        _inventory.Current.Hit();
    }

    private void OnChangeWeapon(InputAction.CallbackContext obj)
    {
        _inventory.Next();
    }






}