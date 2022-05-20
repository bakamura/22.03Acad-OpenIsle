using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleLightSource : MonoBehaviour {

    [SerializeField] private Vector3 rayDirection = Vector3.forward;
    [SerializeField] private LayerMask detecLayer;

    private void FixedUpdate() {
        RaycastHit hit;
        Physics.Raycast(transform.position, rayDirection, out hit, 64f, detecLayer);
        PuzzleLightMirror mirrorComponent = hit.transform.GetComponent<PuzzleLightMirror>();
        if (mirrorComponent != null) {
            for (int i = 0; i < mirrorComponent.lightHits.Length; i++) {
                if (mirrorComponent.lightHits[i] == null) {
                    mirrorComponent.lightHits[i] = new LightHit(rayDirection);
                    break;
                }
                if (mirrorComponent.lightHits[i].angleReceived == Mathf.Atan2(rayDirection.x, rayDirection.z) * Mathf.Rad2Deg) break;
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, rayDirection);
    }
}
