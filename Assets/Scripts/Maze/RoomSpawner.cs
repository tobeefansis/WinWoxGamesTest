using UnityEngine;

public class RoomSpawner : MazeGenerationStage
{
    [Header("Wall Settings")]
    [SerializeField] private float wallHeight = 3f;
    [SerializeField] private float wallThickness = 0.2f;

    [Header("Prefabs (optional — primitives used if empty)")]
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject wallPrefab;

    private Transform _root;

    public override void Execute(MazeContext ctx)
    {
        _root = new GameObject("[Rooms]").transform;
        _root.SetParent(ctx.Root);
        _root.localPosition = Vector3.zero;

        for (int x = 0; x < ctx.Size; x++)
            for (int y = 0; y < ctx.Size; y++)
                SpawnRoom(ctx.Maze[x, y], x, y, ctx);
    }

    public override void Clear()
    {
        if (_root != null)
            DestroyImmediate(_root.gameObject);
    }

    private void SpawnRoom(Room room, int x, int y, MazeContext ctx)
    {
        Vector3 origin = ctx.RoomWorldPosition(x, y);

        GameObject roomGO;
        if (roomPrefab != null)
        {
            roomGO = Instantiate(roomPrefab, origin, Quaternion.identity, _root);
        }
        else
        {
            roomGO = new GameObject();
            roomGO.transform.SetParent(_root);
            roomGO.transform.position = origin;

            CreatePrimitive(roomGO.transform, "Floor",
                new Vector3(0f, -0.05f, 0f),
                new Vector3(ctx.CellSize, 0.1f, ctx.CellSize));
        }

        roomGO.name = $"Room_{x}_{y}";
        room.SceneObject = roomGO;

        SpawnWalls(room, x, y, ctx, roomGO.transform);
    }

    private void SpawnWalls(Room room, int x, int y, MazeContext ctx, Transform parent)
    {
        float c = ctx.CellSize;

        if (room.HasWall(WallDirection.South))
            SpawnWall(parent, new Vector3(0f, 0f, -c * 0.5f), alongZ: false, ctx);

        if (room.HasWall(WallDirection.West))
            SpawnWall(parent, new Vector3(-c * 0.5f, 0f, 0f), alongZ: true, ctx);

        if (y == ctx.Size - 1 && room.HasWall(WallDirection.North))
            SpawnWall(parent, new Vector3(0f, 0f, c * 0.5f), alongZ: false, ctx);

        if (x == ctx.Size - 1 && room.HasWall(WallDirection.East))
            SpawnWall(parent, new Vector3(c * 0.5f, 0f, 0f), alongZ: true, ctx);
    }

    private void SpawnWall(Transform parent, Vector3 localPos, bool alongZ, MazeContext ctx)
    {
        float c = ctx.CellSize;
        localPos.y = wallHeight * 0.5f;

        Vector3 scale = alongZ
            ? new Vector3(wallThickness, wallHeight, c + wallThickness)
            : new Vector3(c + wallThickness, wallHeight, wallThickness);

        GameObject wall;
        if (wallPrefab != null)
        {
            wall = Instantiate(wallPrefab, parent);
            wall.transform.localPosition = localPos;
            wall.transform.localScale = scale;
        }
        else
        {
            wall = CreatePrimitive(parent, "Wall", localPos, scale);
        }

        wall.name = "Wall";
    }

    private GameObject CreatePrimitive(Transform parent, string label, Vector3 localPos, Vector3 scale)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = label;
        go.transform.SetParent(parent);
        go.transform.localPosition = localPos;
        go.transform.localScale = scale;
        return go;
    }
}
