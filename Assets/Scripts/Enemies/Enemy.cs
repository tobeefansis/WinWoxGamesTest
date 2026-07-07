using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float waypointThreshold = 0.15f;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 6f;
    [SerializeField] private float returnDelay = 2f;

    public float Speed             => speed;
    public float WaypointThreshold => waypointThreshold;
    public float DetectionRadius   => detectionRadius;
    public float ReturnDelay       => returnDelay;

    public List<Vector3> Waypoints        { get; private set; }
    public int           CurrentWaypointIndex { get; set; }
    public Transform     Player           { get; private set; }

    private Room[,] _maze;
    private int     _mazeSize;
    private float   _cellSize;
    private float   _heightOffset;

    private IEnemyState      _current;
    private EnemyPatrolState _patrolState;
    private EnemyChaseState  _chaseState;
    private EnemyReturnState _returnState;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void Initialize(List<Vector3> waypoints, Room[,] maze, int mazeSize, float cellSize, float heightOffset)
    {
        Waypoints     = waypoints;
        _maze         = maze;
        _mazeSize     = mazeSize;
        _cellSize     = cellSize;
        _heightOffset = heightOffset;

        CurrentWaypointIndex = 0;

        var player = ServiceLocator.Instance.Get<PlayerController>();
        if (player != null)
            Player = player.transform;
        else
            Debug.LogWarning("[Enemy] PlayerController not found in ServiceLocator.");

        _patrolState = new EnemyPatrolState(this);
        _chaseState  = new EnemyChaseState(this);
        _returnState = new EnemyReturnState(this);

        TransitionTo(_patrolState);
    }

    private void Update() => _current?.Tick();

    public void TransitionToPatrol() => TransitionTo(_patrolState);
    public void TransitionToChase()  => TransitionTo(_chaseState);
    public void TransitionToReturn() => TransitionTo(_returnState);

    public void TransitionTo(IEnemyState next)
    {
     
        _current?.Exit();
        _current = next;
        _current?.Enter();
    }

    public Room GetRoomAt(Vector3 worldPos)
    {
        int x = Mathf.Clamp(Mathf.RoundToInt(worldPos.x / _cellSize), 0, _mazeSize - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(worldPos.z / _cellSize), 0, _mazeSize - 1);
        return _maze[x, y];
    }

    public List<Vector3> BuildPath(Room from, Room to)
    {
        var rooms = MazePathfinder.FindPath(_maze, from, to, _mazeSize);
        if (rooms == null) return null;

        var path = new List<Vector3>(rooms.Count);
        foreach (var r in rooms)
            path.Add(new Vector3(r.X * _cellSize, _heightOffset, r.Y * _cellSize));

        return path;
    }

    public bool CanSeePlayer()
    {
        if (Player == null) return false;

        Vector3 origin   = transform.position + Vector3.up * 0.5f;
        Vector3 toPlayer = Player.position + Vector3.up * 0.5f - origin;

        if (toPlayer.magnitude > detectionRadius) return false;

        if (Physics.Raycast(origin, toPlayer.normalized, out RaycastHit hit,
            toPlayer.magnitude, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            bool seen = hit.collider.CompareTag("Player");
            Debug.DrawLine(origin, hit.point, seen ? Color.green : Color.red);
            return seen;
        }

        Debug.DrawLine(origin, origin + toPlayer, Color.yellow);
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ServiceLocator.Instance.Get<GameStateMachine>()?.PlayerDied();
    }

    public string CurrentStateName => _current?.GetType().Name ?? "None";

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        DrawDetectionRadius();
        DrawWaypoints();
        DrawStateLabel();
    }

    private void DrawDetectionRadius()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.08f);
        Gizmos.DrawSphere(transform.position, detectionRadius);
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void DrawWaypoints()
    {
        if (Waypoints == null || Waypoints.Count == 0) return;

        for (int i = 0; i < Waypoints.Count; i++)
        {
            bool isCurrent = Application.isPlaying && i == CurrentWaypointIndex;

            Gizmos.color = isCurrent ? Color.yellow : new Color(0.3f, 0.8f, 1f);
            Gizmos.DrawSphere(Waypoints[i], isCurrent ? 0.25f : 0.15f);

            UnityEditor.Handles.Label(Waypoints[i] + Vector3.up * 0.35f, i.ToString());

            int next = (i + 1) % Waypoints.Count;
            Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.4f);
            Gizmos.DrawLine(Waypoints[i], Waypoints[next]);
        }
    }

    private void DrawStateLabel()
    {
        var style = new UnityEngine.GUIStyle(UnityEditor.EditorStyles.boldLabel)
        {
            fontSize = 13,
            normal = { textColor = Color.white }
        };

        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 2.2f,
            $"[ {CurrentStateName} ]",
            style);
    }
#endif
}
