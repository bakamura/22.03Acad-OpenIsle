using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator _animator;
    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    public void Dash() {
        _animator.SetTrigger("Dash");
    }
    public void Sword() {
        _animator.SetTrigger("Sword");
    }
    public void Amulet() {
        _animator.SetTrigger("Amulet");
    }
    public void Damaged() {
        _animator.SetTrigger("Damaged");
    }
    public void SpeedXZ(float velocity) {
        _animator.SetFloat("SpeedXZ",velocity);
    }
    public void SpeedY(float velocity) {
        _animator.SetFloat("SpeedXZ", velocity);
    }
    public void Hook(bool isHooked) {
        _animator.SetBool("Hook", isHooked);
    }
    public void Laddered(bool isLaddered) {
        _animator.SetBool("Laddered", isLaddered);
    }


}
