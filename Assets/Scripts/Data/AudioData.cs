using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

[Serializable]
public class AudioData {
    public AudioClip[] audioClips;
    public AudioMixerGroup audioMixerGroup;
    [Range(0f, 1f)] public float volume;
    public bool canLoop;
    [Range(-3f, 3f)] public float pitch;
    [Tooltip("if this is active use the randomPicthRange")]public bool randomPitch;
    [Range(0f, .5f)] public float randomPicthRange;
    public float soundInterval;
}

