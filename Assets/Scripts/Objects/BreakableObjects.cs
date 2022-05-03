using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObjects : MonoBehaviour
{
    [SerializeField] private ParticleSystem _destructionParticles;

    public void DestroyObject() {
        Instantiate(_destructionParticles, transform);
        Destroy(this.gameObject);
    }
}
