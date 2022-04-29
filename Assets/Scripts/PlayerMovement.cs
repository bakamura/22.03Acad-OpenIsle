using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement Instance { get; private set; }

    [Header("Movement")]

    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _maxAirVelocity = 300f; // ???
    [SerializeField] private float _airVelocityDecrease = .25f; // ???
    [Tooltip("How Fast the object rotates to input")]
    [SerializeField] private float _turnSmoothTime = .1f;
    private float _turnSmoothVelc; // Used by SmoothDampAngle because a static function can't store states.
    private float _currentDashSpeed; //

    [Header("Jump")]

    [SerializeField] private float _jumpStrengh = 100f;
    [SerializeField] private Vector3 groundCheckPoint;
    [SerializeField] private Vector3 _boxSize = Vector3.one;
    [SerializeField] private LayerMask groundLayer;
    private bool _isGrounded;
    // (4) Unnecessary?
    [SerializeField] private float _distance;
    private bool _hit;
    private RaycastHit _hitinfo;
    private float _airVelocity;


    [Header("Dash")]

    [SerializeField] private float _dashSpeed = 1000f;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Update() {
        // Jump
        if (PlayerInputs.jumpKeyPressed && _isGrounded) {
            PlayerInputs.jumpKeyPressed = false;
            PlayerData.rb.AddForce(transform.up * _jumpStrengh, ForceMode.Force); // Check forcemode

            // Unused for now
            _airVelocity = _jumpStrengh;
        }

        // Dash
        if (PlayerInputs.dashKeyPressed) {
            PlayerInputs.dashKeyPressed = false;
            _currentDashSpeed = _dashSpeed;
        }
    }

    private void FixedUpdate() {
        GroundCheck();

        Vector3 TotalMovment = new Vector3(GroundMovment().x, /*AirMovment() / _movementSpeed */ PlayerData.rb.velocity.y, GroundMovment().z); // GRAVITY TOO HEAVY
        PlayerData.rb.velocity = TotalMovment * _movementSpeed * _currentDashSpeed;
        _currentDashSpeed = 1; // Instead, make player lose control while dashing, it's simpler
    }

    private Vector3 GroundMovment() {
        if (PlayerInputs.horizontalAxis != 0 || PlayerInputs.verticalAxis != 0) {
            Vector3 groundMovment = GetMovmentDirection(new Vector3(PlayerInputs.horizontalAxis, 0, PlayerInputs.verticalAxis)).normalized;
            return groundMovment;
        }
        else return Vector3.zero;
    }

    private Vector3 GetMovmentDirection(Vector3 direction) {
        float targetLookAngle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg) + Camera.main.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref _turnSmoothVelc, _turnSmoothTime), 0);
        Vector3 moveDirection = Quaternion.Euler(0, targetLookAngle, 0) * Vector3.forward;
        return moveDirection;
    }

    private void GroundCheck() {
        _hit = Physics.OverlapBox(groundCheckPoint, _boxSize / 2, Quaternion.identity, groundLayer) != null;
        // Physics.BoxCast(groundCheckPoint, _boxSize, -transform.up, out _hitinfo, Quaternion.identity, _distance, groundLayer);
        if (_hit) {
            _isGrounded = true;
            if (_airVelocity < 0) _airVelocity = 0;
        }
        else _isGrounded = false;
    }

    private float AirMovment() {
        if (!_isGrounded) {
            _airVelocity -= _jumpStrengh * _airVelocityDecrease;
            Mathf.Clamp(_airVelocity, -_maxAirVelocity, _maxAirVelocity);
        }
        return _airVelocity;
    }

    void OnDrawGizmos() {
        //Check if there has been a hit yet
        if (_hit) {
            Gizmos.color = Color.green;
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(groundCheckPoint, -transform.up * _hitinfo.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + groundCheckPoint + -transform.up * _hitinfo.distance, _boxSize);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else {
            Gizmos.color = Color.red;
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(groundCheckPoint, -transform.up * _hitinfo.distance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + groundCheckPoint + -transform.up * _hitinfo.distance, _boxSize);
        }
    }
}
