using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour {

    [Header("Components")]
    private CharacterController _charControler;
    private PlayerData _playerData;

    [Header("Values")]
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _dashMaxDistance = 10f;
    [SerializeField] private float _dashSpeed = .5f;
    [Tooltip("How Fast the player rotates to face foward")]
    [SerializeField] private float _turnSmoothTime = .1f;
    private float _turnSmoothVelc;

    [SerializeField] private KeyCode _confineCursorKey;
    private bool _isConfined = false; //

    private Vector3 _dashFinalPosition;

    private void Awake() {
        _charControler = GetComponent<CharacterController>();
        _playerData = GetComponent<PlayerData>();
    }

    void Update() {
        Dash();
        Movment();
        if (Input.GetKeyDown(KeyCode.L)) {
            if (_isConfined) _isConfined = false;
            else _isConfined = true;
            Cursor.lockState = _isConfined ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    private void Movment() {
        if (GetMovmentInputs().x != 0 || GetMovmentInputs().y != 0) {
            _charControler.Move(GetMovmenDirection(GetMovmentInputs()).normalized * _movementSpeed * Time.deltaTime);
        }
    }

    private Vector2 GetMovmentInputs() {
        float horAxis = Input.GetAxisRaw("Horizontal");
        float verAxis = Input.GetAxisRaw("Vertical");
        return new Vector2(horAxis, verAxis);
    }

    private Vector3 GetMovmenDirection(Vector2 direction) {
        Vector3 lookDirection = new Vector3(direction.x, 0, direction.y).normalized;
        float targetLookAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        float smoothLookAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref _turnSmoothVelc, _turnSmoothTime);
        transform.rotation = Quaternion.Euler(0, smoothLookAngle, 0);
        Vector3 moveDirection = Quaternion.Euler(0, targetLookAngle, 0) * Vector3.forward;
        return moveDirection;
    }

    private void Dash() {
        if ((GetMovmentInputs().x != 0 || GetMovmentInputs().y != 0) && Input.GetButtonDown("Dash")) {
            _dashFinalPosition = GetDashDistance();
            InvokeRepeating("DashMovment", 0f, _dashSpeed);
            //_playerData.rb.AddForce(new Vector3(horAxis, 0, verAxis).normalized * _dashDistance);
        }
    }
    
    private Vector3 GetDashDistance() {
        Vector3 finalPosition = new Vector3(transform.position.x + (Mathf.Sign(GetMovmenDirection(GetMovmentInputs()).x) * _dashMaxDistance), transform.position.y, transform.position.z + (Mathf.Sign(GetMovmenDirection(GetMovmentInputs()).y) * _dashMaxDistance));
        return finalPosition;
    }

    private void DashMovment() {
        transform.position += new Vector3(_dashMaxDistance * Mathf.Sign(_dashFinalPosition.x) * .05f, 0f, _dashMaxDistance * Mathf.Sign(_dashFinalPosition.z) * .05f);
        if (transform.position.x >= _dashFinalPosition.x || transform.position.z >= _dashFinalPosition.z) {
            CancelInvoke("DashMovment");
        }
    }
}
