using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFragile : MonoBehaviour {

    private bool isActive = false;

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
        if (collision.transform.tag == "Player") StartCoroutine(Break());
    }

    private IEnumerator Break() {
        // Animate fragility

        yield return new WaitForSeconds(_delayToBreak);

        // Play sound
        _collider.enabled = false;
        _mesh.enabled = false;

        yield return new WaitForSeconds(_delayToRespawn);

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
