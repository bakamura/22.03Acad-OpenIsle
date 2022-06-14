using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHookShot : MonoBehaviour {

    [SerializeField] private float _hookDistance;//the hook max distance it can travel
    [SerializeField] private float _hookSpeed;
    [SerializeField] private float _hookStunDuration;
    [SerializeField] private LayerMask[] _checkLayers = new LayerMask[3]; //the objects that it will interact with, 0 = hookPoint, 1 = enemies, 2 = movable objects
    [SerializeField] private AudioClip[] _hitAudios = new AudioClip[4]; // 0 = OnShotting, 1 = hit Something Pullable, 2 = hit something unpullable, 3 = pulling
    [SerializeField] private Transform _hookTransform;
    //[SerializeField] private Transform _hookGunTransform;
    [SerializeField] private float _hookShotSizeIncrease;
    [System.NonSerialized] public bool isHookActive = false;
    private bool _canStartPulling = false;
    private RaycastHit _hitinfo;
    private AudioSource _audio;
    private float _targetDistance;
    //private float _initialTargetDistance;
    private float _objectSizeDifference;
    private bool _willHapenMovment;
    private int _movmentCase;
    private Material[] _chainMaterials = new Material[2];
    private float _baseTilingOffset;//stores the offset of the chain texture to be used when the hook is streching
    private bool _enemyStunned = false;

    private void Awake() {
        _audio = GetComponent<AudioSource>();
        int index = 0;
        foreach (MeshRenderer material in _hookTransform.gameObject.GetComponentsInChildren<MeshRenderer>()) {
            _chainMaterials[index] = material.material;
            index++;
        }
        _baseTilingOffset = _chainMaterials[0].mainTextureScale.y;
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
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hitinfo, _hookDistance);
        //_hookTransform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
        if (_hitinfo.collider != null) transform.LookAt(_hitinfo.point);
        else transform.LookAt(Camera.main.transform.position + (Camera.main.transform.forward * _hookDistance));
        UpdateTargetDistance();
        //_initialTargetDistance = _targetDistance;
        //Mathf.Clamp(_initialTargetDistance, 1, _hookDistance);
    }

    //updates the current distance of the object that got hit from the player 
    private void UpdateTargetDistance() {
        if (_hitinfo.collider != null) {
            if (_hitinfo.rigidbody != null) {//hit something that can probablly be pullable
                if (_objectSizeDifference == 0) _objectSizeDifference = Vector3.Distance(_hitinfo.transform.position, _hitinfo.point);
                _targetDistance = Vector3.Distance(PlayerMovement.Instance.transform.position, _hitinfo.transform.position) - _objectSizeDifference;
            }
            else _targetDistance = Vector3.Distance(_hitinfo.point, PlayerMovement.Instance.transform.position);
        }
        else _targetDistance = _hookDistance;;
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

    //strechs and shrinks the hook
    private void HookMeshLine() {
        //UpdateTargetDistance();
        if (_hookTransform.localScale.z < _targetDistance / 2f && !_canStartPulling) {//increases the hook size
            _hookTransform.localScale += new Vector3(0, 0, _hookShotSizeIncrease);            
        }
        else {//now that the hook got to its nedded distance, cheks for the possibilities that it has
            if (!_canStartPulling) {
                HookPosibilities();
                //_audio.clip = _hitAudios[2];
                //_audio.Play();
                _canStartPulling = true;
            }
            UpdateTargetDistance();
            //if collides with something interactable its scale updates with the targetDistance, if not shrinks itself
            Vector3 newScale = _targetDistance != _hookDistance && _willHapenMovment ? new Vector3(1, 1, Mathf.Clamp(_targetDistance / 2f, 1, _hookDistance)) : new Vector3(1, 1, Mathf.Clamp(_hookTransform.localScale.z - _hookShotSizeIncrease * 2, 1, _hookDistance));
            _hookTransform.localScale = newScale;
        }
        //updates the material to match the current lenght of the hook
        foreach (Material mat in _chainMaterials) mat.mainTextureScale = new Vector2(1, _baseTilingOffset * _hookTransform.localScale.z);        
    }

    private void HookPosibilities() {
        if (_hitinfo.collider != null) { // hit something
            if (Contains(_checkLayers[0], _hitinfo.collider.gameObject.layer)) {//hit a hookPoint,
                PlayerData.rb.useGravity = false;
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
                StunEnemy();
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

    private void StunEnemy() {
        if (!_enemyStunned) {
            _hitinfo.collider.GetComponent<EnemyData>().TakeDamage(0, 0, _hookStunDuration);
            _enemyStunned = true;
        }
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
            _enemyStunned = false;
            //_hookTransform.localRotation = Quaternion.identity;
            transform.localRotation = Quaternion.identity;
            PlayerData.rb.useGravity = true;
            if (_hitinfo.rigidbody != null) _hitinfo.rigidbody.velocity = Vector3.zero;
            PlayerMovement.Instance.movementLock = false;
        }
    }

    private void OnDrawGizmos() {
        if (UnityEditor.EditorApplication.isPlaying && _hitinfo.point != null)
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_hitinfo.point, .5f);
    }
}
