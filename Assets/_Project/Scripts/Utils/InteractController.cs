using System.Collections.Generic;
using UnityEngine;

//Personally I prefer to use an injection library like VContainer but for the test I will not use an external package so a humble singleton is enough.  
namespace Scripts.Utils
{
    public class InteractController: MonoBehaviour
    {
        private const int NUMBER_OF_INTERACTABLES = 1;
        public static InteractController Instance;

        private List<Interactable> _interactables = new List<Interactable>();
        private HashSet<Interactable> _interacted = new HashSet<Interactable>();
    
        private Camera _camera;
        [SerializeField] private Vector3 _deltaMousePosition;
        public Vector3 DeltaPosition => _deltaMousePosition;
        [SerializeField] private Vector3 _lastMousePosition;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            Instance = this;
            _camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            _deltaMousePosition = Input.mousePosition - _lastMousePosition;
            _lastMousePosition = Input.mousePosition;
            
            //We will be using the old input system to identify if the mouse was pressed, I personally prefer using the Event System but because I wanted to avoid canvas objects and physics objects it will not be used. 
            if (Input.GetMouseButton(0))
            {


            
                var worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
                //Flatten the point.
                worldPoint.z = 0;
            
                foreach (var interactable in _interacted)
                {
                    interactable.Interacted(worldPoint);
                }
            
                if (_interacted.Count >= NUMBER_OF_INTERACTABLES)
                { 
                    return;
                }
            
                foreach (var interactable in _interactables)
                {
                    if (_interacted.Contains(interactable))
                    {
                        continue;
                    }
                    var position = interactable.transform.position;
                    position.z = 0;
                    var distance = Vector3.Magnitude(position - worldPoint);
                    if (distance <= interactable.Radius)
                    {

                        _interacted.Add(interactable);
                        interactable.Interacted(worldPoint);
                    }
                }

            }
            else
            {
                foreach (var interactable in _interacted)
                {
                    interactable.Released();
                }
                _interacted.Clear();
            }
        }

        public void Add(Interactable interactable)
        {
            _interactables.Add(interactable);
        }

        public void Remove(Interactable interactable)
        {
            _interactables.Remove(interactable);
        }
    }
}
