using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHookShot : MonoBehaviour {

    [SerializeField] private float _hookDistance;
    [SerializeField] private float _hookSpeed;
    [SerializeField] private LayerMask[] _checkLayers = new LayerMask[3]; // 0 = hookPoint, 1 = enemies, 2 = movable objects
    [SerializeField] private AudioClip[] _hitAudios = new AudioClip[4]; // 0 = hookShot, 1 = hit Something Pullable, 2 = hit something unpullable, 3 = pulling
    [SerializeField] private Transform _hookTransform;
    [SerializeField] private float _hookShotSizeIncrease;
    [System.NonSerialized] public bool isHookActive = false;
    private bool _canStartPulling = false;
    private RaycastHit _hitinfo;
    private AudioSource _audio;
    private float _targetDistance;
    private float _initialTargetDistance;
    private float _objectSizeDifference;
    private bool _willHapenMovment;
    private int _movmentCase;

    private void Awake() {
        _audio = GetComponent<AudioSource>();
    }

    private void FixedUpdate() {
        if (isHookActive) {
            HookMeshLine();
            if (_canStartPulling) {
                HookMovment();
            }
        }
    }

    public void SendHitDetection() {
        Physics.Raycast(PlayerMovement.Instance.transform.position, Camera.main.transform.forward, out _hitinfo, _hookDistance);
        _hookTransform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);//Quaternion.LookRotation(Camera.main.transform.forward);//check for better calc//Quaternion.FromToRotation(_hookTransform.position, _hitinfo.point);
        UpdateTargetDistance();
        _initialTargetDistance = _targetDistance;
        Mathf.Clamp(_initialTargetDistance, 1, _hookDistance);
    }
    private void UpdateTargetDistance() {
        if (_hitinfo.collider != null) {
            if (_hitinfo.rigidbody != null) {
                if (_objectSizeDifference == 0)_objectSizeDifference = Vector3.Distance(_hitinfo.transform.position, _hitinfo.point);
                _targetDistance = Vector3.Distance(PlayerMovement.Instance.transform.position, _hitinfo.transform.position) - _objectSizeDifference;
            }
            else _targetDistance = Vector3.Distance(_hitinfo.point, PlayerMovement.Instance.transform.position);
        }
        else _targetDistance = _hookDistance;
    }

    //public float Duration() {
    //    float result;
    //    if (_targetDistance != _hookDistance) {
    //        if (_hitinfo.rigidbody != null)
    //            result = ObjectvelocityCalc().magnitude / _targetDistance;
    //        else result = PlayerVelocityCalc().magnitude * _targetDistance;
    //    }
    //    else result = _targetDistance / _hookShotSizeIncrease;
    //    return  (_targetDistance / _hookShotSizeIncrease + result) * Time.fixedDeltaTime;
    //}

    public void StartHook() {
        isHookActive = true;
        PlayerMovement.Instance.movementLock = true;
        //_audio.clip = _hitAudios[0];
        //_audio.Play();
    }

    private void HookMeshLine() {
        UpdateTargetDistance();
        if (_hookTransform.localScale.z < _targetDistance && !_canStartPulling) {
            _hookTransform.localScale += new Vector3(0, 0, _hookShotSizeIncrease);
        }
        else {
            if (!_canStartPulling) {
                HookPosibilities();
                //_audio.clip = _hitAudios[2];
                //_audio.Play();
            }
            _canStartPulling = true;
            _hookTransform.localScale = _targetDistance != _hookDistance && _willHapenMovment ? new Vector3(1, 1, Mathf.Clamp(_targetDistance, 1, _hookDistance)) : new Vector3(1, 1, Mathf.Clamp(_hookTransform.localScale.z - _hookShotSizeIncrease * 2, 1, _hookDistance));
        }
    }

    private void HookPosibilities() {
        if (_hitinfo.collider != null) { // hit a hook point
            if (Contains(_checkLayers[0], _hitinfo.collider.gameObject.layer)) {
                PlayerData.rb.useGravity = false; //
                _movmentCase = 1;
                return;
            }
            else if (Contains(_checkLayers[1], _hitinfo.collider.gameObject.layer)) { // hit an enemy
                _movmentCase = 2;
                return;
            }
            else if (Contains(_checkLayers[2], _hitinfo.collider.gameObject.layer)) { // hit a movable object
                _movmentCase = 3;
                return;
            }
        }
        _movmentCase = 0;
    }

    private void HookMovment() {
        switch (_movmentCase) {
            case 1:
                MovePlayerToPoint();
                break;
            case 2:
                break;
            case 3:
                MovePointToPlayer();
                break;
            default:
                break;
        }
        EndHookMovment();
    }

    private bool Contains(LayerMask mask, int layer) {
        return mask == (mask | (1 << layer));
    }


    private void MovePlayerToPoint() {
        _willHapenMovment = true;
        PlayerData.rb.velocity = new Vector3(PlayerVelocityCalc().x, PlayerVelocityCalc().y, PlayerVelocityCalc().z);
    }

    private void MovePointToPlayer() {
        _willHapenMovment = true;
        _hitinfo.rigidbody.velocity = ObjectvelocityCalc();
    }

    private Vector3 PlayerVelocityCalc() {
        return _hookSpeed * /*_initialTargetDistance */(_hitinfo.point - PlayerMovement.Instance.transform.position).normalized; // May need clamping
    }

    private Vector3 ObjectvelocityCalc() {
        return _hookSpeed * /*_initialTargetDistance */ (PlayerMovement.Instance.transform.position - _hitinfo.transform.position).normalized / _hitinfo.rigidbody.mass /* _initialTargetDistance*/;
    }

    public void CancelHook() {
        if (_hookTransform.localScale.z > 1) _hookTransform.localScale = new Vector3(_hookTransform.localScale.x, _hookTransform.localScale.y, 1);
        EndHookMovment();
    }

    public void EndHookMovment() {
        if (_hookTransform.localScale.z <= 1) {
            _objectSizeDifference = 0;
            _canStartPulling = false;
            isHookActive = false;
            _willHapenMovment = false;
            _hookTransform.localRotation = Quaternion.identity;
            PlayerData.rb.useGravity = true;
            if (_hitinfo.rigidbody != null) _hitinfo.rigidbody.velocity = Vector3.zero;
            PlayerMovement.Instance.movementLock = false;
        }
    }
}
