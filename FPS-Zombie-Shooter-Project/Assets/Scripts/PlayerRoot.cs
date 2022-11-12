using System;
using UnityEngine;

[RequireComponent(typeof(ClientRaycaster), typeof(MovementSystem))]
public class PlayerRoot : MonoBehaviour
{
    [SerializeField]
    private DefaultGun _defaultGun;

    [SerializeField]
    private StressReceiver _cameraShakeFX;

    [SerializeField]
    private FpsWeapon[] _weapons;

    [SerializeField]
    private ClampedBulletTextUI _clampedBulletTextUI;
    [SerializeField]
    private InteractionMessageView _messageView;
    [SerializeField]
    private PlayerHealthView _healthView;

    private InputRouter _inputRouter;
    private WeaponInventory _inventory;
    private ClientRaycaster _detector;
    private MovementSystem _movementSystem;
    private GameOverView _gameOverView;

    private readonly float _interactionDistance = 2f;

    private const float MAX_HEALTH = 200f;
    public float Health { get; private set; } = MAX_HEALTH;

    private Interaction _target;

    private Interaction CurrentTarget
    {
        get => _target;
        set
        {
            if (value == null)
                _messageView.UpdateText(string.Empty);
            else
                _messageView.UpdateText(value.PromptText);

            _target = value;
        }
    }

    private void Awake()
    {
        _gameOverView = FindObjectOfType<GameOverView>();
        _movementSystem = GetComponent<MovementSystem>();
        _detector = GetComponent<ClientRaycaster>();

        _inventory = 
            new WeaponInventory(_weapons, _detector, _clampedBulletTextUI.UpdateData);
        _clampedBulletTextUI.Init(_inventory);

        _inputRouter =
            new InputRouter(_movementSystem, _inventory, () => _target, Time.fixedDeltaTime);
        _healthView.UpdateHealthText(Health);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _detector.TryThrowRay(_interactionDistance, out Interaction interaction);

        if(interaction != null && interaction.CanInteract)
          CurrentTarget = interaction;
        else
          CurrentTarget = null;
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

    public void TakeDamage(float damage)
    {
        if (damage < 0f)
            throw new System.InvalidOperationException();

        Health -= damage;

        if (Health <= 0f)
        {
            Health = 0f;
            OnDie();
        }

        _cameraShakeFX.InduceStress();
        _healthView.UpdateHealthText(Health);
        _healthView.PerformScreenEffect(PlayerHealthView.ScreenFXKind.Hurt);
    }

    public void Heal(float hp)
    {
        if (hp < 0f)
            throw new System.InvalidOperationException();

        if (Health + hp > MAX_HEALTH)
            Health = MAX_HEALTH;
        else
            Health += hp;

        _healthView.UpdateHealthText(Health);
        _healthView.PerformScreenEffect(PlayerHealthView.ScreenFXKind.Heal);

    }
    private void OnDie() => _gameOverView.ReloadLevel();
}