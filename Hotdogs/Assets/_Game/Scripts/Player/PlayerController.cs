using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Config")]
    [SerializeField] private Rigidbody pivotPhysics;

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

    // Start is called before the first frame update
    void Start()
    {
        pivotPhysics ??= transform.GetChild(0).GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePivotRigidbody();
    }

    private void UpdatePivotRigidbody()
    {
        Vector3 inputDir = InputManager.Instance.GetMovement();
        inputDir.z = inputDir.y;
        inputDir.y = 0;

        Vector3 localDirection =
            transform.forward * inputDir.z +
            mainCamera.transform.right * inputDir.x;

        bool inputHeld = inputDir.magnitude > 0f;

        pivotPhysics.AddForce(movementForce * localDirection * Time.deltaTime);

        if (pivotPhysics.velocity.magnitude > maxForce)
        {
            pivotPhysics.velocity = pivotPhysics.velocity.normalized * maxForce;
        }

        if (!inputHeld)
        {
            pivotPhysics.velocity *= (1 - Time.deltaTime * movementDamping);
        }
    }
}
