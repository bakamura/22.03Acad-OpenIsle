using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookShot : MonoBehaviour {

    [SerializeField] private float _hookDistance;
    [SerializeField] private float _hookSpeed;
    [SerializeField] private LayerMask[] _checkLayers = new LayerMask[3]; // 0 = hookPoint, 1 = enemies, 2 = movable objects
    [SerializeField] private AudioClip[] _hitAudios = new AudioClip[4]; // 0 = hookShot, 1 = hit Something Pullable, 2 = hit something unpullable, 3 = pulling
    private bool _isHookActive = false;
    private RaycastHit _hitinfo;
    private AudioSource _audio;
    private string _currentRuningMethodName;
    private void Awake() {
        _audio = GetComponent<AudioSource>();
    }
    private void Update() {
        if (PlayerInputs.hookKeyPressed > 0 && PlayerData.Instance.hasHook && !_isHookActive) HookStart();
        if (PlayerMovement.Instance.movementLock && _isHookActive && (PlayerInputs.jumpKeyPressed > 0 || PlayerInputs.dashKeyPressed > 0)) {
            EndHookMovment();
        }
    }
    private void HookHitDetection() {
        if (Physics.Raycast(PlayerMovement.Instance.transform.position, Camera.main.transform.forward, out _hitinfo, _hookDistance)) {
            LockPlayeMovment();
            if (_hitinfo.collider.gameObject.layer == 7 /*_checkLayers[0]*/) { // hit a hook point
                //Debug.Log("hook point");
                _currentRuningMethodName = nameof(MovePlayerToPoint);
                InvokeRepeating(_currentRuningMethodName, 0f, Time.fixedDeltaTime);
            }
            else if (_hitinfo.collider.gameObject.layer == 8/*_checkLayers[1]*/) { // hit an enemy
                //_currentRuningMethodName = nameof(MovePlayerToPoint);
                //InvokeRepeating(_currentRuningMethodName, 0f, Time.deltaTime);
                _currentRuningMethodName = null;
                //Debug.Log("enemy");
                EndHookMovment();
            }
            else if (_hitinfo.collider.gameObject.layer == 9/*_checkLayers[2]*/) { // hit a movable object
                _currentRuningMethodName = nameof(MovePointToPlayer);
                InvokeRepeating(_currentRuningMethodName, 0f, Time.fixedDeltaTime);
                //Debug.Log("movable object");
            }
            else {
                //_audio.clip = _hitAudios[2];
                //_audio.Play();
                _currentRuningMethodName = null;
                //Debug.Log("hit an unpullable");
                EndHookMovment();
            }

        }
    }

    private void HookStart() {
        //_audio.clip = _hitAudios[0];
        //_audio.Play();
        _isHookActive = true;
        HookHitDetection();
    }

    private void MovePlayerToPoint() {
        float distance = Vector3.Distance(_hitinfo.point, PlayerMovement.Instance.transform.position);
        PlayerData.rb.velocity = (_hitinfo.point - PlayerMovement.Instance.transform.position).normalized * _hookSpeed * distance;
        if (distance < 1f) EndHookMovment();
    }

    private void MovePointToPlayer() {
        float distance = Vector3.Distance(PlayerMovement.Instance.transform.position, _hitinfo.transform.position);
        _hitinfo.rigidbody.velocity = ((PlayerMovement.Instance.transform.position - _hitinfo.transform.position).normalized * _hookSpeed * distance) / _hitinfo.rigidbody.mass;
        if (distance < _hitinfo.transform.lossyScale.magnitude / 2f + 1) {
            _hitinfo.rigidbody.velocity = Vector3.zero;
            EndHookMovment();
        }
    }

    private void LockPlayeMovment() {
        PlayerMovement.Instance.movementLock = true;
        PlayerData.rb.useGravity = false;
    }

    private void EndHookMovment() {
        if (_currentRuningMethodName != null) CancelInvoke(_currentRuningMethodName);
        _currentRuningMethodName = null;
        PlayerMovement.Instance.movementLock = false;
        PlayerData.rb.useGravity = true;
        _isHookActive = false;
    }
}
