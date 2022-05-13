using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAsync : MonoBehaviour {

    [SerializeField] private float distanceCheck = 100;
    private AsyncOperation _asyncOperation = null;
    private bool _isSceneLoaded = false;
    [SerializeField] private string SceneToLoad;

    // Evitar uso desnecessario de colliders
    //private void OnTriggerEnter(Collider other) {
    //    if (other.tag == "Player") _asyncOperation = SceneManager.LoadSceneAsync(SceneToLoad, LoadSceneMode.Additive);
    //}
    //
    //private void OnTriggerExit(Collider other) {
    //    if (other.tag == "Player") SceneManager.UnloadSceneAsync(SceneToLoad);
    //}

    void Update() {
        float distance = Vector3.Distance(transform.position, PlayerData.Instance.transform.position);
        if (distance < distanceCheck && _asyncOperation == null && !_isSceneLoaded) _asyncOperation = SceneManager.LoadSceneAsync(SceneToLoad, LoadSceneMode.Additive);        
        else if (distance > distanceCheck && _asyncOperation != null && _isSceneLoaded) {
            SceneManager.UnloadSceneAsync(SceneToLoad);
            _isSceneLoaded = false;
        }
        

        if (_asyncOperation != null) {
            //LoadingSimbol.instance.FillImage(_asyncOperation.progress+0.1f);
            if (_asyncOperation.isDone) {
                //LoadingSimbol.instance.FillImage(0);
                _isSceneLoaded = true;
                _asyncOperation = null;
            }
        }
    }
}
