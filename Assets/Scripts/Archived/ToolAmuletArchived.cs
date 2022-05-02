using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolAmuletArchived : MonoBehaviour {

    // Amulet 

    //public static float amuletDistance = 5;
    //public static UnityAction onActivateAmulet;
    //[SerializeField] private float _amuletCooldown;
    //private float _currentAmuletCooldown;

    //private void Update() {
    //    if (PlayerInputs.amuletKeyPressed > 0 && PlayerData.Instance.hasAmulet && _currentAmuletCooldown >= _amuletCooldown) Activate();
    //    if (_currentAmuletCooldown < _amuletCooldown) _currentAmuletCooldown += Time.deltaTime;
    //}

    //private void Activate() {
    //    _currentAmuletCooldown = 0;
    //    onActivateAmulet.Invoke();
    //}

    // Sword

    //[Header("Components")]

    //private Collider _hitDetectionBox;
    //private AudioSource _audio;
    //private Animator _animator;

    //private void Awake() {
    //    _hitDetectionBox = GetComponent<Collider>();
    //    _audio = GetComponent<AudioSource>();
    //    _animator = GetComponent<Animator>();
    //}

    //private void Update() {
    //    if (PlayerInputs.swordKeyPressed > 0 && PlayerData.Instance.hasSword) _animator.SetBool("ATTACKING", true);
    //}

    //public void ActivateSword() {
    //    //_audio.Play();
    //    _hitDetectionBox.enabled = true;
    //    collisions.Clear();
    //}

    //public void DeactivateSword() {
    //    _hitDetectionBox.enabled = false;
    //    _animator.SetBool("ATTACKING", false);
    //}
}
