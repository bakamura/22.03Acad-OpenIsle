using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleLightReceptor : MonoBehaviour {

    [HideInInspector] public UnityAction onActivate;

    // Calls a method when is hit by a ray from 'PuzzleLightMirror'
    public void Activate() {
        onActivate?.Invoke();
        this.enabled = false;
    }
}
