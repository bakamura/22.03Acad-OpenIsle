using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour {

    [SerializeField] private float _climbSpeed;
    [SerializeField] private KeyCode _climbKey;
    private bool _canClimb = false;


    private void FixedUpdate() {
        if (_canClimb) PlayerData.rb.velocity = new Vector3(PlayerData.rb.velocity.x, (Input.GetKey(_climbKey) ? 1 : -1) * _climbSpeed, PlayerData.rb.velocity.z);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<PlayerData>() != null) {
            _canClimb = true;
            PlayerData.rb.useGravity = false;
            PlayerData._animScript.Laddered(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<PlayerData>() != null) {
            _canClimb = false;
            PlayerData.rb.useGravity = true;
            PlayerData._animScript.Laddered(false);
        }
    }

}
