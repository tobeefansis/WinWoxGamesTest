using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    [Header("Maze")]
    [SerializeField] private int size = 10;
    [SerializeField] private float cellSize = 4f;

    [Header("Stages")]
    [SerializeField] private MazeGenerationStage[] stages;

    public Room[,] Maze { get; private set; }

    private void Start() => Generate();

    [ContextMenu("Generate Maze")]
    public void Generate()
    {
        ClearAll();

        Maze = MazeGenerator.Generate(size);

        var ctx = new MazeContext
        {
            Maze = Maze,
            Size = size,
            CellSize = cellSize,
            Root = transform
        };

        foreach (var stage in stages)
            stage?.Execute(ctx);
    }

    private void ClearAll()
    {
        foreach (var stage in stages)
            stage?.Clear();

        Maze = null;
    }
}
