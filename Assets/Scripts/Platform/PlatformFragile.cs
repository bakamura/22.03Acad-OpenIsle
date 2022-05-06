using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFragile : MonoBehaviour {

    private bool isActive = true;

    [Header("Components")]
    private Collider _collider; // Rename?
    private MeshRenderer _mesh;

    [Header("Info")]
    [SerializeField] private float _delayToBreak;
    [SerializeField] private float _delayToRespawn;

    private void Awake() {
        _collider = GetComponent<Collider>();
        _mesh = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Player") {
            Invoke(nameof(Shake), 0);
            Invoke(nameof(Break), _delayToBreak);
            Invoke(nameof(Respawn), _delayToRespawn);
        }
    }

    private void Shake() {
        // Animate
    }

    private void Break() {
        // Play sound
        _collider.enabled = false;
        _mesh.enabled = false;
    }

    private void Respawn() {
        if (isActive) {
            _collider.enabled = true;
            _mesh.enabled = true;
        }
    }

    private void Activate(bool activating) {
        isActive = activating;
        _collider.enabled = activating;
        _mesh.enabled = activating;
    }

}
