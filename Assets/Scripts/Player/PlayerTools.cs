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

    [SerializeField] private Mesh _swordMesh;
    [SerializeField] private Material _swordMaterial;
    [SerializeField] private float _swordActionDuration;

    public float swordDamage;
    [SerializeField] private Collider _swordCollider;
    [HideInInspector] public List<Collider> swordCollisions = new List<Collider>();

    [Header("Hook")]

    [SerializeField] private Mesh _hookMesh;
    [SerializeField] private Material _hookMaterial;
    [SerializeField] private CinemachineFreeLook cinemachineCam;
    [HideInInspector] public bool isAiming = false;
    // Naka (alternate script)
    [SerializeField] private AlternateToolHookShot _hookAlternateScript;
    public float hookSpeed;
    public float playerHookSpeed;
    public float objectHookSpeed;
    public float maxHookDistance; // In seconds, when going
    public float hookReturnDuration = 1;
    public float hookCorrectionDistance;
    // Vini
    [SerializeField] private ToolHookShot _hookScript;
    public Transform _hookCameraPoint;
    [SerializeField] private CanvasGroup _hookAimUI;

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
            isAiming = false;
            ChangeCameraFollow(transform);

            Invoke(nameof(SwordStart), 0.1f);
            Invoke(nameof(SwordEnd), 0.4f);
        }
        else if (PlayerMovement.Instance.isGrounded && PlayerData.Instance.hasHook && _currentActionCoolDown <= 0 && PlayerInputs.hookKeyPressed > 0 && !AlternateToolHookShot.Instance.active) {
            PlayerInputs.hookKeyPressed = 0;
            isAiming = true;
            //ChangeCameraFollow(_toolMeshFilter.transform);
            ChangeCameraFollow(_hookCameraPoint);
            ChangeMesh(_hookMesh, _hookMaterial, 0);
        }
        else if (PlayerData.Instance.hasAmulet && _currentActionCoolDown <= 0 && PlayerInputs.amuletKeyPressed > 0) {
            PlayerInputs.amuletKeyPressed = 0;
            ChangeMesh(_amuletMesh, _amuletMaterial, _amuletActionDuration);
            isAiming = false;
            ChangeCameraFollow(transform);

            onActivateAmulet.Invoke(); //
        }
        if (!PlayerMovement.Instance.isGrounded) {
            isAiming = false;
            ChangeCameraFollow(transform);
        }
        if (isAiming) {
            transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
            if (PlayerInputs.hookKeyReleased > 0) {
                PlayerInputs.hookKeyReleased = 0;

                isAiming = false;
                ChangeCameraFollow(transform);

                if (!_hookScript.isHookActive) {
                    _hookScript.SendHitDetection();
                    ChangeMesh(_hookMesh, _hookMaterial, 0);
                    _hookScript.StartHook();
                }

                //_hookAlternateScript.InitiateHook();
            }
        }
        else PlayerInputs.hookKeyReleased = 0;

        _currentActionCoolDown = Mathf.Clamp(_currentActionCoolDown - Time.deltaTime, 0, 100000);

        if (_hookScript.isHookActive && (PlayerInputs.jumpKeyPressed > 0 || PlayerInputs.hookKeyPressed > 0 || PlayerInputs.dashKeyPressed > 0)) _hookScript.CancelHook();
    }

    private void ChangeMesh(Mesh mesh, Material material, float actionDuration) {
        if (actionDuration > 0) {
            //Debug.Log("Movement Lock");
            PlayerMovement.Instance.movementLock = true;
            _currentActionCoolDown = actionDuration + _actionInternalCoolDown;
            Invoke(nameof(UnlockMovement), actionDuration);
        }
        PlayerData.rb.velocity = new Vector3(0, PlayerData.rb.velocity.y, 0);
        _toolMeshFilter.mesh = mesh;
        _toolMeshRenderer.material = material;
    }

    private void UnlockMovement() {
        // May have issues with hook function
        //Debug.Log("Movement Unlock");
        PlayerMovement.Instance.movementLock = false;
    }

    private void SwordStart() {
        _swordCollider.enabled = true;
        swordCollisions.Clear();
    }

    private void SwordEnd() {
        _swordCollider.enabled = false;
    }

    public void EndHook() {
        UnlockMovement();
        _currentActionCoolDown = _actionInternalCoolDown;
    }

    private void ChangeCameraFollow(Transform followObject) {
        _hookAimUI.alpha = (followObject == _hookCameraPoint) ? 1f : 0f;
        cinemachineCam.Follow = followObject;
        cinemachineCam.LookAt = followObject;
    }
}
