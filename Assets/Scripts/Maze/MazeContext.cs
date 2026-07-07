using UnityEngine;

public class MazeContext
{
    public Room[,] Maze;
    public int Size;
    public float CellSize;
    public Transform Root;

    public Room GetRoom(int x, int y) => Maze[x, y];

    public Vector3 RoomWorldPosition(int x, int y) =>
        Root.position + new Vector3(x * CellSize, 0f, y * CellSize);
}
