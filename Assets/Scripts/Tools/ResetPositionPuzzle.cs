using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionPuzzle : MonoBehaviour {
    [SerializeField] private Vector3 _pos;
    [SerializeField] private float _damage;
    private bool _playerInArea;

    private void Activate() {
        if (_playerInArea) {
            PlayerData.Instance.TakeDamage(_damage);
            PlayerData.Instance.transform.position = _pos;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) _playerInArea = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) _playerInArea = false;
    }

    private void Start() {
        PlayerTools.onActivateAmulet += Activate;
    }
}
