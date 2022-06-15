using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSword : MonoBehaviour {

    [SerializeField] private float _knockBackForce = 5f;
    [SerializeField] private float _stunDuration;
    private AudioSource _audioComponent;
    [SerializeField] private HitAudio[] _audios;
    private Dictionary<int, HitAudio> _audioDictionary = new Dictionary<int, HitAudio>();
    private float[] _soundCurrentCooldowns = new float[2];
    private HitAudio.soundTypes _currentSoundPlaying;

    // If the collision is not in this "Swing"'s already hit list,
    // Enemy: deals damage
    // Object: destroy it (if "Breakable")
    private void Awake() {
        _audioComponent = GetComponent<AudioSource>();
        //foreach (HitAudio audio in _audios) _audioDictionary.Add((int)audio.soundType, audio);
    }
    private void OnTriggerEnter(Collider other) {
        if (!PlayerTools.instance.swordCollisions.Contains(other)) {
            switch (other.tag) {
                case "Enemy":
                    other.GetComponent<EnemyData>().TakeDamage(PlayerTools.instance.swordDamage, _knockBackForce, _stunDuration);
                    PlaySoundEffect(HitAudio.soundTypes.HitEnemy, 0);
                    break;
                case "Breakable":
                    other.GetComponent<BreakableObjects>().DestroyObject();
                    PlaySoundEffect(HitAudio.soundTypes.HitObject, 0);
                    break;
            }
            PlayerTools.instance.swordCollisions.Add(other);
        }
    }

    public void PlaySoundEffect(HitAudio.soundTypes sound, float soundDelay) {
        if (_audioComponent.isPlaying && !_audioDictionary[(int)_currentSoundPlaying]._audioData.canLoop) return;
        if (_soundCurrentCooldowns[(int)sound] <= 0) {
            //Debug.Log(sound.ToString());
            //AudioSetup(sound);
            _currentSoundPlaying = sound;
            //_audio.PlayDelayed(soundDelay);
        }
    }
    private void AudioSetup(HitAudio.soundTypes sound) {
        _audioComponent.clip = _audioDictionary[(int)sound]._audioData.audioClips[Random.Range(0, _audioDictionary[(int)sound]._audioData.audioClips.Length)];
        _audioComponent.outputAudioMixerGroup = _audioDictionary[(int)sound]._audioData.audioMixerGroup;
        _audioComponent.volume = _audioDictionary[(int)sound]._audioData.volume;
        _audioComponent.loop = _audioDictionary[(int)sound]._audioData.canLoop;
        _soundCurrentCooldowns[(int)sound] = _audioDictionary[(int)sound]._audioData.soundInterval;
        if (_audioDictionary[(int)sound]._audioData.randomPitch) _audioComponent.pitch = Random.Range(-_audioDictionary[(int)sound]._audioData.randomPicthRange, _audioDictionary[(int)sound]._audioData.randomPicthRange);
        else _audioComponent.pitch = _audioDictionary[(int)sound]._audioData.pitch;
    }

    [System.Serializable]
    public class HitAudio {
        public enum soundTypes {
            HitEnemy,
            HitObject
        };
        public soundTypes soundType;
        public AudioData _audioData;
    }
}
