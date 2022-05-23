using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleLightReceptor : MonoBehaviour {

    [HideInInspector] public UnityAction onActivate;
    private Vector3 baseScale;

    // Debug
    private void Start() {
        baseScale = transform.localScale;
        onActivate += DebugMethod;
    }

    public void Activate() {
        onActivate.Invoke();
    }

    // Debug
    private void DebugMethod() {
        transform.localScale = 2 * baseScale;
    }
}
