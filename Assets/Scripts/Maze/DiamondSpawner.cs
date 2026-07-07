using System.Collections.Generic;
using UnityEngine;

public class DiamondSpawner : MazeGenerationStage
{
    [Header("Settings")]
    [SerializeField] private GameObject diamondPrefab;
    [SerializeField] private int minCount = 5;
    [SerializeField] private int maxCount = 10;
    [SerializeField] private float heightOffset = 1f;

    private Transform _root;

    public override void Execute(MazeContext ctx)
    {
        _root = new GameObject("[Diamonds]").transform;
        _root.SetParent(ctx.Root);
        _root.localPosition = Vector3.zero;

        var candidates = BuildCandidateList(ctx);
        Shuffle(candidates);

        int spawnCount = Mathf.Min(Random.Range(minCount, maxCount + 1), candidates.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            Room room = candidates[i];
            Vector3 pos = ctx.RoomWorldPosition(room.X, room.Y) + new Vector3(0f, heightOffset, 0f);
            SpawnDiamond(pos);
        }

        ServiceLocator.Instance.Get<GameStateMachine>()?.Initialize(spawnCount);
    }

    public override void Clear()
    {
        if (_root != null)
            DestroyImmediate(_root.gameObject);
    }

    private List<Room> BuildCandidateList(MazeContext ctx)
    {
        var list = new List<Room>();

        for (int x = 0; x < ctx.Size; x++)
            for (int y = 0; y < ctx.Size; y++)
            {
                if (x == 0 && y == 0) continue;
                if (x == ctx.Size - 1 && y == ctx.Size - 1) continue;
                if (!IsDeadEnd(ctx.GetRoom(x, y))) continue;

                list.Add(ctx.GetRoom(x, y));
            }

        return list;
    }

    private static bool IsDeadEnd(Room room)
    {
        int wallCount = 0;
        for (int i = 0; i < 4; i++)
            if (room.Walls[i]) wallCount++;
        return wallCount == 3;
    }

    private void SpawnDiamond(Vector3 pos)
    {
        GameObject diamond;

        if (diamondPrefab != null)
        {
            diamond = Instantiate(diamondPrefab, pos, Quaternion.identity, _root);
        }
        else
        {
            diamond = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            diamond.transform.SetParent(_root);
            diamond.transform.position = pos;
            diamond.transform.localScale = Vector3.one * 0.4f;

            var rend = diamond.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = new Color(0.4f, 0.9f, 1f);
        }

        diamond.name = "Diamond";
        diamond.AddComponent<Diamond>();
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
