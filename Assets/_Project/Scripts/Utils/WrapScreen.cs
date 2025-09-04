using Scripts.Physics;
using UnityEngine;

namespace Scripts.Utils
{
    public class WrapScreen : MonoBehaviour
    {
        private Camera _camera;
        [SerializeField] PhysicsObject _physicsObject;

        private void Awake()
        {
            _camera = Camera.main;
        }
        
        
        private void Update()
        {
            float height = _camera.orthographicSize;
            float width = height * _camera.aspect;
            
            var position = transform.position;
            if (Mathf.Abs(position.x) > width)
            {
                var x = position.x;
                var sign = Mathf.Sign(x);
                x = Mathf.Abs(x);
                x %= width;
                x -= width;
                x *= sign;
                position.x = x;
                transform.position = position;
                _physicsObject.UpdatePhysics();
            }
        }
    }
}