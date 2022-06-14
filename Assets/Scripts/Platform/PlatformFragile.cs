using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFragile : MonoBehaviour {

    private bool isActive = true;

    [Header("Components")]
    private Collider _col;
    private MeshRenderer _mesh;

    [Header("Info")]
    [SerializeField] private float _delayToBreak;
    [SerializeField] private float _delayToRespawn;
    private bool _hasMovement = false;

    private void Awake() {
        _col = GetComponent<Collider>();
        _mesh = GetComponent<MeshRenderer>();
    }

    private void Start() {
        if (GetComponent<PlatformMoving>() != null) _hasMovement = true;
    }

    // Starts Countdown to break itself
    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Player" && PlayerData.rb.transform.position.y - (PlayerData.rb.transform.lossyScale.y / 2f) > transform.position.y + (transform.lossyScale.y / 2f) - 0.05f) {
            // Animate Shake
            Invoke(nameof(Break), _delayToBreak);
        }
    }

    // Disables Collider and Visibility
    private void Break() {
        // Play sound
        _col.enabled = false;
        _mesh.enabled = false;
        if (_hasMovement && PlayerData.Instance.transform.parent == transform) PlayerData.Instance.transform.parent = null;
        Invoke(nameof(Respawn), _delayToRespawn);
    }

    // Enables Collider and Visibility
    private void Respawn() {
        if (isActive) {
            _col.enabled = true;
            _mesh.enabled = true;
        }
    }
}
