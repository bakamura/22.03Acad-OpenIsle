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
    [SerializeField] private float _movmentSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _detectionRange;// if go to player
    [SerializeField] private float _viewAngle;// if go to player
    [SerializeField] private Color _FOVcolor = Color.red;
    [SerializeField] private float _randomNavegationArea;// if wanders
    [SerializeField] private float _minDistanceFromWanderingPoint;// if wanders
    [SerializeField] private float _randomNavPointCooldown;// if wanders
    public bool _isFlying;
    [SerializeField] private bool _canWander;
    [SerializeField] private bool _willGoTowardsPlayer;

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
            _navMeshAgent.stoppingDistance = _minDistanceFromWanderingPoint;
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

    //rotates the enemy towards its target
    private void SetRotation(Vector3 target) {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target - transform.position), Time.deltaTime * _rotationSpeed);
    }

    private void MovmentLogic() {
        float distanceFromTarget;
        if (_willGoTowardsPlayer) {//if the enemy will move and will follow the player
            distanceFromTarget = Vector3.Distance(PlayerMovement.Instance.transform.position, transform.position);
            if (distanceFromTarget <= _detectionRange && Vector3.Angle(transform.forward, (PlayerMovement.Instance.transform.position - transform.position).normalized) <= _viewAngle / 2) {//move to player
                if (_randomPoinCoroutine != null) {//stop the wandering coroutine
                    StopCoroutine(_randomPoinCoroutine);
                    _randomPoinCoroutine = null;
                    _isMovmentLocked = false;
                }
                if (_navMeshAgent) _navMeshAgent.stoppingDistance = _behaviourData._actionRange;
                _isFollowingPlayer = true;
                _isWandering = false;
                _currentTarget = PlayerMovement.Instance.transform.position;
            }
            else if (_canWander) {//if the enemy will move and didnt find the player, and can wander
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
            if (_navMeshAgent) _navMeshAgent.stoppingDistance = _minDistanceFromWanderingPoint;
            if (distanceFromTarget <= _minDistanceFromWanderingPoint) _isWandering = false;
            if (!_isWandering) {
                _isWandering = true;
                _currentTarget = Vector3.zero;
                _randomPoinCoroutine = StartCoroutine(RandomPointCorotine());
            }
        }
    }

    IEnumerator RandomPointCorotine() {
        _isMovmentLocked = true;
        yield return new WaitForSeconds(_randomNavPointCooldown);
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

    //creates the visual cone in the editor thar represents the FOV
    private Mesh CreateConeMesh() {
        Mesh pyramid = new Mesh();
        int numOfTriangles = 14;
        int numVertices = numOfTriangles * 3;
        float angle = _viewAngle / 2f;
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 Center = transform.forward;//0 

        Vector3 Left = Quaternion.Euler(0, -angle, 0) * transform.forward * _detectionRange;
        Vector3 Right = Quaternion.Euler(0, angle, 0) * transform.forward * _detectionRange;

        Vector3 top = Quaternion.Euler(-angle, 0, 0) * transform.forward * _detectionRange;
        Vector3 bottom = Quaternion.Euler(angle, 0, 0) * transform.forward * _detectionRange;

        float middlePointAngles = angle * .7f;

        Vector3 MiddleTopLeft = Quaternion.Euler(-middlePointAngles, -middlePointAngles, 0) * transform.forward * _detectionRange;
        Vector3 MiddleTopRight = Quaternion.Euler(-middlePointAngles, middlePointAngles, 0) * transform.forward * _detectionRange;

        Vector3 MiddleBottomLeft = Quaternion.Euler(middlePointAngles, -middlePointAngles, 0) * transform.forward * _detectionRange;
        Vector3 MiddleBottomRight = Quaternion.Euler(middlePointAngles, middlePointAngles, 0) * transform.forward * _detectionRange;

        int currentVert = 0;

        //top left
        vertices[currentVert++] = top;
        vertices[currentVert++] = Center;
        vertices[currentVert++] = MiddleTopLeft;
        
        //middle top left
        vertices[currentVert++] = MiddleTopLeft;
        vertices[currentVert++] = Center;
        vertices[currentVert++] = Left;
        
        //middle bottom left
        vertices[currentVert++] = Left;
        vertices[currentVert++] = Center;
        vertices[currentVert++] = MiddleBottomLeft;
        
        //bottom left
        vertices[currentVert++] = MiddleBottomLeft;
        vertices[currentVert++] = Center;
        vertices[currentVert++] = bottom;
        
        //bottom right
        vertices[currentVert++] = bottom;
        vertices[currentVert++] = Center;
        vertices[currentVert++] = MiddleBottomRight;

        //middle bottom right
        vertices[currentVert++] = MiddleBottomRight;
        vertices[currentVert++] = Center;
        vertices[currentVert++] = Right;

        //middle top right
        vertices[currentVert++] = Right;
        vertices[currentVert++] = Center;
        vertices[currentVert++] = MiddleTopRight;

        //top right
        vertices[currentVert++] = MiddleTopRight;
        vertices[currentVert++] = Center;
        vertices[currentVert++] = top;

        //pyramid base
        //top
        vertices[currentVert++] = top;
        vertices[currentVert++] = MiddleTopLeft;
        vertices[currentVert++] = MiddleTopRight;
        
        //top middle
        vertices[currentVert++] = MiddleTopRight;
        vertices[currentVert++] = MiddleTopLeft;
        vertices[currentVert++] = Right;
        
        vertices[currentVert++] = Right;
        vertices[currentVert++] = MiddleTopLeft;
        vertices[currentVert++] = Left;
        
        //bottom middle
        vertices[currentVert++] = Left;
        vertices[currentVert++] = MiddleBottomRight;
        vertices[currentVert++] = Right;
        
        vertices[currentVert++] = MiddleBottomRight;
        vertices[currentVert++] = Left;
        vertices[currentVert++] = MiddleBottomLeft;

        //bottom
        vertices[currentVert++] = MiddleBottomLeft;
        vertices[currentVert++] = bottom;
        vertices[currentVert++] = MiddleBottomRight;

        for (int i = 0; i < numVertices; i++) triangles[i] = i;
        pyramid.vertices = vertices;
        pyramid.triangles = triangles;
        pyramid.RecalculateNormals();

        return pyramid;
    }
#endif
}
