using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLightSource : MonoBehaviour {

    [SerializeField] private Vector3 rayDirection = Vector3.forward;
    [SerializeField] private LayerMask detecLayer;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    private void FixedUpdate() {
        RaycastHit hit;
        Physics.Raycast(transform.position, rayDirection, out hit, 64f, detecLayer);
        PuzzleLightMirror mirrorComponent = hit.transform.GetComponent<PuzzleLightMirror>();
        if (mirrorComponent != null) mirrorComponent.lit = 0.2f;
    }

}
