using UnityEngine;

public class DialogeData : MonoBehaviour
{
    [SerializeField] private DialogeContent[] _dialoges;
    private int _currentDialoge;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) StartDialoge();
    }

    public void StartDialoge() {
        if(_currentDialoge < _dialoges.Length)if(DialogManager.Instance.StartDialoge(_dialoges[_currentDialoge].dialogeInformatation)) _currentDialoge++;
    }
}
//[System.Serializable]
//public class DialogeCharCheck {
//    public int dialogeIndex;
//    [Min(0)] public int startChar;
//    [Min(0)] public int endChar;
//}
//[System.Serializable]
//public class DialogeContent {
//    public string characterName;
//    public string dialoge;
//}
//[System.Serializable]
//public class DialogeCustomInformation {
//    public DialogeCharCheck charInterval;
//    public TMP_FontAsset font;
//    public uint fontSize;
//    public float time;
//    //public bool underline;
//    //public bool bold;
//    //public bool italic;
//    public bool wobble;
//    public bool wave;
//    public Color color;
//}
