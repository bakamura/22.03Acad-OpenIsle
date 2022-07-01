using UnityEngine;

public class PuzzleReseter : MonoBehaviour {

    [SerializeField] private PuzzleObject[] _objectPrefabs;
    private GameObject[] _instances;
    private bool _playerInArea;

    private void Start() {
        PlayerTools.onActivateAmulet += ResetPuzzle;
    }

    // Resets puzzle elements positions
    private void ResetPuzzle() {
        if (/*(PlayerData.Instance.transform.position - transform.position).magnitude < 2f*/_playerInArea) {
            //foreach (GameObject obj in _instances) Destroy(obj);

            for (int i = 0; i < _objectPrefabs.Length; i++) {
                _objectPrefabs[i].puzObject.transform.SetPositionAndRotation(_objectPrefabs[i].position, _objectPrefabs[i].rotation);
                _objectPrefabs[i].puzObject.transform.localScale = _objectPrefabs[i].scale;
                //_instances[i] = Instantiate(_objectPrefabs[i].puzObject, _objectPrefabs[i].position, _objectPrefabs[i].rotation);
                //_instances[i].transform.localScale = _objectPrefabs[i].scale;
                //if (_instances[i].GetComponent<PuzzleLightMirror>() != null && _objectPrefabs[i].variation == 1) _instances[i].GetComponent<PuzzleLightMirror>().ChangeRotation();
            }
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) _playerInArea = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) _playerInArea = false;
    }
}

[System.Serializable]
public class PuzzleObject {

    public GameObject puzObject;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public int variation;

}