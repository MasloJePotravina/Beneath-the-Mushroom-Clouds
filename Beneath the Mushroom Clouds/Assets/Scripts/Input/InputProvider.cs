using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// InputProvider is a class which works as a middle man between the new input system and the scripts
/// </summary>
public class InputProvider 
{
    private GameInputActions gameInput = new();

    //Enable action map actions
    public void Enable()
    {
        gameInput.Player.Move.Enable();
        gameInput.Player.Aiming.Enable();
        gameInput.Player.MousePosition.Enable();
    }
    //Disable action map actions
    public void Disable()
    {
        gameInput.Player.Move.Disable();
        gameInput.Player.Aiming.Disable();
        gameInput.Player.MousePosition.Enable();
    }

    //Return movement values
    public Vector2 MoveInput()
    {
        return gameInput.Player.Move.ReadValue<Vector2>();
    }
    public Vector2 MousePosition()
    {
        return gameInput.Player.MousePosition.ReadValue<Vector2>();
    }

    public bool AimingInput()
    {
        return gameInput.Player.Aiming.ReadValue<float>() > 0.1f;
    }

}
