using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpring : MonoBehaviour {

    //[HideInInspector] public bool active;

    [Header("Components")]
    private Collider _col; // May change
    private MeshRenderer _mesh;

    [SerializeField] private float _strengh;

    private void Awake() {
        _col = GetComponent<Collider>();
        _mesh = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Player") collision.transform.GetComponent<Rigidbody>().AddForce(transform.up * _strengh, ForceMode.Acceleration);
    }

    //public void Activate(bool activating) {
    //    active = activating;
    //    _col.enabled = activating;
    //    _mesh.enabled = activating;
    //}
}
