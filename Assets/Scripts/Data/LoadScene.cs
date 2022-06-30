using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

    [SerializeField] private string SceneToLoad;
    [SerializeField] private LoadTypes _loadType;
    [SerializeField, Tooltip("when entering a new scene, changes the position of the player to this")] private Vector3 _newPlayerPosition;
    private AsyncOperation _asyncOperation = null;
    private bool _isSceneLoaded = false;
    private enum LoadTypes {
        Async,
        ChangeCompletely
    };
    
    [Header("LockedDoor")]

    [SerializeField] private PuzzleLightReceptor lightReceptor;
    private bool _isLocked = false;

    private void Start() {
        _isLocked = lightReceptor != null;
        if (_isLocked) lightReceptor.onActivate += OpenDoor;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !_isLocked){
            switch (_loadType) {
                case LoadTypes.Async:
                    //will only load if ist not currently loading and if not already loaded
                    if (_asyncOperation == null && !_isSceneLoaded) {
                        _asyncOperation = SceneManager.LoadSceneAsync(SceneToLoad, LoadSceneMode.Additive);
                        _isSceneLoaded = true;
                    }
                    break;
                case LoadTypes.ChangeCompletely:
                    SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Single);
                    other.transform.position = _newPlayerPosition;
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && _isSceneLoaded && _loadType == LoadTypes.Async) {
            SceneManager.UnloadSceneAsync(SceneToLoad);
            _isSceneLoaded = false;
        }
    }
    private void OpenDoor() {
        _isLocked = false;
    }
}
