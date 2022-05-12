using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimAndVFX : MonoBehaviour {
    private Animator _animator;
    private AudioSource _audio;
    [SerializeField] private EnemyAudio[] _audioDatas;
    private Dictionary<int, EnemyAudio> _audioDictionary = new Dictionary<int, EnemyAudio>();
    private float[] _soundCurrentCooldowns = new float[4];
    private EnemyAudio.soundTypes _currentSoundPlaying;

    private void Awake() {
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        foreach (EnemyAudio data in _audioDatas) _audioDictionary.Add((int)data.soundType, data);
    }

    private void Update() {
        for (int i = 0; i < _soundCurrentCooldowns.Length; i++) _soundCurrentCooldowns[i] -= Time.deltaTime;        
    }

    public void AttackAnim(float atkSpeed) {
        _animator.SetFloat("ATKSPEED", atkSpeed);
        _animator.SetTrigger("ATTACK");        
        //PlaySoundEffect(EnemyAudio.soundTypes.Attack, _animator.GetCurrentAnimatorStateInfo(0).length / 2f);
    }

    public void StunAnim() {
        _animator.SetTrigger("STUN");
        PlaySoundEffect(EnemyAudio.soundTypes.Stun, 0f);
    }

    public void EndStunAnim() {
        _animator.SetTrigger("ENDSTUN");
    }

    public void MovmentAnim(float isTargetInRange) {
        _animator.SetFloat("TARGET", isTargetInRange);
        if (isTargetInRange > 0) PlaySoundEffect(EnemyAudio.soundTypes.Walk, 0f);
    }

    public void PlaySoundEffect(EnemyAudio.soundTypes sound, float soundDelay) {
        if (_audio.isPlaying && !_audioDictionary[(int)_currentSoundPlaying]._audioData.canLoop) return;
        if (_soundCurrentCooldowns[(int)sound] <= 0) {
            //Debug.Log(sound.ToString());
            AudioSetup(sound);
            _currentSoundPlaying = sound;
            //_audio.PlayDelayed(soundDelay);
        }
    }
    private void AudioSetup(EnemyAudio.soundTypes sound) {
        //_audio.clip = _audioDictionary[(int)sound]._audioData.audioClips[Random.Range(0, _audioDictionary[(int)sound]._audioData.audioClips.Length)];
        //_audio.volume = _audioDictionary[(int)sound]._audioData.volume;
        //_audio.loop = _audioDictionary[(int)sound]._audioData.canLoop;
        _soundCurrentCooldowns[(int)sound] = _audioDictionary[(int)sound]._audioData.soundInterval;
        //if (_audioDictionary[(int)sound]._audioData.randomPitch) _audio.pitch = Random.Range(-3f, 3f);
        //else _audio.pitch = _audioDictionary[(int)sound]._audioData.pitch;
    }


    [System.Serializable]
    public class EnemyAudio {
        public enum soundTypes {
            Attack,
            Idle,
            Walk,
            Stun,
            Die
        };
        public soundTypes soundType;
        public AudioData _audioData;
    }
}
