using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementModule : MonoBehaviour
{
    [Header("Player Config")]
    [SerializeField] private Rigidbody _rigidbody = null;
    [SerializeField] private GameObject head = null;

    [Header("Temp Settings")]
    [SerializeField] private float movementForce = 400f;
    [SerializeField] private float movementDamping = 0.98f;
    [SerializeField] private float maxForce = 4000f;

    private Camera mainCamera
    {
        get
        {
            _mainCamera ??= Camera.main;
            return _mainCamera;
        }
    }
    private Camera _mainCamera;

    private void OnValidate()
    {
        _rigidbody = GetComponent<Rigidbody>();
        head = _rigidbody.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        UpdatePlayerPosition();
    }

    private void LateUpdate()
    {
        UpdateLookDirection();
    }

    private void UpdatePlayerPosition()
    {
        Vector3 inputDir = InputManager.Instance.GetMovement();
        inputDir.z = inputDir.y;
        inputDir.y = 0;

        bool inputHeld = inputDir.magnitude > 0f;
        Vector3 localDirection =
            transform.forward * inputDir.z +
            mainCamera.transform.right * inputDir.x;

        _rigidbody.AddForce(movementForce * localDirection * Time.deltaTime);

        if (_rigidbody.velocity.magnitude > maxForce)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * maxForce;
        }

        if (!inputHeld)
        {
            _rigidbody.velocity *= (1 - Time.deltaTime * movementDamping);
        }
    }

    private void UpdateLookDirection()
    {
        Vector3 lookDir = mainCamera.transform.forward;
        Vector3 bodyDir = lookDir;
        bodyDir.y = 0;

        transform.LookAt(transform.position + bodyDir);
        head.transform.LookAt(transform.position + mainCamera.transform.forward);
    }
}
