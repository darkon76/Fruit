using System;
using UnityEngine;

namespace Scripts.Physics
{
    public class PhysicsObject : MonoBehaviour
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public bool IsResting;
        public int RestingCounter = 0;
        public float Radius;

        public PhysicsMaterialSettings Material;

        private PhysicsSolver _solver;

        public event Action OnCollision; 

        public PhysicsSolver PhysicsSolver
        {
            set => _solver = value;
        }

        public void OnEnable()
        {
            IsResting = false;
            RestingCounter = 0;
            _solver.Add(this);
        }

        public void OnDisable()
        {
            _solver.Remove(this);
        }

        public void UpdatePhysics()
        {
            Position = transform.position;
        }
        public void UpdateTransform()
        {
            transform.position = Position;
        }

        public void Collision()
        {
            OnCollision?.Invoke();
        }
    }
}
