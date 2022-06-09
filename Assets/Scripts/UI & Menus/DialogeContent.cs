using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "dialogeContent", menuName = "DialogeContent")]
public class DialogeContent : ScriptableObject
{
    public static char startAndEndEditChar = '*';
    //public DialogeText[] dialogeText;
    [Tooltip("Use the '*' To start and end an editting piece")]public DialogeCustomInformation[] dialogeInformatation;
}
//[System.Serializable]
//public class DialogeText {
//    public string characterName;
//    [TextArea(1, 5)] public string dialoge;
//}
[System.Serializable]
public class DialogeCustomInformation {
    public string characterName;
    [TextArea(1, 5)] public string dialoge;
    public TMP_FontAsset[] font;
    [Min(0)]public float[] fontSize;
    public float[] writeInterval;
    public FontStyles[] fontStyle;
    //public bool bold;
    //public bool italic;
    public enum AnimationTypes {
        Wobble,
        Wave,
        Shake
    };
    public float[] animationIntensity;
    public AnimationTypes[] animationType;
    public Color[] color;
}
