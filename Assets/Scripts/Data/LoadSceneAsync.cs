using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAsync : MonoBehaviour {

    [SerializeField] private float distanceCheck = 100;
    private AsyncOperation _asyncOperation = null;
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
        if(Vector3.Distance(transform.position, PlayerData.Instance.transform.position) < distanceCheck) _asyncOperation = SceneManager.LoadSceneAsync(SceneToLoad, LoadSceneMode.Additive);
        else SceneManager.UnloadSceneAsync(SceneToLoad);

        if (_asyncOperation != null) {
            //LoadingSimbol.instance.FillImage(_asyncOperation.progress+0.1f);
            if (_asyncOperation.isDone) {
                //LoadingSimbol.instance.FillImage(0);
                _asyncOperation = null;
            }
        }
    }
}
