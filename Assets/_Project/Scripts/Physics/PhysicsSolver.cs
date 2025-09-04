using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Physics
{
    public class PhysicsSolver: MonoBehaviour
    {
        public static PhysicsSolver Instance;
        private Camera _camera;
        private List<PhysicsObject> _physicObjects;
        [SerializeField] private int capacity = 4;
        [SerializeField] private int RestingCount = 4;
        [SerializeField] private float restDistance = .001f;
    
        private void Awake()
        {
            Instance = this;
            _camera = Camera.main;
            _physicObjects = new List<PhysicsObject>(4);
        }

        public void Add(PhysicsObject physicsObject)
        {
            _physicObjects.Add(physicsObject);
        }

        public void Remove(PhysicsObject physicsObject)
        {
            _physicObjects.Remove(physicsObject);
        }
    
        //Normally the physics are ran at FixedUpdate but because the project is fully avoiding the physics library it is using the normal update.
        private void Update()
        {
            Step(Time.deltaTime);
        }
        private void Step(float deltaTime)
        {
            float height = _camera.orthographicSize;
            float width = height * _camera.aspect;

            for (int i = _physicObjects.Count - 1; i >= 0; i--)
            {
                var physicsObject = _physicObjects[i];
                if (physicsObject.IsResting)
                {
                    continue;
                }
            
                var position = physicsObject.Position;
                var velocity = physicsObject.Velocity;
            
                velocity += physicsObject.Acceleration * deltaTime;
                velocity -= physicsObject.Material.Drag * velocity;
                position += velocity * deltaTime;

                var offset = position.y - physicsObject.Radius;
            
                //Check if it reached the ground
                if (offset <= 0 && velocity.y * offset >=0)
                {
                    position.y = physicsObject.Radius;
                    velocity.y *= -physicsObject.Material.CollisionDamping;
                }

                //Check if there is movement, if not make the shape rest to avoid vibrations.
                if (Vector3.Magnitude(position - physicsObject.Position) < restDistance)
                {
                    physicsObject.RestingCounter++;
                    if (physicsObject.RestingCounter >= RestingCount)
                    {
                        physicsObject.IsResting = true;
                    }
                }
                else
                {
                    physicsObject.RestingCounter = 0;
                }

                if (physicsObject.CanWrapScreen && Mathf.Abs(position.x) > width)
                {
                    var x = position.x;
                    x += width;
                    x %= width;
                    x -= width;
                    position.x = x;
                }
                physicsObject.Position = position;
                physicsObject.Velocity = velocity;
            
                physicsObject.UpdateTransform();
            }
        }
    }
}
