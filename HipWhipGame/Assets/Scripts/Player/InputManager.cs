/*
File Name:    InputManager.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using UnityEngine.InputSystem;
using HipWhipGame;

public class InputManager : MonoBehaviour
{
    public int playerIndex;
    public FighterController fighterController;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (fighterController == null)
        {
            return;
        }

        fighterController.OnMove(context.ReadValue<Vector2>());
    }

    public void OnButtonEast(InputAction.CallbackContext context)
    {
        if (fighterController == null)
        {
            return;
        }

        if (context.started)
        {
            fighterController.PerformButtLowAttack();
        }
        else if (context.canceled)
        {

        }
    }

    public void OnButtonWest(InputAction.CallbackContext context)
    {
        if (fighterController == null)
        {
            return;
        }

        if (context.performed)
        {
            fighterController.PerformPunchFast();
        }
        else if (context.canceled)
        {
            
        }
    }

    public void OnButtonNorth(InputAction.CallbackContext context)
    {
        if (fighterController == null)
        {
            return;
        }

        if (context.started)
        {
            fighterController.PerformButtAttackMidPoke();
        }
        else if (context.canceled)
        {
            
        }
    }

    public void OnButtonSouth(InputAction.CallbackContext context)
    {
        if (fighterController == null)
        {
            return;
        }

        if (context.performed)
        {
            fighterController.PerformButtAttackHopKick();
        }
        else if (context.canceled)
        {

        }
    }

    public void OnShakeController()
    {
        //if (sumo == null)
        //{
        //    return;
        //}

        //sumo.TryAttack();
    }
}
