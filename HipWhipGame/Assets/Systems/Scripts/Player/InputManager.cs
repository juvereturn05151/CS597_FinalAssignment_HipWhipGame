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

    public void OnButtonEast(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }

        if (context.started)
        {
            //fighterComponentManager.FighterInputHandler.PerformButtLowAttack();
        }
        else if (context.canceled)
        {

        }
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
            //fighterComponentManager.FighterInputHandler.PerformPunchFast();
        }
        else if (context.canceled)
        {
            fighterComponentManager.FighterController.LastInput.light = false;
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
            //fighterComponentManager.FighterInputHandler.PerformButtAttackMidPoke();
        }
        else if (context.canceled)
        {
            
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
            //fighterComponentManager.FighterInputHandler.PerformButtAttackHopKick();
        }
        else if (context.canceled)
        {
            fighterComponentManager.FighterController.LastInput.heavy = false;
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
            //fighterComponentManager.FighterInputHandler.PerformButtTornado();
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
            //fighterComponentManager.FighterInputHandler.HoldBlock();
            fighterComponentManager.FighterController.LastInput.block = true;
        }
        else if (context.canceled)
        {
            //fighterComponentManager.FighterInputHandler.ReleaseBlock();
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
            //fighterComponentManager.FighterInputHandler.OnRightStick(context.ReadValue<Vector2>());
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
            fighterComponentManager.FighterController.LastInput.grab = true;
            //fighterComponentManager.FighterInputHandler.PerformTryGrab();
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
