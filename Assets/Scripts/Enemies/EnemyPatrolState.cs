using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private readonly Enemy _enemy;

    public EnemyPatrolState(Enemy enemy)
    {
        _enemy = enemy;
    }

    public void Enter() { }
    public void Exit() { }

    public void Tick()
    {
        if (_enemy.CanSeePlayer())
        {
            _enemy.TransitionToChase();
            return;
        }

        Patrol();
    }

    private void Patrol()
    {
        List<Vector3> waypoints = _enemy.Waypoints;
        if (waypoints == null || waypoints.Count == 0) return;

        Vector3 target = waypoints[_enemy.CurrentWaypointIndex];
        Vector3 dir = target - _enemy.transform.position;

        if (new Vector2(dir.x, dir.z).magnitude <= _enemy.WaypointThreshold)
        {
            _enemy.CurrentWaypointIndex = (_enemy.CurrentWaypointIndex + 1) % waypoints.Count;
            return;
        }

        _enemy.transform.position = Vector3.MoveTowards(
            _enemy.transform.position, target, _enemy.Speed * Time.deltaTime);

        Vector3 lookDir = new Vector3(dir.x, 0f, dir.z);
        if (lookDir != Vector3.zero)
            _enemy.transform.forward = lookDir.normalized;
    }
}
