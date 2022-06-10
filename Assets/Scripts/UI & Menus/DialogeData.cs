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