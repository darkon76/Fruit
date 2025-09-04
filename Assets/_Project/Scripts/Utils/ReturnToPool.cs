using UnityEngine;
using UnityEngine.Pool;

namespace Scripts.Utils
{
    public class ReturnToPool : MonoBehaviour
    {
        public IObjectPool<GameObject> Pool;

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                Pool.Release(gameObject);
            }
        }
    }
}
