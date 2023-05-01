using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoSingleton<InputManager>
{
    public delegate void OnPress();
    public event OnPress onLeftClickPressed = delegate { };
    public event OnPress onLeftClickReleased = delegate { };
    public event OnPress onLeftClickHeld = delegate { };
    public event OnPress onRightClickPressed = delegate { };
    public event OnPress onRightClickReleased = delegate { };
    public event OnPress onRightClickHeld = delegate { };

    private PlayerControls controls;

    protected override void Awake()
    {
        base.Awake();

        controls ??= new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.Player.LeftClick.started += (_) => ClickPerformed(onLeftClickPressed);
        controls.Player.LeftClick.canceled += (_) => ClickPerformed(onLeftClickReleased);

        controls.Player.RightClick.started += (_) => ClickPerformed(onRightClickPressed);
        controls.Player.RightClick.canceled += (_) => ClickPerformed(onRightClickReleased);
    }

    private void OnDisable()
    {
        controls.Player.LeftClick.started -= (_) => ClickPerformed(onLeftClickPressed);
        controls.Player.LeftClick.canceled -= (_) => ClickPerformed(onLeftClickReleased);

        controls.Player.RightClick.started -= (_) => ClickPerformed(onRightClickPressed);
        controls.Player.RightClick.canceled -= (_) => ClickPerformed(onRightClickReleased);

        controls.Disable();
    }

    private void Update()
    {
        if(controls.Player.LeftClick.IsPressed())
        {
            ClickPerformed(onLeftClickHeld);
        }

        if (controls.Player.RightClick.IsPressed())
        {
            ClickPerformed(onRightClickHeld);
        }
    }

    public Vector2 GetLookDelta()
    {
        if (!Application.isPlaying)
            return Vector2.zero;

        if (controls == null || controls.Player.Look == null)
            return Vector2.zero;

        return controls.Player.Look.ReadValue<Vector2>();
    }

    public Vector2 GetMovement()
    {
        return controls.Player.Movement.ReadValue<Vector2>();
    }

    private void ClickPerformed(OnPress callback)
    {
        callback?.Invoke();
    }
}
