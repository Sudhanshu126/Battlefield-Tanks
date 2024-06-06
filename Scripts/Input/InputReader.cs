using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance { get; private set; }
    public bool isPrimaryFiring { get; private set; }

    public event Action PrimaryFirePerformedEvent;

    private Controls controls;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        controls = new Controls();
        controls.Player.Enable();

        controls.Player.PrimaryFire.performed += PrimaryFire_performed;
        controls.Player.PrimaryFire.canceled += PrimaryFire_canceled;
    }

    private void PrimaryFire_performed(InputAction.CallbackContext obj)
    {
        PrimaryFirePerformedEvent?.Invoke();
        isPrimaryFiring = true;
    }

    private void PrimaryFire_canceled(InputAction.CallbackContext obj)
    {
        isPrimaryFiring = false;
    }

    public Vector2 GetMovementVectorNormalized()
    {
        return controls.Player.Move.ReadValue<Vector2>().normalized;
    }

    public Vector2 GetMousePosition()
    {
        return controls.Player.Aim.ReadValue<Vector2>();
    }
}
