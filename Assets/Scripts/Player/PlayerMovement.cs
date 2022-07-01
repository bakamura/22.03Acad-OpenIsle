using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement Instance { get; private set; }

    [Header("Movement")]

    public float _movementAcceleration = 5f; // Changed due to cheats
    [Tooltip("How Fast the object rotates to input")]
    [SerializeField] private float _turnSmoothTime = .1f;
    private float _turnSmoothVelc; // Used by SmoothDampAngle because a static function can't store states.
    [HideInInspector] public bool movementLock = false;

    [Header("Jump")]

    [SerializeField] private float _jumpStrengh = 100f;
    [SerializeField] private Vector3 groundCheckPoint;
    [SerializeField] private Vector3 _boxSize = Vector3.one;
    [SerializeField] private LayerMask groundLayer;
    [HideInInspector] public bool isGrounded;


    [Header("Dash")]

    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashInteractionDuration = 0.5f; // For a certain period of time after dashing, colliding with some entities will result in diferent interaction
    private bool _isDashing = false;
    [SerializeField] private float _dashInernalCooldown;
    private float _dashCurrentCooldown = 0;
    private PlayerAnim _animScript;

    private void Awake() {
        if (Instance == null) { 
        Instance = this;
            _animScript = GetComponent<PlayerAnim>();
        } 
        else if (Instance != this) Destroy(gameObject);
    }

    void Update() {
        isGrounded = Physics.OverlapBox(transform.position + groundCheckPoint, _boxSize / 2, Quaternion.identity, groundLayer).Length > 0;
        _dashCurrentCooldown -= Time.deltaTime;//updates de dash cooldown

        if (!movementLock) {
            // Jump
            if (PlayerInputs.jumpKeyPressed > 0 && isGrounded && PlayerData.rb.useGravity) {
                PlayerInputs.jumpKeyPressed = 0;

                PlayerData.rb.velocity = new Vector3(PlayerData.rb.velocity.x, _jumpStrengh, PlayerData.rb.velocity.z);
            }

            // Dash
            if (PlayerInputs.dashKeyPressed > 0 && _dashCurrentCooldown <= 0 /*&& HorizontalMovementAndRotation().magnitude > 0*/) {
                PlayerInputs.dashKeyPressed = 0;
                _dashCurrentCooldown = _dashInernalCooldown;
                //if the player is doing movment inputs the dash will go to the direction of the movment, in other case will go to the current foward direction
                float targetLookAngle = new Vector2(PlayerInputs.horizontalAxis, PlayerInputs.verticalAxis).magnitude > 0 ?
                    (Mathf.Atan2(PlayerInputs.horizontalAxis, PlayerInputs.verticalAxis) * Mathf.Rad2Deg) + Camera.main.transform.eulerAngles.y :
                    Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, targetLookAngle, 0);
                PlayerData.rb.velocity += Quaternion.Euler(0, targetLookAngle, 0) * Vector3.forward * _dashSpeed;
                _isDashing = true;
                _animScript.Dash();
                Invoke(nameof(StopDash), _dashInteractionDuration);
            }
        }
    }

    private void FixedUpdate() {
        // Movement
        if (!movementLock) {
            Vector3 hMovement = HorizontalMovementAndRotation() * Time.fixedDeltaTime;
            //float expectedMag = (PlayerData.rb.velocity + hMovement).magnitude;
            PlayerData.rb.velocity += hMovement;
            _animScript.SpeedXZ(new Vector3(PlayerData.rb.velocity.x, 0, PlayerData.rb.velocity.z).magnitude);
        }
            _animScript.SpeedY(PlayerData.rb.velocity.y);
    }

    //updates the rotation and movment of the player
    private Vector3 HorizontalMovementAndRotation() {
        if (PlayerInputs.horizontalAxis != 0 || PlayerInputs.verticalAxis != 0) {
            float targetLookAngle = (Mathf.Atan2(PlayerInputs.horizontalAxis, PlayerInputs.verticalAxis) * Mathf.Rad2Deg) + Camera.main.transform.eulerAngles.y;
            // Set rotation
            if (!PlayerTools.instance.isAiming) transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref _turnSmoothVelc, _turnSmoothTime), 0);

            // Set Movement
            Vector3 moveDirection = (Quaternion.Euler(0, targetLookAngle, 0) * Vector3.forward).normalized;
            return moveDirection * _movementAcceleration;
        }
        //if the player is not moving aply a desaceleration to it, and its stronger if is on grond
        else return (isGrounded ? -4 : -0.125f) * new Vector3(PlayerData.rb.velocity.x, 0, PlayerData.rb.velocity.z).normalized;
    }

    private void StopDash() {
        _isDashing = false;
    }

    private void OnCollisionStay(Collision collision) {
        if (_isDashing) {
            switch (collision.transform.tag) {
                case "Bullet":
                    collision.rigidbody.velocity = new Vector3(PlayerData.rb.velocity.x, 0, PlayerData.rb.velocity.z).normalized * collision.rigidbody.velocity.magnitude;
                    break;
                case "MovableObject":
                    collision.transform.GetComponent<MovableBox>().MoveToPosition();
                    break;
            }
        }
    }

    // Converts a Vector2 into an angle (float)
    public static float GetAngle(float x, float y) {
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {
        if (isGrounded) Gizmos.color = Color.green;
        else Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + groundCheckPoint, _boxSize);
    }
#endif
}
