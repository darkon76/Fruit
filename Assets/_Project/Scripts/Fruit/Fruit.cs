using System;
using System.Collections.Generic;
using Scripts.Physics;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Fruit
{
    public class Fruit : MonoBehaviour
    {
        enum State
        {
            Growing,
            Waiting,
            Dragging,
            Free
        }
    
        [Header("References")]
        //At the awake we could find the references but ast the long run it is better to drag and drop at the inspector. 
        [SerializeField] private PhysicsObject _physicsObject;
        [SerializeField] private Interactable _interactable;
        [SerializeField] private AudioSource _audioSource;
        [Space]
        public float Radius  = 1;
        [Header("Grow")]
        [SerializeField] private AnimationCurve _growAnimationCurve;
        [SerializeField] private float _growTime = 1;
        [SerializeField] private AudioClip _growAudioClip;
        [Space] 
        [SerializeField] private AnimationCurve _deformationCurve;
        [Header("Physics")] 
        [SerializeField] private float _launchMultiplier = 2;
        [SerializeField] private Vector3 _addedLaunchForce = new Vector3(0,.1f,0);
        [SerializeField] private List<ForceToAudio> _groundForceToAudio = new();
        [Space] 
        [SerializeField] private float _autoDestroyTimer = 10f;
        [Header("Debug")]
        [SerializeField] private State _currentState;
        [SerializeField] private float _growCurrentTime = 0;
        [SerializeField] private float _autoDestroyCurrentTime = 0;
        private Vector3 _lastPosition;
        private Vector3 _lastDragPosition;

        public event Action OnInteracted; 

        private void Awake()
        {
            _interactable.OnInteracted += Interacted;
            _interactable.OnReleased += Released;

            _physicsObject.OnCollision += GroundCollision;
        }

        //Because we are pooling we need to reset the local variables.
        private void OnEnable()
        {
            _currentState = State.Growing;
            _growCurrentTime = 0;
            _autoDestroyCurrentTime = 0;
            _audioSource.clip = _growAudioClip;
            _audioSource.Play();
        }

        private void OnDisable()
        {
            _interactable.enabled = false;
            _physicsObject.enabled = false;
        }

        private void OnDestroy()
        {
            _interactable.OnInteracted -= Interacted;
            _interactable.OnReleased -= Released;
            
            _physicsObject.OnCollision -= GroundCollision;
        }
        
        private void GroundCollision()
        {
            var velocityMagnitude = Mathf.Abs(_physicsObject.Velocity.y);
            for (int i = _groundForceToAudio.Count - 1; i >= 0; i--)
            {
                if (_groundForceToAudio[i].Force <= velocityMagnitude)
                {
                    _audioSource.clip = _groundForceToAudio[i].AudioClip;
                    _audioSource.Play();
                    break;
                }
            }
        }

        private void Update()
        {
            DeformScale();
        
            switch (_currentState)
            {
                case State.Growing:
                    _growCurrentTime += Time.deltaTime;
                    transform.localScale = Vector3.one * _growAnimationCurve.Evaluate(_growCurrentTime / _growTime);
                    if (_growCurrentTime >= _growTime)
                    {
                        _lastPosition = transform.position;
                        _currentState = State.Waiting;
                        _interactable.enabled = true;
                    }
                
                    break;
                case State.Waiting:
                    break;
                case State.Dragging:
                    _autoDestroyCurrentTime = 0;
                    break;
                case State.Free:
                    _autoDestroyCurrentTime += Time.deltaTime;
                    if (_autoDestroyCurrentTime >= _autoDestroyTimer)
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void DeformScale()
        {
            //Uses the last position of the object to calculate how much the object will be deformed.
            //The only downside is that it doesn't squishes on collision. That will require soft body physics. 
            var deltaMovement = transform.position - _lastPosition;
            _lastPosition = transform.position;
        
            var difference = Mathf.Abs(deltaMovement.x) - Mathf.Abs(deltaMovement.y);
            var absDifference = Mathf.Abs(difference);
            var evaluatedDeformation = _deformationCurve.Evaluate(absDifference);
            Vector3 scale ;
            if (difference > 0)
            {
                scale = new Vector3(1 + evaluatedDeformation, 1 - evaluatedDeformation, 0);
            }
            else
            {
                scale = new Vector3(1 - evaluatedDeformation, 1 + evaluatedDeformation, 0);
            }

            transform.localScale = scale;
        }

        private void Released()
        {
            _currentState = State.Free;
            var launchVelocity = transform.position - _lastDragPosition;
            launchVelocity *= _launchMultiplier;
            launchVelocity += _addedLaunchForce;
            _physicsObject.Velocity = launchVelocity;
            _physicsObject.UpdatePhysics();
            _physicsObject.enabled = true;
        }

        private void Interacted(Vector3 mousePosition)
        {
            _currentState = State.Dragging;
            _lastDragPosition = transform.position;
            mousePosition.z = transform.position.z;
            transform.position = mousePosition;
            _physicsObject.enabled = false;
            OnInteracted?.Invoke();
        }

        private void OnValidate()
        {
            _interactable.Radius = Radius;
            _physicsObject.Radius = Radius;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, Radius);
        }
    }
}
