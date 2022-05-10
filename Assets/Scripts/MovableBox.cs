using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBox : MonoBehaviour {

    [Header("Info")]
    //public float movementSpeed;
    private float _internalCooldown = 0;
    private float _size;
    private Vector3 _currentMovement;

    private void Start() {
        _size = transform.localScale.x;
    }

    private void Update() {
        transform.position += _currentMovement * Time.deltaTime;
        _internalCooldown -= Time.deltaTime;
    }

    public void MoveToPosition() {
        if (_internalCooldown <= 0 && PlayerData.rb.transform.position.y - (PlayerData.rb.transform.lossyScale.y / 2f) < transform.position.y + (transform.lossyScale.y / 2f) + 0.05f) {
            Vector2 direction = new Vector3(transform.position.x - PlayerData.rb.transform.position.x, transform.position.z - PlayerData.rb.transform.position.z);
            switch (PlayerMovement.GetAngle(direction.x, direction.y)) {
                case float a when a >= 315 || a < 45:
                    _currentMovement = Vector3.right * PlayerData.rb.velocity.magnitude;
                    break;
                case float a when a >= 45 && a < 135:
                    _currentMovement = Vector3.forward * PlayerData.rb.velocity.magnitude;
                    break;
                case float a when a >= 135 && a < 225:
                    _currentMovement = Vector3.left * PlayerData.rb.velocity.magnitude;
                    break;
                case float a when a >= 225 && a < 315:
                    _currentMovement = Vector3.back * PlayerData.rb.velocity.magnitude;
                    break;
            }
            Invoke(nameof(StopMovement), _size / PlayerData.rb.velocity.magnitude);
            _internalCooldown = 0.4f;
        }
    }

    private void StopMovement() {
        _currentMovement = Vector3.zero;
    }

    private void OnCollisionStay(Collision collision) { // REMAKE
        Debug.Log(collision.transform.name);
        if(collision.transform.tag == "Ground") {
            if (_currentMovement == Vector3.zero) Debug.Log("Couldn't Stop Movement of box because it wasn't moving already!");
            else Debug.Log("movement stop");
            transform.position -= _currentMovement * Time.deltaTime;
            _currentMovement = Vector3.zero;
        }
    }
}
