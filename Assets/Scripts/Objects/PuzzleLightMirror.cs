using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLightMirror : MonoBehaviour {

    public bool isRotated = false;
    [SerializeField] private LayerMask detecLayer;
    [HideInInspector] public LightHit[] lightHits = new LightHit[4];

    private void Start() {
        PlayerTools.onActivateAmulet += ChangeRotation;
    }

    private void FixedUpdate() {
        for (int i = 0; i < 4; i++) {
            lightHits[i].lit -= Time.fixedDeltaTime;
            if (lightHits[i].lit > 0) {
                RaycastHit hit;
                Physics.Raycast(transform.position, CorrelateDirections(lightHits[i].angleReceived), out hit, detecLayer);
                PuzzleLightMirror mirrorComponent = hit.transform.GetComponent<PuzzleLightMirror>();
                if (mirrorComponent != null) for (int j = 0; j < 4; i++) {
                        if (mirrorComponent.lightHits[j] == null) {
                            mirrorComponent.lightHits[j] = new LightHit(CorrelateDirections(lightHits[j].angleReceived));
                            break;
                        }
                        if (mirrorComponent.lightHits[j].angleReceived == Mathf.Atan2(CorrelateDirections(lightHits[i].angleReceived).x, CorrelateDirections(lightHits[i].angleReceived).z) * Mathf.Rad2Deg) break;
                    }
                else {
                    PuzzleLightReceptor receptorComponent = hit.transform.GetComponent<PuzzleLightReceptor>();
                    if (receptorComponent != null) receptorComponent.Activate();
                }
            }
            else lightHits[i] = null;
        }
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < 4; i++) if (lightHits[i] != null) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, CorrelateDirections(lightHits[i].angleReceived) * Mathf.Rad2Deg);
            }
    }

    private Vector3 CorrelateDirections(float angleReceived) {
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

    private void ChangeRotation() {
        isRotated = !isRotated;
        transform.eulerAngles += new Vector3(0, 180, 0);
    }
}

public class LightHit {

    public float lit = 0.2f;
    public float angleReceived;

    public LightHit(Vector3 fromPos) {
        angleReceived = Mathf.Atan2(fromPos.x, fromPos.z) * Mathf.Rad2Deg;
    }
}