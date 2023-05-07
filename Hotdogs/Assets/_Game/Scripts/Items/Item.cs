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

    // There are multiple pathways to stack on additional action requests
    private Action onHoldRequest;
    private Action onDropFromHoldRequest;
    private Action onDropRequest;

    private Rigidbody _rigidbody;
    private Vector3 goalPosition;
    private Vector3 goalFacingDirection;
    private Quaternion goalFacingRotation;
    private int defaultLayer;

    public bool isHeld { get; protected set; }
    public Rigidbody Rigidbody => _rigidbody;

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        defaultLayer = gameObject.layer;

        onHeld += OnHold;
        onDropped += OnDrop;
    }

    private void OnDisable()
    {
        onHeld -= OnHold;
        onDropped -= OnDrop;
    }

    private void Update()
    {
        Rigidbody.isKinematic = isHeld;

        if (!isHeld)
            return;

        goalFacingRotation = Quaternion.LookRotation(goalFacingDirection);

        transform.position = Vector3.Lerp(transform.position, goalPosition, 10f * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, goalFacingRotation, 30f * Time.deltaTime);

        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
    }

    public void Hold(Action onHold = null, Action onDrop = null)
    {
        onHoldRequest = onHold;
        onDropFromHoldRequest = onDrop;

        onHeld?.Invoke();
    }

    public void Drop(Action onDropSuccess = null)
    {
        onDropRequest = onDropSuccess;
        onDropped?.Invoke();
    }

    public void OnHold()
    {
        Debug.Log($"{name} was just held!");

        isHeld = true;

        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        SetLayer(LayerMask.NameToLayer("PlayerHands"));

        onHoldRequest?.Invoke();
    }

    public void OnDrop()
    {
        Debug.Log($"{name} was just dropped!");

        isHeld = false;

        _rigidbody.useGravity = true;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        SetLayer(defaultLayer);

        onDropFromHoldRequest?.Invoke();
        onDropRequest?.Invoke();
    }

    public void SetPosition(Vector3 position, Vector3 forward)
    {
        goalPosition = position;
        goalFacingDirection = forward;
    }

    private void SetLayer(int layer)
    {
        foreach(var renderer in GetComponentsInChildren<MeshRenderer>(true))
        {
            renderer.gameObject.layer = layer;
        }
    }
}
