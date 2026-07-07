using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator
{
    private static readonly (int dx, int dy, WallDirection from, WallDirection to)[] Dirs =
    {
        ( 0,  1, WallDirection.North, WallDirection.South),
        ( 1,  0, WallDirection.East,  WallDirection.West ),
        ( 0, -1, WallDirection.South, WallDirection.North),
        (-1,  0, WallDirection.West,  WallDirection.East ),
    };

    public static Room[,] Generate(int size)
    {
        var maze = new Room[size, size];

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                maze[x, y] = new Room(x, y);

        var stack = new Stack<Room>();
        Room start = maze[0, 0];
        start.IsVisited = true;
        stack.Push(start);

        while (stack.Count > 0)
        {
            Room current = stack.Peek();
            var neighbors = GetUnvisitedNeighbors(maze, current, size);

            if (neighbors.Count > 0)
            {
                var (next, from, to) = neighbors[Random.Range(0, neighbors.Count)];
                current.RemoveWall(from);
                next.RemoveWall(to);
                next.IsVisited = true;
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }

        return maze;
    }

    private static List<(Room room, WallDirection from, WallDirection to)> GetUnvisitedNeighbors(
        Room[,] maze, Room room, int size)
    {
        var result = new List<(Room, WallDirection, WallDirection)>(4);

        foreach (var (dx, dy, from, to) in Dirs)
        {
            int nx = room.X + dx;
            int ny = room.Y + dy;

            if (nx >= 0 && nx < size && ny >= 0 && ny < size && !maze[nx, ny].IsVisited)
                result.Add((maze[nx, ny], from, to));
        }

        return result;
    }
}
