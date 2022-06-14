using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpring : MonoBehaviour {

    [SerializeField] private float _strengh;

    // If collides with the Player, an is under it, Adds velocity towards this.Transform.forward to the Player
    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Player" && PlayerData.rb.transform.position.y - (PlayerData.rb.transform.lossyScale.y / 2f) > transform.position.y + (transform.lossyScale.y / 2f) - 0.05f) collision.transform.GetComponent<Rigidbody>().AddForce(transform.up * _strengh, ForceMode.Acceleration);
    }
}
