using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBox : MonoBehaviour {

    [Header("Info")]
    [SerializeField] private float _minimumSpeed = 0.2f;
    private float _size;
    private Vector3 _currentMovement;

    private void Start() {
        _size = transform.localScale.x;
    }

    private void Update() {
        transform.position += _currentMovement * Time.deltaTime;
    }

    // Starts Moving to a direction (in only one axis) until reaches its last position, plus it's scale in that direction
    public void MoveToPosition() {
        if (_currentMovement == Vector3.zero && PlayerData.rb.transform.position.y - (PlayerData.rb.transform.lossyScale.y / 2f) < transform.position.y + (transform.lossyScale.y / 2f) + 0.05f) {
            Vector2 direction = new Vector3(transform.position.x - PlayerData.rb.transform.position.x, transform.position.z - PlayerData.rb.transform.position.z);
            _currentMovement = GetDirection(PlayerMovement.GetAngle(direction.x, direction.y)) * (PlayerData.rb.velocity.magnitude > _minimumSpeed ? PlayerData.rb.velocity.magnitude : _minimumSpeed);
            Invoke(nameof(StopMovement), _size / (PlayerData.rb.velocity.magnitude > _minimumSpeed ? PlayerData.rb.velocity.magnitude : _minimumSpeed));
        }
    }

    // Stops the movement from 'MoveToPosition'
    private void StopMovement() {
        _currentMovement = Vector3.zero;
    }

    // Translates an angle (float) to a Vector3
    private Vector3 GetDirection(float angle) {
        switch (angle) {
            case float a when a >= 315 || a < 45: return Vector3.right;
            case float a when a >= 45 && a < 135: return Vector3.forward;
            case float a when a >= 135 && a < 225: return Vector3.left;
            case float a when a >= 225 && a < 315: return Vector3.back;
            default: return Vector3.zero; // Never called
        }
    }

    // Stops the movement if the box is hitting a wall
    private void OnCollisionStay(Collision collision) {
        if (collision.transform.tag == "Ground") {
            if (_currentMovement == Vector3.zero) Debug.Log("Couldn't Stop Movement of box because it wasn't moving already!");
            else Debug.Log("movement stop");
            Vector3 direction = transform.position - collision.contacts[0].point;
            if (direction.x > direction.z) direction = direction.x > 0 ? Vector3.right : Vector3.left;
            else direction = direction.z > 0 ? Vector3.forward : Vector3.back;
            transform.position += direction * 0.025f;
            _currentMovement = Vector3.zero;
        }
    }
}
