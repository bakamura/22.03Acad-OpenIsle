using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmuletObject : MonoBehaviour {

    [SerializeField] private Material _materialToChange;
    [SerializeField] private LayerMask _enemyLayerMask;
    private Material _standarMaterial;
    private MeshRenderer _meshRender;
    private Collider _collider;

    private void Awake() {
        _meshRender = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
        _standarMaterial = _meshRender.material;
    }

    private void Start() {
        PlayerTools.onActivateAmulet += Changedimension;
    }

    public void Changedimension() {
        if (Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) < PlayerTools.amuletDistance) {
            _collider.enabled = !_collider.enabled;
            if (_collider.enabled) DetectForEnemy();
            if (_meshRender.material == _standarMaterial) _meshRender.material = _materialToChange;
            else _meshRender.material = _standarMaterial;
        }
    }

    // May not be used
    private void DetectForEnemy() {
        //new Vector3(_meshRender.bounds.size.x * transform.localScale.x, _meshRender.bounds.size.y * transform.localScale.y, _meshRender.bounds.size.z * transform.localScale.z)
        Collider[] hits = Physics.OverlapBox(transform.position, transform.lossyScale / 2, Quaternion.identity, _enemyLayerMask);
        foreach (Collider enemy in hits)  enemy.GetComponent<EnemyData>().TakeDamage(999);        
    }
}
