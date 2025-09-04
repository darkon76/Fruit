using Scripts.Utils;
using UnityEngine;

namespace Scripts.Fruit
{
    public class FruitSpawner : MonoBehaviour
    {
        [SerializeField] private ObjectPoolSO _objectPoolSo;
        private Fruit _spawnedFruit;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Spawn();
        }


        [ContextMenu("Spawn")]
        void Spawn()
        {
            var fruitGo = _objectPoolSo.Get();
            var fruitT = fruitGo.transform;
            fruitT.SetPositionAndRotation(transform.position, transform.rotation);

            _spawnedFruit = fruitGo.GetComponent<Fruit>();
            if (_spawnedFruit != null)
            {
                _spawnedFruit.OnInteracted += FruitOnInteracted;
            }
        }

        private void FruitOnInteracted()
        {
            _spawnedFruit.OnInteracted -= FruitOnInteracted;
            _spawnedFruit = null;
            Spawn();
        }
    }
}
