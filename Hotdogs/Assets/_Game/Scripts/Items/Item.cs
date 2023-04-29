using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IHoldable
{
    public delegate void OnHeld();
    public event OnHeld onHeld = delegate { };
    
    public delegate void OnDropped();
    public event OnDropped onDropped = delegate { };

    private Rigidbody _rigidbody;

    public bool isHeld { get; protected set; }

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();

        onHeld += OnHold;
        onDropped += OnDrop;
    }

    private void OnDisable()
    {
        onHeld -= OnHold;
        onDropped -= OnDrop;
    }

    public void Hold(Action onHold = null, Action onDrop = null)
    {
        onHeld?.Invoke();
    }

    public void Drop(Action onDropSuccess = null)
    {
        onDropped?.Invoke();
    }

    public void OnHold()
    {
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        Debug.Log($"Held! {gameObject.name}");
    }

    public void OnDrop()
    {
        _rigidbody.useGravity = true;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        Debug.Log($"Dropped! {gameObject.name}");
    }
}
