using UnityEngine;

public class PlayerRoot : MonoBehaviour
{
    [SerializeField]
    private MovementSystem _movementSystem;
    [SerializeField]
    private DefaultGun _defaultGun;

    [SerializeField]
    private FpsWeapon[] _weapons;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private RectTransform _aimTransform;

    [SerializeField]
    private ClampedBulletTextUI _clampedBulletTextUI;

    private InputRouter _inputRouter;
    private WeaponInventory _inventory;
    private AimTargetFinder _aimTargetFinder;
   
    private void Awake()
    {
        _aimTargetFinder = new AimTargetFinder(_aimTransform, _camera);
        _inventory = 
            new WeaponInventory(_weapons, _aimTargetFinder, _clampedBulletTextUI.UpdateData);
        _clampedBulletTextUI.Init(_inventory);

        _inputRouter =
            new InputRouter(_movementSystem, _inventory, Time.fixedDeltaTime);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        _inputRouter.Enable();
        _inventory.Enable();
    }
    private void OnDisable() => _inputRouter.Disable();

    private void FixedUpdate()
    {
        _inputRouter.FixedUpdate();
    }
}