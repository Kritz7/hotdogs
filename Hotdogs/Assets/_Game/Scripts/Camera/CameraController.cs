using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : CinemachineExtension
{
    [Header("Camera Config")]
    [SerializeField] private bool yInvert = false;
    [SerializeField] private bool xInvert = false;
    [SerializeField] private Vector2 sensitivity = new Vector2(10, 10);
    [SerializeField] private Vector2 yClamp = new Vector2(-85, 85);

    private bool init;
    private int startupFamesCameraDelay = 90;
    private Vector3 initialRotation;
    private Vector3 currentRotation;

    protected override void OnEnable()
    {
        base.OnEnable();

        SetCursorLock(true);

        initialRotation = transform.localRotation.eulerAngles;
        currentRotation = initialRotation;
    }

    private void OnDisable()
    {
        SetCursorLock(false);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetCursorLock(false);
        }

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            SetCursorLock(true);
        }
    }

    private void SetCursorLock(bool locked)
    {
        Cursor.lockState = locked ?
            CursorLockMode.Confined :
            CursorLockMode.None;

        Cursor.visible = !locked;
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (!Application.isPlaying)
            return;

        if (!vcam.Follow)
            return;

        if (stage != CinemachineCore.Stage.Aim)
            return;

        // Camera starts reading the moment you hit 'play' in editor causing weird behaviour.
        if (Time.frameCount < startupFamesCameraDelay)
            return;

        // Need access to `state` so have to perform some init here
        if(!init)
        {
            Camera.main.transform.localRotation = Quaternion.identity;
            currentRotation = Vector3.zero;

            state.RawOrientation = Quaternion.Euler(currentRotation);
            init = true;
            return;
        }

        Vector2 inputDelta = InputManager.Instance.GetLookDelta();
        currentRotation.x += inputDelta.x * sensitivity.x * deltaTime;
        currentRotation.y += inputDelta.y * sensitivity.y * deltaTime;
        currentRotation.y = Mathf.Clamp(currentRotation.y, yClamp.x, yClamp.y);

        state.RawOrientation = Quaternion.Euler(
            yInvert ? -currentRotation.y : currentRotation.y,
            xInvert ? -currentRotation.x : currentRotation.x, 0f);
    }
}
