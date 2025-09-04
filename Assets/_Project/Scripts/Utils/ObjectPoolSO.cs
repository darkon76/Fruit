using Scripts.Physics;
using UnityEngine;
using UnityEngine.Pool;

namespace Scripts.Utils
{
    [CreateAssetMenu(fileName = "ObjectPoolSO", menuName = "Scriptable Objects/ObjectPoolSO")]
    public class ObjectPoolSO : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private bool collectionChecks = false;
        [SerializeField] private int defaultCapacity = 1;
        [SerializeField] private int maxPoolSize =10;
    
        private IObjectPool<GameObject> _objectPool;

        public void OnEnable()
        {
            _objectPool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, null, OnDestroyPoolObject, collectionChecks, defaultCapacity, maxPoolSize);
        }
    
        private GameObject CreatePooledItem()
        {
            var go = Instantiate(_prefab);
            var returnToPool = go.AddComponent<ReturnToPool>();
            returnToPool.Pool = _objectPool;

            var po = go.GetComponent<PhysicsObject>();
            if (po != null)
            {
                po.PhysicsSolver = PhysicsSolver.Instance;
            }

            var interactable = go.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.Controller = InteractController.Instance;
            }
        
            return go;
        }
        private void OnDestroyPoolObject(GameObject obj)
        {
            //Only destroy the objects at play mode, to avoid an editor error. 
            if (Application.isPlaying)
            {
                Destroy(obj);
            }
        }

        private void OnTakeFromPool(GameObject obj)
        {
            obj.SetActive(true);
        }

        public GameObject Get()
        {
            return _objectPool.Get();
        }

        public void OnDisable()
        {
            _objectPool.Clear();
        }
    }
}
