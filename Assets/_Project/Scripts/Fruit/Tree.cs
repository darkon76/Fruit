using Scripts.Fruit;
using UnityEngine;

public class Tree : MonoBehaviour
{
    enum State
    {
        Idle,
        Shaking
    }

    [SerializeField] private float _shakeTime = .3f;
    [SerializeField] private float _shakeForce = 1f;
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
            var slerp = _shakeCurrentTime / _shakeTime;
            
            transform.rotation = Quaternion.Slerp(_referenceOrientation, Quaternion.identity, slerp);
            
        }
    }

    public void FruitPulled(FruitSpawner fruitSpawner)
    {
        _shakeCurrentTime = 0;
        var target = (fruitSpawner.transform.position - transform.position);
        target.x *= _shakeForce;
        _referenceOrientation = Quaternion.FromToRotation(Vector3.up, target);
        
        
        _currentState = State.Shaking;
    }
}
