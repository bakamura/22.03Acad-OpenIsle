using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public void SetParticleState(bool isActivated) {
        gameObject.SetActive(isActivated);
    }
}
