using UnityEngine;

public class StartEndSpawner : MazeGenerationStage
{
    [Header("Prefabs (optional — primitives used if empty)")]
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject endPrefab;

    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private float spawnHeightOffset = 1f;

    public Room StartRoom { get; private set; }
    public Room EndRoom { get; private set; }

    private GameObject _startInstance;
    private GameObject _endInstance;

    public override void Execute(MazeContext ctx)
    {
        StartRoom = ctx.GetRoom(0, 0);
        EndRoom = ctx.GetRoom(ctx.Size - 1, ctx.Size - 1);

        Vector3 startPos = ctx.RoomWorldPosition(0, 0);
        Vector3 endPos = ctx.RoomWorldPosition(ctx.Size - 1, ctx.Size - 1);

        _startInstance = SpawnMarker(startPrefab, startPos, "Start", ctx.Root);
        _endInstance   = SpawnMarker(endPrefab,   endPos,   "End",   ctx.Root);
        _endInstance.AddComponent<ExitTrigger>();

        TeleportPlayer(startPos);
    }

    public override void Clear()
    {
        if (_startInstance != null) DestroyImmediate(_startInstance);
        if (_endInstance != null) DestroyImmediate(_endInstance);

        StartRoom = null;
        EndRoom = null;
    }

    private GameObject SpawnMarker(GameObject prefab, Vector3 worldPos, string label, Transform root)
    {
        GameObject marker;

        if (prefab != null)
        {
            marker = Instantiate(prefab, worldPos, Quaternion.identity, root);
        }
        else
        {
            marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.transform.SetParent(root);
            marker.transform.position = worldPos + new Vector3(0f, 0.5f, 0f);
            marker.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            var rend = marker.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = label == "Start" ? Color.green : Color.red;
        }

        marker.name = $"[{label}]";
        return marker;
    }

    private void TeleportPlayer(Vector3 startPos)
    {
        if (player == null)
        {
            Debug.LogWarning("[StartEndSpawner] Player not assigned — skipping teleport.");
            return;
        }

        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = startPos + new Vector3(0f, spawnHeightOffset, 0f);

        if (cc != null) cc.enabled = true;
    }
}
