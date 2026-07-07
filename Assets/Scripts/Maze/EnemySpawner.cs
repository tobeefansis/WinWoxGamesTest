using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MazeGenerationStage
{
    [Header("Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int count = 3;
    [SerializeField] private int minPatrolDistance = 3;
    [SerializeField] private float heightOffset = 0.5f;

    private Transform _root;
    private MazeContext _ctx;

    public override void Execute(MazeContext ctx)
    {
        _ctx = ctx;

        _root = new GameObject("[Enemies]").transform;
        _root.SetParent(ctx.Root);
        _root.localPosition = Vector3.zero;

        var validRooms = BuildValidRoomList(ctx);

        for (int i = 0; i < count; i++)
        {
            if (!TryPickPatrolPair(validRooms, out Room roomA, out Room roomB))
            {
                Debug.LogWarning("[EnemySpawner] Could not find a valid patrol pair.");
                continue;
            }

            var roomPath = MazePathfinder.FindPath(ctx.Maze, roomA, roomB, ctx.Size);
            if (roomPath == null || roomPath.Count < 2) continue;

            var waypoints = BuildRoundTrip(roomPath, ctx);
            SpawnEnemy(waypoints, ctx);
        }
    }

    public override void Clear()
    {
        if (_root != null)
            DestroyImmediate(_root.gameObject);
    }

    private List<Room> BuildValidRoomList(MazeContext ctx)
    {
        var list = new List<Room>();
        for (int x = 0; x < ctx.Size; x++)
            for (int y = 0; y < ctx.Size; y++)
            {
                if (x == 0 && y == 0) continue;
                if (x == ctx.Size - 1 && y == ctx.Size - 1) continue;
                list.Add(ctx.GetRoom(x, y));
            }
        return list;
    }

    private bool TryPickPatrolPair(List<Room> rooms, out Room roomA, out Room roomB)
    {
        roomA = null;
        roomB = null;

        if (rooms.Count < 2) return false;

        for (int attempt = 0; attempt < 30; attempt++)
        {
            roomA = rooms[Random.Range(0, rooms.Count)];
            roomB = rooms[Random.Range(0, rooms.Count)];

            if (roomA != roomB && ManhattanDistance(roomA, roomB) >= minPatrolDistance)
                return true;
        }

        return false;
    }

    private List<Vector3> BuildRoundTrip(List<Room> path, MazeContext ctx)
    {
        var waypoints = new List<Vector3>(path.Count * 2 - 2);

        for (int i = 0; i < path.Count; i++)
            waypoints.Add(RoomToWorld(path[i], ctx));

        for (int i = path.Count - 2; i > 0; i--)
            waypoints.Add(RoomToWorld(path[i], ctx));

        return waypoints;
    }

    private Vector3 RoomToWorld(Room room, MazeContext ctx) =>
        ctx.RoomWorldPosition(room.X, room.Y) + new Vector3(0f, heightOffset, 0f);

    private void SpawnEnemy(List<Vector3> waypoints, MazeContext ctx)
    {
        GameObject go;

        if (enemyPrefab != null)
        {
            go = Instantiate(enemyPrefab, waypoints[0], Quaternion.identity, _root);
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.transform.SetParent(_root);
            go.transform.position = waypoints[0];
            go.transform.localScale = new Vector3(0.5f, 0.8f, 0.5f);

            var rend = go.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = new Color(1f, 0.3f, 0.3f);
        }

        go.name = "Enemy";
        go.AddComponent<Enemy>().Initialize(waypoints, ctx.Maze, ctx.Size, ctx.CellSize, heightOffset);
    }

    private static int ManhattanDistance(Room a, Room b) =>
        Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
}
