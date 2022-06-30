using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLightMirror : MonoBehaviour {

    public bool isRotated = false;
    [HideInInspector] public LightHit[] lightHits = new LightHit[2];
    [SerializeField] private GameObject rayPrefab;
    private GameObject[] rayInstance = new GameObject[2];

    private void Start() {
        for (int i = 0; i < rayInstance.Length; i++) rayInstance[i] = Instantiate(rayPrefab);
        PlayerTools.onActivateAmulet += ChangeRotation;
        if(isRotated) transform.eulerAngles += new Vector3(0, 180, 0);
    }

    private void FixedUpdate() {
        // Casts a ray if is being hit from a specific direction, from 'PuzzleLightSource' or 'PuzzleLightMirror', simulating a 90° reflection with an object, based on the reflector's angle

        for (int i = 0; i < rayInstance.Length; i++) {
            if (lightHits[i] != null) {
                lightHits[i].lit -= Time.fixedDeltaTime;
                if (lightHits[i].lit > 0) {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, CorrelateDirections(lightHits[i].angleReceived), out hit)) {
                        rayInstance[i].GetComponent<MeshRenderer>().enabled = true;
                        rayInstance[i].transform.position = (hit.point + transform.position) / 2;
                        rayInstance[i].transform.eulerAngles = new Vector3(90, Mathf.Atan2(CorrelateDirections(lightHits[i].angleReceived).x, CorrelateDirections(lightHits[i].angleReceived).z) * Mathf.Rad2Deg, 0); // Test
                        rayInstance[i].transform.localScale = new Vector3(0.5f, Vector3.Distance(transform.position, hit.point) / 2f /*REVIEW*/, 0.5f);
                    }
                    else {
                        rayInstance[i].GetComponent<MeshRenderer>().enabled = true;
                        rayInstance[i].transform.position = (CorrelateDirections(lightHits[i].angleReceived) * 256 + transform.position) / 2;
                        rayInstance[i].transform.eulerAngles = new Vector3(90, Mathf.Atan2(CorrelateDirections(lightHits[i].angleReceived).x, CorrelateDirections(lightHits[i].angleReceived).z) * Mathf.Rad2Deg, 0); // Test
                        rayInstance[i].transform.localScale = new Vector3(0.5f, Vector3.Distance(transform.position, CorrelateDirections(lightHits[i].angleReceived) * 256) / 2f /*REVIEW*/, 0.5f);
                    }
                    PuzzleLightMirror mirrorComponent = hit.transform.GetComponent<PuzzleLightMirror>();
                    if (mirrorComponent != null) {
                        for (int j = 0; j < 4; i++) {
                            if (mirrorComponent.lightHits[j] == null) {
                                mirrorComponent.lightHits[j] = new LightHit(CorrelateDirections(lightHits[j].angleReceived));
                                break;
                            }
                            if (mirrorComponent.lightHits[j].angleReceived == Mathf.Atan2(CorrelateDirections(lightHits[i].angleReceived).x, CorrelateDirections(lightHits[i].angleReceived).z) * Mathf.Rad2Deg) break;
                        }
                    }
                    else {
                        PuzzleLightReceptor receptorComponent = hit.transform.GetComponent<PuzzleLightReceptor>();
                        if (receptorComponent != null) receptorComponent.Activate();
                    }
                }
                else lightHits[i] = null;
            }
            else {
                rayInstance[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < 2; i++) if (lightHits[i] != null) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, CorrelateDirections(lightHits[i].angleReceived) * Mathf.Rad2Deg);
            }
    }

    // Associates the direction it receives from to the direction the deflection goes
    private Vector3 CorrelateDirections(float angleReceived) {
        if (isRotated) {
            switch (angleReceived) {
                case float a when a < 45: return Vector3.right;
                case float a when a < 135: return Vector3.forward;
                case float a when a < 225: return Vector3.left;
                case float a when a < 285: return Vector3.back;
                default: return Vector3.forward; //
            }
        }
        else {
            switch (angleReceived) {
                case float a when a < 45: return Vector3.left;
                case float a when a < 135: return Vector3.back;
                case float a when a < 225: return Vector3.right;
                case float a when a < 285: return Vector3.forward;
                default: return Vector3.back; //
            }
        }
    }

    // Changes the angle of the object, affecting the direction the deflection goes
    public void ChangeRotation() {
        if (Vector3.Distance(PlayerData.Instance.transform.position, transform.position) < 2) {
            isRotated = !isRotated;
            transform.eulerAngles += new Vector3(0, 90, 0);
        }
    }
}

// Stores info about being hit by a ray from a specific angle.
public class LightHit {

    public float lit = 0.2f;
    public float angleReceived;

    public LightHit(Vector3 fromPos) {
        angleReceived = Mathf.Atan2(fromPos.x, fromPos.z) * Mathf.Rad2Deg;
    }
}