using System.Collections.Generic;
using UnityEngine;

public class EnemyReturnState : IEnemyState
{
    private readonly Enemy _enemy;
    private List<Vector3> _path;
    private int _pathIndex;

    public EnemyReturnState(Enemy enemy)
    {
        _enemy = enemy;
    }

    public void Enter()
    {
        int nearestIndex = FindNearestPatrolIndex();
        _enemy.CurrentWaypointIndex = nearestIndex;

        var fromRoom = _enemy.GetRoomAt(_enemy.transform.position);
        var toRoom   = _enemy.GetRoomAt(_enemy.Waypoints[nearestIndex]);

        _path = _enemy.BuildPath(fromRoom, toRoom);
        _pathIndex = (_path != null && _path.Count > 1) ? 1 : 0;
    }

    public void Exit() { }

    public void Tick()
    {
        if (_enemy.CanSeePlayer())
        {
            _enemy.TransitionToChase();
            return;
        }

        if (_path == null || _pathIndex >= _path.Count)
        {
            _enemy.TransitionToPatrol();
            return;
        }

        Vector3 target = _path[_pathIndex];
        float dist = new Vector2(
            target.x - _enemy.transform.position.x,
            target.z - _enemy.transform.position.z).magnitude;

        if (dist <= _enemy.WaypointThreshold)
        {
            _pathIndex++;
            return;
        }

        Vector3 dir = target - _enemy.transform.position;
        _enemy.transform.position = Vector3.MoveTowards(
            _enemy.transform.position, target, _enemy.Speed * Time.deltaTime);

        Vector3 lookDir = new Vector3(dir.x, 0f, dir.z);
        if (lookDir != Vector3.zero)
            _enemy.transform.forward = lookDir.normalized;
    }

    private int FindNearestPatrolIndex()
    {
        var waypoints = _enemy.Waypoints;
        int best = _enemy.CurrentWaypointIndex;
        float bestDist = float.MaxValue;

        for (int i = 0; i < waypoints.Count; i++)
        {
            float d = Vector3.Distance(_enemy.transform.position, waypoints[i]);
            if (d < bestDist) { bestDist = d; best = i; }
        }

        return best;
    }
}
