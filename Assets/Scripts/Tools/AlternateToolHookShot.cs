using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateToolHookShot : MonoBehaviour {

    public static AlternateToolHookShot Instance { get; private set; }

    [Header("Components")]
    private Rigidbody rb;
    private Collider col;

    [Header("Info")]
    private bool _active;
    private int _state = 0;
    private GameObject _currentTarget;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Update() {
        switch (_state) {
            case 0: break;
            case 1: // Propel Player
                if (Vector3.Distance(PlayerData.Instance.transform.position, _currentTarget.transform.position) < 0.25f /* Change for a variable to be edited in unity? */) {
                    EndHook();
                    break;
                }
                PlayerData.rb.velocity = (_currentTarget.transform.position - PlayerData.Instance.transform.position).normalized * PlayerTools.instance.playerHookSpeed;
                break;
            case 2: // Pull Object
                _currentTarget.GetComponent<Rigidbody>().velocity = (PlayerData.Instance.transform.position - _currentTarget.transform.position).normalized * PlayerTools.instance.objectHookSpeed;
                transform.position = _currentTarget.transform.position; // Need to change to hitpoint
                break;
            case 3: // Returning
                rb.velocity = (PlayerData.Instance.transform.position - transform.position).normalized / (PlayerTools.instance.maxHookDuration / 2);
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        switch (other.tag) {
            case "HookPoint":
                Debug.Log("HookPoint");
                transform.position = other.transform.position;
                _state = 1;
                _currentTarget = other.gameObject;
                CancelInvoke(nameof(ReturnHook));
                CancelInvoke(nameof(EndHook));
                break;
            case "Enemy":
                Debug.Log("Enemy");
                transform.position = other.transform.position;
                CancelInvoke(nameof(ReturnHook));
                CancelInvoke(nameof(EndHook));
                break;
            case "MovableObject": // May not be included
                Debug.Log("MovableObject");
                // Get hit point and correct angle
                _state = 2;
                _currentTarget = other.gameObject;
                CancelInvoke(nameof(ReturnHook));
                CancelInvoke(nameof(EndHook));
                break;
            default:
                Debug.Log("Not Valid Target Hit");
                break;
        }
    }

    public void InitiateHook() {
        _active = true;
        col.enabled = true;
        rb.velocity = Camera.main.transform.forward * PlayerTools.instance.hookSpeed;
        transform.parent = null;
        Invoke(nameof(ReturnHook), PlayerTools.instance.maxHookDuration);
        Invoke(nameof(EndHook), PlayerTools.instance.maxHookDuration * 1.5f);

    }

    private void ReturnHook() {
        if (_active) _state = 3;
    }

    private void EndHook() {
        if (_active) {
            _active = false;
            _state = 0;
            col.enabled = false;
            rb.velocity = Vector3.zero;
            transform.position = PlayerData.Instance.activeToolPoint.transform.position;
        }
    }
}
