using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAsync : MonoBehaviour
{
    private AsyncOperation _asyncOperation = null;
    [SerializeField] private string SceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") _asyncOperation = SceneManager.LoadSceneAsync(SceneToLoad, LoadSceneMode.Additive);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") SceneManager.UnloadSceneAsync(SceneToLoad);
    }
    void Update()
    {
        if (_asyncOperation != null)
        {
            //LoadingSimbol.instance.FillImage(_asyncOperation.progress+0.1f);
            if (_asyncOperation.isDone)
            {
                //LoadingSimbol.instance.FillImage(0);
                _asyncOperation = null;
            }
        }
    }
}
