using System.Collections.Generic;

public static class MazePathfinder
{
    public static List<Room> FindPath(Room[,] maze, Room start, Room end, int size)
    {
        if (start == end) return new List<Room> { start };

        var parent = new Dictionary<Room, Room>();
        var queue = new Queue<Room>();

        queue.Enqueue(start);
        parent[start] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == end) break;

            foreach (var neighbor in GetPassableNeighbors(maze, current, size))
            {
                if (!parent.ContainsKey(neighbor))
                {
                    parent[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (!parent.ContainsKey(end)) return null;

        var path = new List<Room>();
        var room = end;
        while (room != null)
        {
            path.Add(room);
            parent.TryGetValue(room, out room);
        }
        path.Reverse();
        return path;
    }

    public static IEnumerable<Room> GetPassableNeighbors(Room[,] maze, Room room, int size)
    {
        int x = room.X, y = room.Y;
        if (y + 1 < size && !room.HasWall(WallDirection.North)) yield return maze[x, y + 1];
        if (x + 1 < size && !room.HasWall(WallDirection.East))  yield return maze[x + 1, y];
        if (y - 1 >= 0   && !room.HasWall(WallDirection.South)) yield return maze[x, y - 1];
        if (x - 1 >= 0   && !room.HasWall(WallDirection.West))  yield return maze[x - 1, y];
    }
}
