using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private Transform[] _targets;  // Array of targets (could be Transforms or just positions)

    [SerializeField]
    private float _speed;

    private int _targetIndex = 0; // To keep track of which target the platform is moving to

    private Transform _previousTarget;
    private Transform _currentTarget;

    private float _timeToTarget;
    private float _elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        // Make sure we start moving to the first target
        if (_targets.Length > 0)
        {
            _currentTarget = _targets[0];
            _previousTarget = _targets[_targets.Length - 1];
            targetNextTarget();  // Start the movement
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_targets.Length == 0) return;  // Exit early if no targets

        _elapsedTime += Time.deltaTime;

        float elapsedPercentage = _elapsedTime / _timeToTarget;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage); // Smooth transition

        // Move the platform from the previous target to the current target
        transform.position = Vector3.Lerp(_previousTarget.position, _currentTarget.position, elapsedPercentage);

        // Once we've reached the target, move to the next one
        if (elapsedPercentage >= 1)
        {
            targetNextTarget();
        }
    }

    // Select the next target and calculate time to get there
    private void targetNextTarget()
    {
        _previousTarget = _currentTarget;

        // Move to the next target, wrapping around if needed
        _targetIndex = (_targetIndex + 1) % _targets.Length;
        _currentTarget = _targets[_targetIndex];

        // Reset the timer for the new movement
        _elapsedTime = 0;

        float distanceToTarget = Vector3.Distance(_previousTarget.position, _currentTarget.position);
        _timeToTarget = distanceToTarget / _speed; // Time to move to the next target
    }

    private void OnTriggerEnter(Collider other)
    {
        // Attach the object to the moving platform
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        // Detach the object when it leaves the platform
        other.transform.SetParent(null);
    }
}
