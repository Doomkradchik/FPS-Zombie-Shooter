using System;
using System.Collections.Generic;

public class WeaponInventory
{
    public WeaponInventory(FpsWeapon[] start,
        AimTargetFinder aimTarget, Action<FpsWeapon> onDataChanged)
    {
        _weapons = start;
        if (start.Length == 0)
            throw new InvalidOperationException();

        foreach (var weapon in _weapons)
            weapon.Init(aimTarget, onDataChanged);
    }

    public event Action<FpsWeapon> _weaponChanged;
    public IEnumerable<FpsWeapon> Weapons => _weapons;
    private FpsWeapon[] _weapons = null;
    private int _currentIndex = 0;

    public FpsWeapon Current => _weapons[_currentIndex];

    public void Enable()
    {
        foreach (var weapon in _weapons)
            weapon.Hide();

        _weapons[0].Unhide();
        _weaponChanged?.Invoke(_weapons[0]);
    }

    public void Next()
    {
        _weapons[_currentIndex].Hide();
        _currentIndex++;

        if (_weapons.Length < _currentIndex + 1)
            _currentIndex = 0;

        _weapons[_currentIndex].Unhide();
        _weaponChanged?.Invoke(_weapons[_currentIndex]);
    }

}
