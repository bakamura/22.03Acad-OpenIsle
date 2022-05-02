using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    [Header("Components")]

    private Collider _hitDetectionBox;
    private AudioSource _audio;
    private Animator _animator;

    [Header("Status")]

    [SerializeField] private float _damage;

    private void Awake() {
        _hitDetectionBox = GetComponent<Collider>();
        _audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        if (PlayerInputs.swordKeyPressed > 0 && PlayerData.Instance.hasSword) _animator.SetBool("ATTACKING", true);
    }

    private void OnCollisionEnter(Collision collision) {
        switch (collision.gameObject.tag) {
            case "Enemy":
                collision.gameObject.GetComponent<EnemyData>().TakeDamage(_damage);
                break;
            case "Breakable":
                //case used to the vines in the plateform area and the breakable doors in puzzle area
                break;
        }
    }

    public void ActivateSword() {
        //_audio.Play();
        _hitDetectionBox.enabled = true;
    }

    public void DeactivateSword() {
        _hitDetectionBox.enabled = false;
        _animator.SetBool("ATTACKING", false);
    }

}
