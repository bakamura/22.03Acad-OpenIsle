using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour {

    private CharacterController _charControler;

    [SerializeField] private float _movementSpeed = 5f;
    [Tooltip("How Fast the player rotates to face foward")]
    [SerializeField] private float _turnSmoothTime = .1f;
    private float _turnSmoothVelc;

    [SerializeField] private KeyCode _confineCursorKey;
    private bool _isConfined = false; //

    private void Awake() {
        _charControler = GetComponent<CharacterController>();
    }

    void Update() {
        float horAxis = Input.GetAxisRaw("Horizontal");
        float verAxis = Input.GetAxisRaw("Vertical");
        if (horAxis != 0 || verAxis != 0) {
            Vector3 lookDirection = new Vector3(horAxis, 0, verAxis).normalized;
            float targetLookAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float smoothLookAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref _turnSmoothVelc, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, smoothLookAngle, 0);
            Vector3 moveDirection = Quaternion.Euler(0, targetLookAngle, 0) * Vector3.forward;
            _charControler.Move(moveDirection.normalized * _movementSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            if (_isConfined) _isConfined = false;
            else _isConfined = true;
            Cursor.lockState = _isConfined ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}
