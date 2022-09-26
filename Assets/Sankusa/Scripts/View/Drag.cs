using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UniRx;
using System;
using UnityEngine.InputSystem;

namespace Sankusa.unity1week202209.View {
    public class Drag : MonoBehaviour, IPointerDownHandler
    {
        // rigidbody.velocityで移動しないと設置済みブロックが接触時にずれる

        private UnityEvent onDragBegin = new UnityEvent();
        public IObservable<Unit> OnDragBegin => onDragBegin.AsObservable();

        private UnityEvent onDragEnd = new UnityEvent();
        public IObservable<Unit> OnDragEnd => onDragEnd.AsObservable();

        private bool dragging = false;
        public bool Dragging => dragging;

        private Rigidbody2D rb;

        void Start() {
            rb = GetComponent<Rigidbody2D>();
        }
        public void OnPointerDown(PointerEventData data) {
            dragging = true;
            onDragBegin.Invoke();
        }

        void Update() {
            if(dragging) {
                if(Mouse.current.leftButton.wasReleasedThisFrame) {
                    onDragEnd.Invoke();
                    dragging = false;
                    rb.velocity = Vector2.zero;
                }
            }
        }

        void FixedUpdate() {
            if(dragging) rb.velocity = (Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position) / (1f / 60f);
        }

        public void Reset() {
            dragging = false;
            rb.velocity = Vector2.zero;
        }
    }
}