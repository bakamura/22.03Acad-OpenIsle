using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovment : MonoBehaviour {
    [Header("Components")]
    //[SerializeField] private LayerMask _obstacleLayer;
    [HideInInspector] public NavMeshAgent _navMeshAgent;
    private EnemyAnimAndVFX _visualData;
    private EnemyBehaviour _behaviourData;

    [Header("Info")]
    public float _movmentSpeed;
    public float _rotationSpeed;
    public float _detectionRange;// if go to player
    public float _viewAngle;// if go to player
    public Color _FOVcolor;
    public float _randomNavegationArea;// if wanders
    [Tooltip("the minimal distance it needs to be for a new point generation")] public float minDistanceFromWanderingPoint;// if wanders
    [Tooltip("the interval bettwen moving to a new point")] public float randomNavPointCooldown;// if wanders
    public bool _isFlying;
    public bool _canWander;
    [Tooltip("if player is inside the action area, this will follow player")] public bool _willGoTowardsPlayer;

    [HideInInspector] public bool _isMovmentLocked;
    private Vector3 _currentTarget;
    private bool _isWandering;
    private bool _isFollowingPlayer;
    private Coroutine _randomPoinCoroutine = null;
    private Vector3 _startPoint;
    private Mesh _FOVMesh;

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _visualData = GetComponent<EnemyAnimAndVFX>();
        _behaviourData = GetComponent<EnemyBehaviour>();
        _startPoint = transform.position;
        _currentTarget = transform.position;
        if (_navMeshAgent) {
            //_navMeshAgent.speed = _movmentSpeed;
            _navMeshAgent.stoppingDistance = minDistanceFromWanderingPoint;
            //_navMeshAgent.angularSpeed *= _rotationSpeed;
        }
    }

    private void Update() {
        MovmentLogic();
        if (!_isFlying && _navMeshAgent) GroundMovment();
        else FlyingMovment();
        if (!_isMovmentLocked) _visualData.MovmentAnim(1f);
        else _visualData.MovmentAnim(0f);
    }

    private void GroundMovment() {
        if (!_isMovmentLocked && _currentTarget != Vector3.zero) {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.destination = _currentTarget;
        }
        else _navMeshAgent.isStopped = true;
    }

    private void FlyingMovment() {
        if (!_isMovmentLocked && _currentTarget != Vector3.zero) {
            Vector3 _movmentDirection = _isFollowingPlayer ? ((_currentTarget + _behaviourData.pointAroundPlayer) - transform.position).normalized : (_currentTarget - transform.position).normalized;
            transform.position += _movmentSpeed * Time.deltaTime * _movmentDirection;
            SetRotation(_currentTarget);
        }
    }

    private void SetRotation(Vector3 target) {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target - transform.position), Time.deltaTime * _rotationSpeed);
    }

    private void MovmentLogic() {
        float distanceFromTarget;
        if (_willGoTowardsPlayer) {
            distanceFromTarget = Vector3.Distance(PlayerMovement.Instance.transform.position, transform.position);
            if (distanceFromTarget <= _detectionRange && Vector3.Angle(transform.forward, (PlayerMovement.Instance.transform.position - transform.position).normalized) <= _viewAngle / 2) {//move to player
                if (_randomPoinCoroutine != null) {
                    StopCoroutine(_randomPoinCoroutine);
                    _randomPoinCoroutine = null;
                    _isMovmentLocked = false;
                }
                if (_navMeshAgent) _navMeshAgent.stoppingDistance = _behaviourData._actionRange;
                _isFollowingPlayer = true;
                _isWandering = false;
                _currentTarget = PlayerMovement.Instance.transform.position;
            }
            else if (_canWander) {
                _isFollowingPlayer = false;
                distanceFromTarget = Vector3.Distance(_currentTarget, transform.position);
                CheckForNewRandomPoint(distanceFromTarget);
            }
            else {
                _currentTarget = Vector3.zero;
                _isFollowingPlayer = false;
            }
        }
        else if (_canWander) {
            _isFollowingPlayer = false;
            distanceFromTarget = Vector3.Distance(_currentTarget, transform.position);
            CheckForNewRandomPoint(distanceFromTarget);
        }
    }

    private void CheckForNewRandomPoint(float distanceFromTarget) {
        if (_randomPoinCoroutine == null) {
            if (_navMeshAgent) _navMeshAgent.stoppingDistance = minDistanceFromWanderingPoint;
            if (distanceFromTarget <= minDistanceFromWanderingPoint) _isWandering = false;
            if (!_isWandering) {
                _isWandering = true;
                _currentTarget = Vector3.zero;
                _randomPoinCoroutine = StartCoroutine(RandomPointCorotine());
            }
        }
    }

    IEnumerator RandomPointCorotine() {
        _isMovmentLocked = true;
        yield return new WaitForSeconds(randomNavPointCooldown);
        /*while (_currentTarget == Vector3.zero)*/
        GenerateRandomNavegationPoint();//if dosent want the raycast check, remove the while
        _isMovmentLocked = false;
        _randomPoinCoroutine = null;
    }

    private void GenerateRandomNavegationPoint() {
        _currentTarget = _startPoint + new Vector3(Random.Range(-_randomNavegationArea, _randomNavegationArea), _isFlying ? Random.Range(-_randomNavegationArea, _randomNavegationArea) : 0, Random.Range(-_randomNavegationArea, _randomNavegationArea));
        //if (Physics.Raycast(transform.position, (_currentTarget - transform.position).normalized, Vector3.Distance(transform.position, _currentTarget), _obstacleLayer)) _currentTarget = Vector3.zero;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        // detection area
        if (_willGoTowardsPlayer) {
            Gizmos.color = _FOVcolor;
            //Gizmos.DrawWireSphere(transform.position, _detectionRange);
            Gizmos.DrawMesh(_FOVMesh, transform.position, transform.rotation);            
        }
        // wandering area
        if (_canWander) {
            Gizmos.color = Color.green;
            if (UnityEditor.EditorApplication.isPlaying) Gizmos.DrawWireSphere(_startPoint, _randomNavegationArea);
            else Gizmos.DrawWireSphere(transform.position, _randomNavegationArea);
        }
        // current moving target point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_currentTarget, .5f);
    }

    private void OnValidate() {
        _FOVMesh = CreateConeMesh();
    }

    private Mesh CreateConeMesh() {
        Mesh pyramid = new Mesh();
        int numOfTriangles = 6;
        int numVertices = numOfTriangles * 3;
        float angle = _viewAngle / 2;
        //NOTE: for the mesh to be rendered propperly all triangles needs to be constructed clokwise. (put a clock in the start point an make the pointer go clokwise when creating the triangle)
        Vector3[] vertices = new Vector3[numVertices];
        int[] triagles = new int[numVertices];

        Vector3 Center = Vector3.zero;
        Vector3 Left = Quaternion.Euler(0, -angle, 0) * Vector3.forward * _detectionRange;
        Vector3 Right = Quaternion.Euler(0, angle, 0) * Vector3.forward * _detectionRange;

        Vector3 top = Quaternion.Euler(angle, 0, 0) * Vector3.forward * _detectionRange;
        Vector3 bottom = Quaternion.Euler(-angle, 0, 0) * Vector3.forward * _detectionRange;

        int currentVert = 0;
        //bottom left
        vertices[currentVert++] = bottom;//center, center
        vertices[currentVert++] = Center;//bottom, left
        vertices[currentVert++] = Left;//left, bottom

        //top left
        vertices[currentVert++] = Left;
        vertices[currentVert++] = Center;
        vertices[currentVert++] = top;

        //top right
        vertices[currentVert++] = Center;
        vertices[currentVert++] = Right;
        vertices[currentVert++] = top;

        //bottom right
        vertices[currentVert++] = Center;
        vertices[currentVert++] = bottom;
        vertices[currentVert++] = Right;

        //pyramid base
        vertices[currentVert++] = top;
        vertices[currentVert++] = Right;
        vertices[currentVert++] = Left;

        vertices[currentVert++] = bottom;
        vertices[currentVert++] = Left;
        vertices[currentVert++] = Right;

        for (int i = 0; i < numVertices; i++) triagles[i] = i;
        pyramid.vertices = vertices;
        pyramid.triangles = triagles;
        pyramid.RecalculateNormals();

        return pyramid;
    }
#endif
}
