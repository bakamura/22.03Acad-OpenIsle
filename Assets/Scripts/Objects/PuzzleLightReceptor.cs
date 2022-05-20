using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleLightReceptor : MonoBehaviour {

    [HideInInspector] public UnityAction onActivate;

    public void Activate() {
        onActivate.Invoke();
    }
}
