/*
File Name:    InputManager.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using UnityEngine.InputSystem;
using RollbackSupport;

public class InputManager : MonoBehaviour
{
    public int playerIndex;

    private FighterComponentManager fighterComponentManager;

    public void SetFightingComponentManager(FighterComponentManager fighterComponentManager) 
    {
        this.fighterComponentManager = fighterComponentManager;
        this.fighterComponentManager.FighterController.playerIndex = playerIndex;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }

        fighterComponentManager.FighterController.LastInput.horiz = context.ReadValue<Vector2>().x;
        fighterComponentManager.FighterController.LastInput.vert = context.ReadValue<Vector2>().y;
    }

    public void OnButtonWest(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }

        if (context.performed)
        {
            fighterComponentManager.FighterController.LastInput.light = true;
        }
        else if (context.canceled)
        {
            fighterComponentManager.FighterController.LastInput.light = false;
        }
    }

    public void OnButtonSouth(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }

        if (context.performed)
        {
            fighterComponentManager.FighterController.LastInput.heavy = true;
        }
        else if (context.canceled)
        {
            fighterComponentManager.FighterController.LastInput.heavy = false;
        }
    }

    public void OnButtonEast(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }

        if (context.started)
        {
            fighterComponentManager.FighterController.LastInput.grab = true;
        }
        else if (context.canceled)
        {
            fighterComponentManager.FighterController.LastInput.grab = false;
        }
    }

    public void OnButtonNorth(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }

        if (context.started)
        {
            
        }
        else if (context.canceled)
        {
            
        }
    }


    public void OnRightShoulder(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }

        if (context.performed)
        {

        }
        else if (context.canceled)
        {

        }
    }

    public void OnRightTrigger(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }

        if (context.performed)
        {
            
        }
        else if (context.canceled)
        {
            fighterComponentManager.FighterController.LastInput.block = false;
        }
    }

    public void OnRightStick(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }

        if (context.started) 
        {
            fighterComponentManager.FighterController.LastInput.sidestep = context.ReadValue<Vector2>().x;
        }

    }

    public void OnLeftShoulder(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }
        if (context.performed)
        {
            fighterComponentManager.FighterController.LastInput.block = true;
        }
        else if (context.canceled)
        {
            fighterComponentManager.FighterController.LastInput.block = false;
        }
    }

    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }
        if (context.performed)
        {
            fighterComponentManager.FighterController.LastInput.grab = true;
        }
        else if (context.canceled)
        {
            fighterComponentManager.FighterController.LastInput.grab = false;
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
