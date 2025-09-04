using System;
using Scripts.Fruit;
using Unity.VisualScripting;
using UnityEngine;

public class Tree : MonoBehaviour
{
    enum State
    {
        Idle,
        Shaking
    }

    [SerializeField] private float _shakeTime = .3f;
    [SerializeField] private float _shakeSpeed = 1f;
    [Header("Debug")] 
    [SerializeField] private State _currentState;
    [SerializeField] private Quaternion _referenceOrientation;
    [SerializeField] private float _shakeCurrentTime = 0;
    private void Update()
    {
        if (_currentState == State.Shaking)
        {
            _shakeCurrentTime += Time.deltaTime;

            if (_shakeCurrentTime >= _shakeTime)
            {
                _currentState = State.Idle;
                transform.rotation = Quaternion.identity;
                return;
            }
            var slerp = 1 - _shakeCurrentTime / _shakeTime;
            var ping = Mathf.PingPong(_shakeCurrentTime * _shakeSpeed, slerp);
            
            transform.rotation = Quaternion.Slerp(Quaternion.identity,_referenceOrientation, ping);
            
        }
    }

    public void FruitPulled(FruitSpawner fruitSpawner)
    {
        _shakeCurrentTime = 0;

        var target = fruitSpawner.transform.position - transform.position;
        _referenceOrientation = Quaternion.FromToRotation(Vector3.up, target);
        _currentState = State.Shaking;
    }
}
