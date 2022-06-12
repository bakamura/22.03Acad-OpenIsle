using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "dialogeContent", menuName = "DialogeContent")]
public class DialogeContent : ScriptableObject
{
    public static char startAndEndEditChar = '*';
    [Tooltip("Use the '*' To start and end an editting piece")]public DialogeFormatationInfo[] dialogeInformatation;
}
[System.Serializable]
public class DialogeFormatationInfo {
    public string characterName;
    [TextArea(1, 5)] public string dialoge;
    public TMP_FontAsset[] font;
    [Min(0), Tooltip("Set to 0 if dosent want to change")] public float[] fontSize;
    [Min(0), Tooltip("Set to 0 if dosent want to change")] public float[] writeInterval;
    public bool[] bold;
    public bool[] italic;
    [Tooltip("Set Alpha Channel to 0 if dosent want to change")] public Color[] color;
    public DialogeAnimationInfo[] animationDetails;
    //public enum AnimationTypes {        
    //    Wobble,
    //    Wave,
    //    Shake
    //};
    //public AnimationTypes[] animationType;
    //[Min(0), Tooltip("Set to 0 if dosent want to animate")] public float[] animationIntensity;
}
[System.Serializable]
public class DialogeAnimationInfo {
    public enum AnimationTypes {
        Wobble,
        Wave,
        Shake
    };
    public AnimationTypes animationType;
    [Min(0), Tooltip("Set to 0 if dosent want to animate")] public float animationIntensity;
}
public class ChangedFormatation {
    public bool fontChanged;
    public bool fontSizeChanged;
    public bool boldChanged;
    public bool italicChanged;
    public bool colorChanged;

    public void ResetValues() {
        fontChanged = false;
        fontSizeChanged = false;
        boldChanged = false;
        italicChanged = false;
        colorChanged = false;
    }
}