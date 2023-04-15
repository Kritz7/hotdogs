using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    private PlayerControls controls;

    protected override void Awake()
    {
        base.Awake();

        controls ??= new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public Vector2 GetLookDelta()
    {
        if (!Application.isPlaying)
            return Vector2.zero;

        if (controls == null || controls.Player.Look == null)
            return Vector2.zero;

        return controls.Player.Look.ReadValue<Vector2>();
    }
    public Vector2 GetMovement() => controls.Player.Movement.ReadValue<Vector2>();
}
