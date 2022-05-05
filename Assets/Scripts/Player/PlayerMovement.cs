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
    [HideInInspector] public bool movementLock = false;
    [SerializeField] private float _speedCap; // Magnitude

    [Header("Jump")]

    [SerializeField] private float _jumpStrengh = 100f;
    [SerializeField] private Vector3 groundCheckPoint;
    [SerializeField] private Vector3 _boxSize = Vector3.one;
    [SerializeField] private LayerMask groundLayer;
    [HideInInspector] public bool isGrounded;


    [Header("Dash")]

    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDuration = .3f;
    [SerializeField] private float _dashInernalCooldown;
    private Vector3 _currentDashVelocity;
    private float _dashCurrentCooldown = 0;
    private Vector3 _facing;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Update() {
        if (!movementLock) {

            //Debug.Log((PlayerInputs.jumpKeyPressed > 0 ? "TryingJump" : "NotJumping") + "\n" + (_isGrounded? "Is Grounded" : "Not Grounded")); // Dash memo working, jump not WEIRD BEHAVIOUR
            if (PlayerInputs.jumpKeyPressed > 0 && isGrounded) {
                PlayerInputs.jumpKeyPressed = 0;

                PlayerData.rb.AddForce(transform.up * _jumpStrengh, ForceMode.Acceleration); // Check forcemode
            }

            // Dash
            if (PlayerInputs.dashKeyPressed > 0 && _dashCurrentCooldown <= 0 && HorizontalMovement().magnitude > 0) {
                PlayerInputs.dashKeyPressed = 0;
                _dashCurrentCooldown = _dashInernalCooldown;

                movementLock = true;
                //float targetLookAngle = new Vector2(PlayerInputs.horizontalAxis, PlayerInputs.verticalAxis).magnitude > 0 ?
                //    (Mathf.Atan2(PlayerInputs.horizontalAxis, PlayerInputs.verticalAxis) * Mathf.Rad2Deg) + Camera.main.transform.eulerAngles.y :
                //    Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
                //transform.rotation = Quaternion.Euler(0, targetLookAngle, 0);
                //PlayerData.rb.velocity += Quaternion.Euler(0, targetLookAngle, 0) * Vector3.forward * _dashSpeed;
                _currentDashVelocity = HorizontalMovement() / _movementSpeed * _dashSpeed;
                PlayerData.rb.velocity += _currentDashVelocity;
                Invoke(nameof(EndDash), _dashDuration);
            }
        }
        // GroundCheck
        isGrounded = Physics.OverlapBox(transform.position + groundCheckPoint, _boxSize / 2, Quaternion.identity, groundLayer).Length > 0;
        _dashCurrentCooldown = Mathf.Clamp(_dashCurrentCooldown - Time.deltaTime, 0, _dashInernalCooldown);
    }

    private void FixedUpdate() {
        // Movement
        if (!movementLock) {
            if (!isGrounded) {
                Vector3 hMovement = HorizontalMovement(); //* Time.fixedDeltaTime;
                float expectedMag = (PlayerData.rb.velocity + hMovement).magnitude;
                if (expectedMag < _speedCap || expectedMag < PlayerData.rb.velocity.magnitude) PlayerData.rb.velocity += hMovement;
                return;
            }
            PlayerData.rb.velocity = new Vector3(HorizontalMovement().x, PlayerData.rb.velocity.y, HorizontalMovement().z);
        }
    }

    private Vector3 HorizontalMovement() {
        if (PlayerInputs.horizontalAxis != 0 || PlayerInputs.verticalAxis != 0) {
            Vector3 groundMovment = GetMovementAndSetRotation(new Vector3(PlayerInputs.horizontalAxis, 0, PlayerInputs.verticalAxis)).normalized;
            return groundMovment * _movementSpeed;
        }
        else return Vector3.zero;
        //else return isGrounded ? -4 * PlayerData.rb.velocity : Vector3.zero;
    }

    private Vector3 GetMovementAndSetRotation(Vector3 direction) {
        float targetLookAngle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg) + Camera.main.transform.eulerAngles.y;
        if (!PlayerTools.instance.isAiming) transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref _turnSmoothVelc, _turnSmoothTime), 0);
        Vector3 moveDirection = Quaternion.Euler(0, targetLookAngle, 0) * Vector3.forward;
        return moveDirection;
    }

    void EndDash() {
        movementLock = false;
        PlayerData.rb.velocity -= _currentDashVelocity;
    }

    void OnDrawGizmos() {
        if (isGrounded) Gizmos.color = Color.green;
        else Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + groundCheckPoint, _boxSize);
    }
}
