using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sankusa.unity1week202209.View {
    [RequireComponent(typeof(Camera))]
    public class Zoomer : MonoBehaviour
    {
        [SerializeField] private float zoomSpeed;
        public float ZoomSpeed => zoomSpeed;

        private Camera mainCamera;
        private float defaultSize;

        void Start()
        {
            mainCamera = Camera.main;
            defaultSize = mainCamera.orthographicSize;
        }

        void Update()
        {
            var scroll = Mouse.current.scroll.y.ReadValue();
            mainCamera.orthographicSize -= scroll * zoomSpeed;
        }
    }
}