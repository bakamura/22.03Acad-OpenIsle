using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement Instance { get; private set; }

    [Header("Movement")]

    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _maxAirVelocity = 300f;
    [Tooltip("How Fast the object rotates to input")]
    [SerializeField] private float _turnSmoothTime = .1f;
    private float _turnSmoothVelc; // Used by SmoothDampAngle because a static function can't store states.

    [Header("Jump")]

    [SerializeField] private float _jumpStrengh = 100f;
    [SerializeField] private Vector3 groundCheckPoint;
    [SerializeField] private Vector3 _boxSize = Vector3.one;
    [SerializeField] private LayerMask groundLayer;
    private bool _isGrounded;


    [Header("Dash")]

    [SerializeField] private float _dashSpeed = 1000f;
    [SerializeField] private float _dashDuration = .3f;
    private bool _isDashing = false;
    private Vector3 _dashDirection;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Update() {
        // Jump
        _isGrounded = Physics.OverlapBox(transform.position + groundCheckPoint, _boxSize / 2, Quaternion.identity, groundLayer).Length > 0; //

        if (PlayerInputs.jumpKeyPressed && _isGrounded) {
            PlayerInputs.jumpKeyPressed = false;
            if (_isGrounded) PlayerData.rb.AddForce(transform.up * _jumpStrengh, ForceMode.Force); // Check forcemode
        }

        // Dash
        if (PlayerInputs.dashKeyPressed) {
            PlayerInputs.dashKeyPressed = false;
            _isDashing = true;
            _dashDirection = GroundMovment() / _movementSpeed * _dashSpeed;
            Invoke("EndDash", _dashDuration);
        }
    }

    void EndDash() {
        _isDashing = false;
    }
    private void FixedUpdate() {
        //GroundCheck();
        float airVelocity = Mathf.Clamp(PlayerData.rb.velocity.y, -_maxAirVelocity, _maxAirVelocity);
        if (!_isDashing) {
            Vector3 TotalMovment = new Vector3(GroundMovment().x, airVelocity, GroundMovment().z); // GRAVITY TOO HEAVY
            PlayerData.rb.velocity = TotalMovment;
        }
        else PlayerData.rb.velocity = new Vector3(_dashDirection.x, airVelocity, _dashDirection.z);
    }

    private Vector3 GroundMovment() {
        if (PlayerInputs.horizontalAxis != 0 || PlayerInputs.verticalAxis != 0) {
            Vector3 groundMovment = GetMovmentDirection(new Vector3(PlayerInputs.horizontalAxis, 0, PlayerInputs.verticalAxis)).normalized;
            return groundMovment * _movementSpeed;
        }
        else return Vector3.zero;
    }

    private Vector3 GetMovmentDirection(Vector3 direction) {
        float targetLookAngle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg) + Camera.main.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref _turnSmoothVelc, _turnSmoothTime), 0);
        Vector3 moveDirection = Quaternion.Euler(0, targetLookAngle, 0) * Vector3.forward;
        return moveDirection;
    }

    //private void GroundCheck() {
    //    _hit = Physics.OverlapBox(transform.position + groundCheckPoint, _boxSize / 2, Quaternion.identity, groundLayer) != null;
    //    Collider[] collisions = Physics.OverlapBox(transform.position + groundCheckPoint, _boxSize / 2, Quaternion.identity, groundLayer);
    //    foreach (Collider col in collisions) Debug.Log(col.name);
    //     Physics.BoxCast(groundCheckPoint, _boxSize, -transform.up, out _hitinfo, Quaternion.identity, _distance, groundLayer);
    //    if (_hit) {
    //        _isGrounded = true;
    //    }
    //    else _isGrounded = false;
    //}

    void OnDrawGizmos() {
        if (_isGrounded) Gizmos.color = Color.green;
        else Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + groundCheckPoint, _boxSize);
    }
}
