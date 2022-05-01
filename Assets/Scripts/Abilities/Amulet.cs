using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Amulet : MonoBehaviour
{
    public static float amuletDistance = 5;
    public static UnityAction onActivateAmulet;
    [SerializeField] private float _amuletCooldown;
    private float _currentAmuletCooldown;

    private void Update() {
        if (PlayerInputs.amuletKeyPressed > 0 && PlayerData.Instance.hasAmulet && _currentAmuletCooldown >= _amuletCooldown) Activate();
        if (_currentAmuletCooldown < _amuletCooldown) _currentAmuletCooldown += Time.deltaTime;
    }

    private void Activate() {
        _currentAmuletCooldown = 0;
        onActivateAmulet.Invoke();
    }
}
