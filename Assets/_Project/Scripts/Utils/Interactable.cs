using System;
using UnityEngine;

namespace Scripts.Utils
{
    public class Interactable : MonoBehaviour
    {
        private InteractController _controller;
        public float Radius;

        public event Action<Vector3> OnInteracted;
        public event Action OnReleased;

        public InteractController Controller
        {
            set => _controller = value;
        }
        private void OnEnable()
        {
            _controller.Add(this);
        }

        private void OnDisable()
        {
            _controller.Remove(this);
        }

        public void Interacted(Vector3 position)
        {
            OnInteracted?.Invoke(position);
        }

        public void Released()
        {
            OnReleased?.Invoke();
        }
    }
}
