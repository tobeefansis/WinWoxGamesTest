using UnityEngine;

public enum WallDirection { North = 0, East = 1, South = 2, West = 3 }

public class Room
{
    public readonly int X;
    public readonly int Y;

    public readonly bool[] Walls = { true, true, true, true };

    public bool IsVisited;

    public GameObject SceneObject;

    public Room(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool HasWall(WallDirection dir) => Walls[(int)dir];
    public void RemoveWall(WallDirection dir) => Walls[(int)dir] = false;
}
