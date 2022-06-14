using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleLightSource : MonoBehaviour {

    [SerializeField] private Vector3 rayDirection = Vector3.forward;
    [SerializeField] private GameObject rayPrefab;
    private GameObject rayInstance;

    private void Start() {
        rayInstance = Instantiate(rayPrefab);
    }

    private void Update() {
        // Casts a Raycast, then move an object to simulate the ray. Triggers 'PuzzleLightMirror' if it's present in the object.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, rayDirection, out hit)) {
            rayInstance.transform.position = (hit.point + transform.position) / 2;
            rayInstance.transform.eulerAngles = new Vector3(90, Mathf.Atan2(rayDirection.x, rayDirection.z) * Mathf.Rad2Deg, 0);
            rayInstance.transform.localScale = new Vector3(0.5f, Vector3.Distance(transform.position, hit.point) / 2 /*REVIEW*/, 0.5f);
        }
        else {
            rayInstance.transform.position = (rayDirection * 256 + transform.position) / 2;
            rayInstance.transform.eulerAngles = new Vector3(90, Mathf.Atan2(rayDirection.x, rayDirection.z) * Mathf.Rad2Deg, 0);
            rayInstance.transform.localScale = new Vector3(0.5f, Vector3.Distance(transform.position, rayDirection * 256) / 2 /*REVIEW*/, 0.5f);
        }
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
