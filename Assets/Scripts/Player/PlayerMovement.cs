using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement Instance { get; private set; }

    [Header("Movement")]

    [SerializeField] private float _movementSpeed = 5f;
    [Tooltip("How Fast the object rotates to input")]
    [SerializeField] private float _turnSmoothTime = .1f;
    private float _turnSmoothVelc; // Used by SmoothDampAngle because a static function can't store states.
    // [HideInInspector]
    public bool movementLock = false;
    public float _maxAirVelocity = 300f;

    [Header("Jump")]

    [SerializeField] private float _jumpStrengh = 100f;
    [SerializeField] private Vector3 groundCheckPoint;
    [SerializeField] private Vector3 _boxSize = Vector3.one;
    [SerializeField] private LayerMask groundLayer;
    private bool _isGrounded;


    [Header("Dash")]

    [SerializeField] private float _dashSpeed = 1000f;
    [SerializeField] private float _dashDuration = .3f;
    private Vector3 _facing;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Update() {
        if (!movementLock) {
            // Jump
            _isGrounded = Physics.OverlapBox(transform.position + groundCheckPoint, _boxSize / 2, Quaternion.identity, groundLayer).Length > 0;

            //Debug.Log((PlayerInputs.jumpKeyPressed > 0 ? "TryingJump" : "NotJumping") + "\n" + (_isGrounded? "Is Grounded" : "Not Grounded")); // Dash memo working, jump not WEIRD BEHAVIOUR
            if (PlayerInputs.jumpKeyPressed > 0 && _isGrounded) {
                PlayerInputs.jumpKeyPressed = 0;

                PlayerData.rb.AddForce(transform.up * _jumpStrengh, ForceMode.Acceleration); // Check forcemode
            }

            // Dash
            if (PlayerInputs.dashKeyPressed > 0) {
                PlayerInputs.dashKeyPressed = 0;

                //PlayerData.rb.AddForce(GetMovementAndSetRotation(new Vector3(PlayerInputs.horizontalAxis, 0, PlayerInputs.verticalAxis)).normalized * _dashSpeed, ForceMode.Impulse); // Check forcemode
                movementLock = true;
                //PlayerData.rb.velocity = _facing * _dashSpeed;
                PlayerData.rb.velocity = new Vector3(_facing.x * _dashSpeed, VerticalMovment(), _facing.z * _dashSpeed);
                Invoke(nameof(EndDash), _dashDuration);
            }
        }
    }

    private void FixedUpdate() {
        // Movement
        if (!movementLock) PlayerData.rb.velocity = new Vector3(HorizontalMovement().x, VerticalMovment(), HorizontalMovement().z);
    }

    private Vector3 HorizontalMovement() {
        if (PlayerInputs.horizontalAxis != 0 || PlayerInputs.verticalAxis != 0) {
            Vector3 groundMovment = GetMovementAndSetRotation(new Vector3(PlayerInputs.horizontalAxis, 0, PlayerInputs.verticalAxis)).normalized;
            _facing = groundMovment;
            return groundMovment * _movementSpeed;
        }
        else return Vector3.zero;
    }

    private float VerticalMovment() {
        return Mathf.Clamp(PlayerData.rb.velocity.y, -_maxAirVelocity, _maxAirVelocity);
    }

    private Vector3 GetMovementAndSetRotation(Vector3 direction) {
        float targetLookAngle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg) + Camera.main.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref _turnSmoothVelc, _turnSmoothTime), 0);
        Vector3 moveDirection = Quaternion.Euler(0, targetLookAngle, 0) * Vector3.forward;
        return moveDirection;
    }

    void EndDash() {
        movementLock = false;
        PlayerData.rb.velocity = Vector3.zero;
    }

    void OnDrawGizmos() {
        if (_isGrounded) Gizmos.color = Color.green;
        else Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + groundCheckPoint, _boxSize);
    }
}
