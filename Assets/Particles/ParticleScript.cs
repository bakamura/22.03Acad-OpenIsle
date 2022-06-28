using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    private ParticleSystem _particle;
    ParticleSystem.MainModule _mainModule;
    [SerializeField] private Color _color;
    private void Awake() {
        _particle = GetComponent<ParticleSystem>();
        _mainModule = _particle.main;
    }
    private void Update() {
        //_mainModule.startColor = new Color(_mainModule.startColor.color.r, _mainModule.startColor.color.g, _mainModule.startColor.color.b, 
        //    Mathf.Clamp(Vector3.Distance(transform.position, PlayerData.Instance.transform.position), 0f, 1f));
        _mainModule.startColor = new Color(_color.r, _color.g, _color.b, _color.a);
    }
}
