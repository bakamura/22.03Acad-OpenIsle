using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateToolHookShot : MonoBehaviour {

    public static AlternateToolHookShot Instance { get; private set; }

    [Header("Components")]
    private Rigidbody rb;
    private Collider col;

    [Header("Info")]
    [HideInInspector] public bool active;
    private int _state = 0;
    private GameObject _currentTarget;

    private float _currentReturnPoint = 0;
    private Vector3 _lastPoint;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Update() {
        if (!active) {
            transform.position = PlayerData.Instance.activeToolPoint.transform.position;
            transform.rotation = PlayerData.Instance.activeToolPoint.transform.rotation;
        }
        else {
            switch (_state) {
                case 0: // Check if surpass max distance
                    if (Vector3.Distance(PlayerData.Instance.activeToolPoint.transform.position, transform.position) > PlayerTools.instance.maxHookDistance) ReturnHook();
                    break;
                case 1: // Propel Player
                    if (Vector3.Distance(PlayerData.Instance.transform.position, _currentTarget.transform.position) < PlayerTools.instance.hookCorrectionDistance) {
                        EndHook();
                        break;
                    }
                    PlayerData.rb.velocity = (_currentTarget.transform.position - PlayerData.Instance.transform.position).normalized * PlayerTools.instance.playerHookSpeed;
                    break;
                case 2: // Pull Object
                    _currentTarget.GetComponent<Rigidbody>().velocity = (PlayerData.Instance.transform.position - _currentTarget.transform.position).normalized * PlayerTools.instance.objectHookSpeed;
                    transform.position = _currentTarget.transform.position; // Need to change to hitpoint
                    // Need to endHook at some given point
                    break;
                case 3: // Returning
                    if (_currentReturnPoint >= 1) {
                        EndHook();
                        break;
                    }
                    _currentReturnPoint += Time.deltaTime / PlayerTools.instance.hookReturnDuration;
                    transform.position = Vector3.Lerp(_lastPoint, PlayerData.Instance.transform.position, _currentReturnPoint);
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        switch (other.tag) {
            case "HookPoint":
                Debug.Log("HookPoint");
                transform.position = other.transform.position;
                _state = 1;
                _currentTarget = gameObject; //
                rb.velocity = Vector3.zero;
                break;
            case "Enemy":
                Debug.Log("Enemy");
                transform.position = other.transform.position;
                break;
            case "MovableObject": // May not be included
                Debug.Log("MovableObject");
                // Get hit point and correct angle
                _state = 2;
                _currentTarget = other.gameObject;
                rb.velocity = Vector3.zero;
                break;
            default:
                Debug.Log("Not Valid Target Hit");
                ReturnHook();
                break;
        }
    }

    public void InitiateHook() {
        active = true;
        col.enabled = true;
        rb.velocity = Camera.main.transform.forward * PlayerTools.instance.hookSpeed;
        PlayerData.rb.velocity = new Vector3(0, PlayerData.rb.velocity.y, 0);
        PlayerMovement.Instance.movementLock = true;
    }

    private void ReturnHook() {
        _state = 3;
        _currentReturnPoint = 0;
        _lastPoint = transform.position;
    }

    private void EndHook() {
        if (active) {
            active = false;
            _state = 0;
            col.enabled = false;
            rb.velocity = Vector3.zero;
            PlayerTools.instance.EndHook(); //
        }
    }
}
