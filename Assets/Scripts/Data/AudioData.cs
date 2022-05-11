using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[Serializable]
public class AudioData
{
    public AudioClip[] audioClips;
    [Range(0f, 1f)]public float volume;
    public bool canLoop;
    public bool randomPitch;
    [Range(-3f, 3f)] public float pitch;
    public float soundInterval;
}

//[CustomEditor(typeof (AudioData))]
//public class AudioDataEditor : Editor {
//    public override void OnInspectorGUI() {
//        AudioData _class = new AudioData();
//        _class.randomPitch = GUILayout.Toggle(_class.randomPitch, "randomPicth");
//        if (_class.randomPitch) _class.pitch = EditorGUILayout.Slider(_class.pitch, -3f, 3f);        
//    }
//}

