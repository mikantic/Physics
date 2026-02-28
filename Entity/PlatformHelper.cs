using UnityEngine;

public class PlatformHelper : MonoBehaviour
{
    /// <summary>
    /// rigidbody
    /// </summary>
    [SerializeField] private Rigidbody _rigidbody;

    /// <summary>
    /// list of positions to meet
    /// </summary>
    [SerializeField] private Vector3[] _goalPositions;

    /// <summary>
    /// how close it needs to get to complete a goal position
    /// </summary>
    [SerializeField] private float _margin;

    /// <summary>
    /// how fast it moves
    /// </summary>
    [SerializeField] private float _speed;

    /// <summary>
    /// if the platform moves to start after reaching the end or goes backwards from the end
    /// </summary>
    [SerializeField] private bool _loop;

    /// <summary>
    /// current goal position
    /// </summary>
    private int _goalIndex;

    private int _direction = 1;

    private void Start()
    {
        if (_goalPositions.Length > 0)
        {
            _goalIndex = 0;
        }
    }

    private void FixedUpdate()
    {
        if (_goalPositions.Length <= 0) return;

        Vector3 direction = (_goalPositions[_goalIndex] - transform.position).normalized * _speed;
        _rigidbody.linearVelocity = direction;

        if (Vector3.Distance(_goalPositions[_goalIndex], transform.position) <= _margin)
        {
            _goalIndex += _direction;
            if (_loop)
            {
                if (_goalIndex >= _goalPositions.Length) _goalIndex = 0;
            }
            else
            {
                if (_goalIndex >= _goalPositions.Length || _goalIndex < 0)
                {
                    _direction *= -1;
                    _goalIndex += _direction * 2;
                }
            }
        }

    }

    
}
