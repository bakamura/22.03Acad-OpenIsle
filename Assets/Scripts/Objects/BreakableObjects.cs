using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObjects : MonoBehaviour {

    [SerializeField] private ParticleSystem _destructionParticles;

    // Creates particles and destroys itself when triggered
    public void DestroyObject() {
        Instantiate(_destructionParticles, transform);
        Destroy(gameObject);
    }
}
