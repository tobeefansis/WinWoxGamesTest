using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private readonly Enemy _enemy;

    private List<Vector3> _path;
    private int   _pathIndex;
    private float _lostTimer;
    private float _recalcTimer;

    private const float RecalcInterval = 0.5f;

    public EnemyChaseState(Enemy enemy)
    {
        _enemy = enemy;
    }

    public void Enter()
    {
        _lostTimer   = 0f;
        _recalcTimer = 0f;
        RecalcPath();
    }

    public void Exit() { }

    public void Tick()
    {
        if (_enemy.CanSeePlayer())
        {
            _lostTimer = 0f;

            _recalcTimer += Time.deltaTime;
            if (_recalcTimer >= RecalcInterval)
            {
                _recalcTimer = 0f;
                RecalcPath();
            }

            FollowPath();
        }
        else
        {
            _lostTimer += Time.deltaTime;

            if (_lostTimer >= _enemy.ReturnDelay)
                _enemy.TransitionToReturn();
            else
                FollowPath();
        }
    }

    private void RecalcPath()
    {
        var from = _enemy.GetRoomAt(_enemy.transform.position);
        var to   = _enemy.GetRoomAt(_enemy.Player.position);

        _path = _enemy.BuildPath(from, to);

        _pathIndex = (_path != null && _path.Count > 1) ? 1 : 0;
    }

    private void FollowPath()
    {
        if (_path == null || _pathIndex >= _path.Count)
        {
            MoveTowards(_enemy.Player.position);
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

        MoveTowards(target);
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 dir = target - _enemy.transform.position;
        _enemy.transform.position = Vector3.MoveTowards(
            _enemy.transform.position, target, _enemy.Speed * Time.deltaTime);

        Vector3 lookDir = new Vector3(dir.x, 0f, dir.z);
        if (lookDir != Vector3.zero)
            _enemy.transform.forward = lookDir.normalized;
    }
}
