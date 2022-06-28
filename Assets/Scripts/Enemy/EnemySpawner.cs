using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] private EnemyWave[] _waves;
    //[SerializeField] private Vector3[] _spawnPoints;//standard spawn points
    [SerializeField] private GameObject[] _doorsToOpen;
    [SerializeField] private UnityEvent _OnEndWavesEvents;
#if UNITY_EDITOR
    [SerializeField, Min(0)] private int _currentWaveDebug; //used to see the spawn points of the wave index
    //[SerializeField] private bool _seeStandardSpawnPoints;
    [SerializeField] private bool _seeWaveSpawnPoints;
#endif
    private int _currentWave;
    private int _currentSpawnedEnemies;
    private int _defeatedEnemiesInCurrentWave;//counts how many enemies died in the current wave, used to start a new wave if possible
    private Coroutine _spawnEnemyesCoroutine = null;
    private List<EnemyInfo> _enemiesSpanwed = new List<EnemyInfo>();//stores the enemies spawned to pe reused if possible
    private AudioSource _audioSource;
    private bool _isCompleted;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        //if the player enter and the doors are still closed the waves will start
        if (other.CompareTag("Player") && _spawnEnemyesCoroutine == null && !_isCompleted) {
            foreach (GameObject doors in _doorsToOpen) doors.SetActive(true);//closes all doors
            _spawnEnemyesCoroutine = StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies() {
        while (_currentSpawnedEnemies < _waves[_currentWave].enemies.Length) {
            GameObject enemy = _waves[_currentWave].enemies[_currentSpawnedEnemies];
            EnemyInfo lookingForEnemy = _enemiesSpanwed.Find(obj => obj.type == enemy.GetComponent<EnemyBehaviour>()._enemyType && obj.isFlying == enemy.GetComponent<EnemyMovment>()._isFlying && !obj.enemy.activeInHierarchy);//new EnemyInfo { enemy = null };
            if (lookingForEnemy.enemy != null) {//enemy found and is not being used so it will be reused
                //Debug.Log("pooling");
                if (_waves[_currentWave].selectRandomSpawnPoint) lookingForEnemy.enemy.transform.position = transform.position + _waves[_currentWave].spawnPoint[Random.Range(0, _waves[_currentWave].spawnPoint.Length)];
                else lookingForEnemy.enemy.transform.position = transform.position + _waves[_currentWave].spawnPoint[_currentSpawnedEnemies];
                lookingForEnemy.enemy.GetComponent<EnemyData>().Activate(true);
            }
            else CreateEnemy();//enemy not found so i will be created
            _currentSpawnedEnemies++;
            yield return new WaitForSeconds(_waves[_currentWave].spawnInterval);
        }
        _spawnEnemyesCoroutine = null;
    }

    private void CreateEnemy() {
        GameObject gobj = Instantiate(_waves[_currentWave].enemies[_currentSpawnedEnemies],
                    _waves[_currentWave].selectRandomSpawnPoint ?
                    transform.position + _waves[_currentWave].spawnPoint[Random.Range(0, _waves[_currentWave].spawnPoint.Length)] :
                    transform.position + _waves[_currentWave].spawnPoint[_currentSpawnedEnemies], Quaternion.identity);
        gobj.GetComponent<EnemyData>().OnEnemyDefeat += StartNewWave;//on death checks if this spawner can proceed to the next wave
        _enemiesSpanwed.Add(new EnemyInfo { type = gobj.GetComponent<EnemyBehaviour>()._enemyType, enemy = gobj, isFlying = gobj.GetComponent<EnemyMovment>()._isFlying });
        //Debug.Log("create");
    }

    public void StartNewWave() {
        _defeatedEnemiesInCurrentWave++;
        if (_spawnEnemyesCoroutine == null && _currentWave < _waves.Length && _defeatedEnemiesInCurrentWave == _waves[_currentWave].enemies.Length) {//checks if all enemies in current wave are dead
            _defeatedEnemiesInCurrentWave = 0;
            _currentSpawnedEnemies = 0;
            _currentWave++;
            if (_currentWave == _waves.Length) {//if this is the last wave end the spawner
                //Debug.Log("endChalenge");
                foreach (GameObject doors in _doorsToOpen) doors.SetActive(false);
                _OnEndWavesEvents.Invoke();
                _isCompleted = true;
                //_audioSource.Play();
            }
            else {//proceed to the next wave
                //Debug.Log("new Wave");
                _spawnEnemyesCoroutine = StartCoroutine(SpawnEnemies());
            }
        }
    }

    private void OnDrawGizmosSelected() {
        if (_seeWaveSpawnPoints) {
            if (_waves[_currentWaveDebug].spawnPoint.Length > 0 && _currentWaveDebug < _waves.Length) {
                Gizmos.color = Color.blue;
                foreach (Vector3 point in _waves[_currentWaveDebug].spawnPoint) Gizmos.DrawSphere(transform.position + point, .3f);
            }
        }
    }
}
[System.Serializable]
public class EnemyWave {
    public GameObject[] enemies;
    public Vector3[] spawnPoint;
    public float spawnInterval;
    public bool selectRandomSpawnPoint;
}
public struct EnemyInfo {
    public EnemyBehaviour.EnemyTypes type;
    public bool isFlying;
    public GameObject enemy;
}
