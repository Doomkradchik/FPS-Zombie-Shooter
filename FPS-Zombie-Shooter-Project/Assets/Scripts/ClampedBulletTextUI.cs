using UnityEngine.UI;
using UnityEngine;
using System;

public class ClampedBulletTextUI : MonoBehaviour
{
    [SerializeField]
    private Text _bulletsData;

    private WeaponInventory _inventory;
    private bool _inited = false;

    private void OnEnable()
    {
        if(_inited == false)
        {
            enabled = false;
            return;
        }

        try
        {
            Validate();
        }
        catch (Exception e)
        {
            enabled = false;
            throw e;
        }
        _inventory._weaponChanged += UpdateData;
    }

    public void Init(WeaponInventory inventory)
    {
        _inventory = inventory;
        _inited = true;
        enabled = true;
    }

    private void Validate()
    {
        if (_bulletsData == null)
            throw new InvalidOperationException();
    }

    public void UpdateData(FpsWeapon weapon)
    {
        string data;

        if (weapon is DefaultGun dg)
            data = $"{dg.Bullets} / {dg.MaxBullets}";
        else
            if (weapon is Knife k)
            data = "Infinity";
        else
            throw new InvalidOperationException();

        _bulletsData.text = data;
    }
}
