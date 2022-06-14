using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleLightReceptor : MonoBehaviour {

    [HideInInspector] public UnityAction onActivate;
    private Vector3 baseScale;

    
    private void Start() {
        // Debug
        baseScale = transform.localScale;
        onActivate += DebugMethod;
    }

    // Calls a method when is hit by a ray from 'PuzzleLightMirror'
    public void Activate() {
        onActivate.Invoke();
    }

    // Debug
    private void DebugMethod() {
        transform.localScale = 2 * baseScale;
    }
}
