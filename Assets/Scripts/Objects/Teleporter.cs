using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Vector3 _pointToTeleport;
    //[SerializeField, Range(0,360)] private float _lookAngle;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) other.transform.SetPositionAndRotation(transform.position + _pointToTeleport, Quaternion.identity/*Quaternion.Euler(other.transform.rotation.x, _lookAngle, other.transform.rotation.z)*/);        
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + _pointToTeleport, .3f);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position + _pointToTeleport,  new Vector3(Mathf.Sin(_lookAngle), 0, Mathf.Cos(_lookAngle)));
    }
}
