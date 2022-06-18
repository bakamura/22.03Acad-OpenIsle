using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] private EnemyWave[] _waves;
    [SerializeField] private Vector3[] _spawnPoints;
    [SerializeField] private GameObject[] _doorsToOpen;
#if UNITY_EDITOR
    [SerializeField, Min(0)] private int _currentWaveDebug;
    [SerializeField] private bool _seeStandardSpawnPoints;
    [SerializeField] private bool _seeWaveFixedSpawnPoints;
#endif
    private int _currentWave;
    private int _currentSpawnedEnemies;
    private int _defeatedEnemiesInCurrentWave;
    private Coroutine _spawnEnemyesCoroutine = null;
    private List<EnemyInfo> _enemiesSpanwed = new List<EnemyInfo>();
    private AudioSource _audioSource;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        //if the player enter and the doors are still closed the waves will start
        if (other.CompareTag("Player") && _spawnEnemyesCoroutine == null && _doorsToOpen[0].activeInHierarchy) _spawnEnemyesCoroutine = StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies() {
        while (_currentSpawnedEnemies < _waves[_currentWave].enemies.Length) {
            GameObject enemy = _waves[_currentWave].enemies[_currentSpawnedEnemies];
            EnemyInfo lookingForEnemy = new EnemyInfo { enemy = null };
            foreach (EnemyInfo info in _enemiesSpanwed) {
                if (info.type == enemy.GetComponent<EnemyBehaviour>()._enemyType && info.isFlying == enemy.GetComponent<EnemyMovment>()._isFlying && !info.enemy.activeInHierarchy) {
                    lookingForEnemy = info;
                    break;
                }
            }
            //EnemyInfo obj = _enemiesSpanwed.Find(a => a.type == enemy.GetComponent<EnemyBehaviour>()._enemyType && a.isFlying == enemy.GetComponent<EnemyMovment>()._isFlying);//looks for this type of enemy in list
            if (lookingForEnemy.enemy != null) {//enemy found and is not being used so it will be reused
                //Debug.Log("pooling");
                if (_waves[_currentWave].selectRandomSpawnPoint) lookingForEnemy.enemy.transform.position = transform.position + _spawnPoints[Random.Range(0, _spawnPoints.Length)];
                else lookingForEnemy.enemy.transform.position = transform.position + _waves[_currentWave].spawnPoint[_currentSpawnedEnemies];
                lookingForEnemy.enemy.GetComponent<EnemyData>().Activate(true);
            }
            else CreateEnemy();
            _currentSpawnedEnemies++;
            yield return new WaitForSeconds(_waves[_currentWave].spawnInterval);
        }
        _spawnEnemyesCoroutine = null;
    }

    private void CreateEnemy() {
        GameObject gobj = Instantiate(_waves[_currentWave].enemies[_currentSpawnedEnemies],
                    _waves[_currentWave].selectRandomSpawnPoint ? transform.position + _spawnPoints[Random.Range(0, _spawnPoints.Length)] : transform.position + _waves[_currentWave].spawnPoint[_currentSpawnedEnemies], Quaternion.identity);
        gobj.GetComponent<EnemyData>().OnEnemyDefeat += StartNewWave;
        _enemiesSpanwed.Add(new EnemyInfo { type = gobj.GetComponent<EnemyBehaviour>()._enemyType, enemy = gobj, isFlying = gobj.GetComponent<EnemyMovment>()._isFlying });
        //Debug.Log("create");
    }

    public void StartNewWave() {
        _defeatedEnemiesInCurrentWave++;
        if (_spawnEnemyesCoroutine == null && _currentWave < _waves.Length && _defeatedEnemiesInCurrentWave == _waves[_currentWave].enemies.Length) {            
            _defeatedEnemiesInCurrentWave = 0;
            _currentSpawnedEnemies = 0;
            _currentWave++;
            if (_currentWave == _waves.Length) {
                //Debug.Log("endChalenge");
                foreach (GameObject doors in _doorsToOpen) doors.SetActive(false);
                //_audioSource.Play();
            }
            else {
                //Debug.Log("new Wave");
                _spawnEnemyesCoroutine = StartCoroutine(SpawnEnemies());
            }
        }
    }

    private void OnDrawGizmosSelected() {
        if (_seeStandardSpawnPoints) {
            if (_spawnPoints.Length > 0) {
                Gizmos.color = Color.red;
                foreach (Vector3 point in _spawnPoints) Gizmos.DrawSphere(transform.position + point, .3f);
            }
        }
        if (_seeWaveFixedSpawnPoints) {
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
    /*    [Tooltip("if this is true use the spawnPoint array inside the waves, else will use the general spawnPointarray")]*/
    public bool selectRandomSpawnPoint;
}
public struct EnemyInfo {
    public EnemyBehaviour.EnemyTypes type;
    public bool isFlying;
    public GameObject enemy;
}
