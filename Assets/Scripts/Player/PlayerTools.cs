using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    // public AudioClip swordAudio

    public float swordDamage;
    [SerializeField] private Collider _swordCollider;
    [System.NonSerialized] public List<Collision> swordCollisions = new List<Collision>();

    [Header("Hook")]

    [SerializeField] private ToolHookShot _hookScript;
    [SerializeField] private Mesh _hookMesh;
    [SerializeField] private Material _hookMaterial;
    [SerializeField] private float _hookActionDuration;

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
            ChangeMesh(_swordMesh, _swordMaterial, _amuletActionDuration);

            Invoke(nameof(SwordStart), 0.1f);
            Invoke(nameof(SwordEnd), 0.4f);
        }
        else if (PlayerData.Instance.hasHook && _currentActionCoolDown <= 0 && PlayerInputs.hookKeyPressed > 0) {
            if (!_hookScript.isHookActive) {
                PlayerInputs.hookKeyPressed = 0;
                ChangeMesh(_hookMesh, _hookMaterial, _amuletActionDuration);

                // PlayerData.rb.useGravity = false;
                _hookScript.HookStart();
                // CHANGE CURRENT ACTION COOLDOWN BASED ON HOOK HIT
            }
        }
        else if (PlayerData.Instance.hasAmulet && _currentActionCoolDown <= 0 && PlayerInputs.amuletKeyPressed > 0) {
            PlayerInputs.amuletKeyPressed = 0;
            ChangeMesh(_amuletMesh, _amuletMaterial, _amuletActionDuration);

            onActivateAmulet.Invoke(); //
        }

        _currentActionCoolDown -= Time.deltaTime;

        if (_hookScript.isHookActive && (PlayerInputs.jumpKeyPressed > 0 || PlayerInputs.hookKeyPressed > 0 || PlayerInputs.dashKeyPressed > 0)) _hookScript.EndHookMovment();
    }

    private void ChangeMesh(Mesh mesh, Material material, float actionDuration) {
        PlayerMovement.Instance.movementLock = true;
        PlayerData.rb.velocity = Vector3.zero;
        _toolMeshFilter.mesh = mesh;
        _toolMeshRenderer.material = material;
        _currentActionCoolDown = actionDuration + _actionInternalCoolDown;
        Invoke(nameof(UnlockMovement), actionDuration);
    }

    private void UnlockMovement() {
        // May have issues with hook function
        PlayerMovement.Instance.movementLock = false;
    }

    private void SwordStart() {
        _swordCollider.enabled = true;
        swordCollisions.Clear();
    }

    private void SwordEnd() {
        _swordCollider.enabled = false;
    }
}
