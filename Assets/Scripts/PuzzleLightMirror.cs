using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLightMirror : MonoBehaviour {

    public bool isRotated = false;
    [HideInInspector] public float lit = 0f;
    [SerializeField] private LayerMask detecLayer;

    private void FixedUpdate() {
        lit -= Time.fixedDeltaTime;
        if (lit > 0) {
            RaycastHit hit;
            Physics.Raycast(transform.position, CorrelateDirections(), out hit, detecLayer);
            PuzzleLightMirror mirrorComponent = hit.transform.GetComponent<PuzzleLightMirror>();
            if (mirrorComponent != null) mirrorComponent.lit = 0.2f; //
            else {
                PuzzleLightReceptor receptorComponent = hit.transform.GetComponent<PuzzleLightReceptor>();
                if (receptorComponent != null) receptorComponent.Activate();
            }
        }
    }

    private Vector3 CorrelateDirections() {
        // Calc rotation light received;
        float angleReceived = 0; // To be changed
        if (isRotated) {
            switch (angleReceived) {
                case float a when a < 45: return Vector3.forward;
                case float a when a < 135: return Vector3.right;
                case float a when a < 225: return Vector3.back;
                case float a when a < 285: return Vector3.left;
                default: return Vector3.forward; //
            }
        }
        else {
            switch (angleReceived) {
                case float a when a < 45: return Vector3.back;
                case float a when a < 135: return Vector3.left;
                case float a when a < 225: return Vector3.forward;
                case float a when a < 285: return Vector3.right;
                default: return Vector3.back; //
            }
        }
    }
}
