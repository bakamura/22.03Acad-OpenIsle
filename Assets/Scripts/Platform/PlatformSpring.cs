using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpring : MonoBehaviour {

    [System.NonSerialized] public bool active;
    private Collider _collider; // May change
    private MeshRenderer _mesh;

    [SerializeField] private float _strengh;

    private void Awake() {
        _collider = GetComponent<Collider>();
        _mesh = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Player") collision.transform.GetComponent<Rigidbody>().AddForce(transform.up * _strengh, ForceMode.Acceleration);
    }

    public void Activate(bool activating) {
        active = activating;
        _collider.enabled = activating;
        _mesh.enabled = activating;
    }

}
