using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    private CharacterController CC;
    private Transform cameraTransform;
    [SerializeField] private float speed = 5f;
    [Tooltip("How Fast the player rotates to face foward")]
    [SerializeField] private float turnSmoothTime = .1f;
    private float turnSmoothVelc; 
    private void Awake()
    {
        CC = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        float horz = Input.GetAxisRaw("Horizontal");
        float vertc = Input.GetAxisRaw("Vertical");
        if (horz != 0 || vertc != 0)
        {
            Vector3 lookDirection = new Vector3(horz, 0, vertc).normalized;
            float targetLookAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float smoothLookAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref turnSmoothVelc, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, smoothLookAngle, 0);
            Vector3 moveDirection = Quaternion.Euler(0, targetLookAngle, 0) * Vector3.forward;
            CC.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
    }
}
