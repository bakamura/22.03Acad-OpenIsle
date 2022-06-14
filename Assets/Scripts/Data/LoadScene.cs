using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

    //[SerializeField] private float distanceCheck = 100;
    [SerializeField] private string SceneToLoad;
    [SerializeField] private LoadTypes _loadType;
    private AsyncOperation _asyncOperation = null;
    private bool _isSceneLoaded = false;
    private enum LoadTypes {
        Async,
        ChangeCompletely
    };

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")){
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

    //void Update() {
    //    float distance = Vector3.Distance(transform.position, PlayerData.Instance.transform.position);
    //    if (distance < distanceCheck && _asyncOperation == null && !_isSceneLoaded) _asyncOperation = SceneManager.LoadSceneAsync(SceneToLoad, LoadSceneMode.Additive);        
    //    else if (distance > distanceCheck && _isSceneLoaded) {
    //        SceneManager.UnloadSceneAsync(SceneToLoad);
    //        _isSceneLoaded = false;
    //    }


    //    if (_asyncOperation != null) {
    //        //LoadingSimbol.instance.FillImage(_asyncOperation.progress+0.1f);
    //        if (_asyncOperation.isDone) {
    //            //LoadingSimbol.instance.FillImage(0);
    //            _asyncOperation = null;
    //            _isSceneLoaded = true;
    //        }
    //    }
    //}

    //private void OnDrawGizmosSelected() {
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, distanceCheck);
    //}
}
