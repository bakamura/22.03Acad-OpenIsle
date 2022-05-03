using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class PlayerTools : MonoBehaviour {

    public static PlayerTools instance { get; private set; }

    [Header("Info")]

    [SerializeField] private float _actionInternalCoolDown;
    private float _currentActionCoolDown;
    [SerializeField] private MeshFilter _toolMeshFilter;
    [SerializeField] private MeshRenderer _toolMeshRenderer;

    [Header("Sword")]

    [SerializeField] private ToolSword _swordScript;
    [SerializeField] private Mesh _swordMesh;
    [SerializeField] private Material _swordMaterial;
    [SerializeField] private float _swordActionDuration;

    public float swordDamage;
    [SerializeField] private Collider _swordCollider;
    [HideInInspector] public List<Collision> swordCollisions = new List<Collision>();

    [Header("Hook")]

    [SerializeField] private ToolHookShot _hookScript;
    [SerializeField] private Mesh _hookMesh;
    [SerializeField] private Material _hookMaterial;
    [SerializeField] private float _hookActionDuration;
    [SerializeField] private CinemachineFreeLook cinemachineCam;
    // Naka alternate script
    private bool _isAiming = false;
    [SerializeField] private AlternateToolHookShot _hookAlternateScript;
    public float hookSpeed;
    public float playerHookSpeed;
    public float objectHookSpeed;
    public float maxHookDuration; // In seconds, when going

    [Header("Amulet")]

    [SerializeField] private Mesh _amuletMesh;
    [SerializeField] private Material _amuletMaterial;
    [SerializeField] private float _amuletActionDuration;

    public static float amuletDistance = 7;
    public static UnityAction onActivateAmulet;

    private void Awake() {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    private void Update() {
        if (PlayerData.Instance.hasSword && _currentActionCoolDown <= 0 && PlayerInputs.swordKeyPressed > 0) {
            PlayerInputs.swordKeyPressed = 0;
            ChangeMesh(_swordMesh, _swordMaterial, _swordActionDuration);
            _isAiming = false;
            ChangeCameraFollow(transform);

            Invoke(nameof(SwordStart), 0.1f);
            Invoke(nameof(SwordEnd), 0.4f);
        }
        else if (PlayerData.Instance.hasHook && _currentActionCoolDown <= 0 && PlayerInputs.hookKeyPressed > 0) {
            PlayerInputs.hookKeyPressed = 0;
            _isAiming = true;
            ChangeCameraFollow(_toolMeshFilter.transform);
            ChangeMesh(_hookMesh, _hookMaterial, 9999);
        }
        else if (PlayerData.Instance.hasAmulet && _currentActionCoolDown <= 0 && PlayerInputs.amuletKeyPressed > 0) {
            PlayerInputs.amuletKeyPressed = 0;
            ChangeMesh(_amuletMesh, _amuletMaterial, _amuletActionDuration);
            _isAiming = false;
            ChangeCameraFollow(transform);

            onActivateAmulet.Invoke(); //
        }
        if (_isAiming && PlayerInputs.hookKeyReleased > 0) {
            PlayerInputs.hookKeyReleased = 0;

            _isAiming = false;
            ChangeCameraFollow(transform);

            //if (!_hookScript.isHookActive) {
            //    PlayerInputs.hookKeyPressed = 0;
            //    _hookScript.SendHitDetection();
            //    ChangeMesh(_hookMesh, _hookMaterial, _hookScript.Duration());
            //    Debug.Log(_hookScript.Duration());
            
            //    _hookScript.StartHook();
            //}

            _hookAlternateScript.InitiateHook();
        }

        _currentActionCoolDown -= Time.deltaTime;

        if (_hookScript.isHookActive && (PlayerInputs.jumpKeyPressed > 0 || PlayerInputs.hookKeyPressed > 0 || PlayerInputs.dashKeyPressed > 0)) _hookScript.CancelHook();
    }

    private void ChangeMesh(Mesh mesh, Material material, float actionDuration) {
        PlayerMovement.Instance.movementLock = true;
        PlayerData.rb.velocity = new Vector3(0, PlayerData.rb.velocity.y, 0);
        _toolMeshFilter.mesh = mesh;
        _toolMeshRenderer.material = material;
        //Debug.Log(actionDuration);
        _currentActionCoolDown = actionDuration + _actionInternalCoolDown;
        Invoke(nameof(UnlockMovement), actionDuration);
    }

    private void UnlockMovement() {
        // May have issues with hook function
        Debug.Log("Movement Unlock");
        PlayerMovement.Instance.movementLock = false;
        //PlayerData.rb.useGravity = true;
    }

    public void EndHook() {
        UnlockMovement();
        _currentActionCoolDown = _actionInternalCoolDown;
    }

    private void SwordStart() {
        _swordCollider.enabled = true;
        swordCollisions.Clear();
    }

    private void SwordEnd() {
        _swordCollider.enabled = false;
    }

    private void ChangeCameraFollow(Transform followObject) {
        cinemachineCam.Follow = followObject;
        cinemachineCam.LookAt = followObject;
    }
}
