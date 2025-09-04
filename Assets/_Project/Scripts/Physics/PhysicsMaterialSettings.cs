using UnityEngine;

namespace Scripts.Physics
{
    [CreateAssetMenu(fileName = "PhysicsMaterial", menuName = "Scriptable Objects/PhysicsMaterial")]
    public class PhysicsMaterialSettings : ScriptableObject
    {
        [SerializeField] private float _collisionDamping = 0.8f;
        [SerializeField] private float _drag = .01f;

        public float CollisionDamping => _collisionDamping;
        public float Drag => _drag;
    }
}
