using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmuletObject : MonoBehaviour {
    [SerializeField] private Material _materialToChange;
    private Material _standarMaterial;
    private MeshRenderer _meshRender;
    private Collider _collider;

    private void Awake() {
        _meshRender = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
        _standarMaterial = _meshRender.material;
    }
    private void Start() {
        Amulet.onActivateAmulet += Changedimension;
    }

    public void Changedimension() {
        if (Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) < Amulet.amuletDistance) {
            _collider.enabled = !_collider.enabled;
            if (_meshRender.material == _standarMaterial) _meshRender.material = _materialToChange;
            else _meshRender.material = _standarMaterial;
        }
    }
}
