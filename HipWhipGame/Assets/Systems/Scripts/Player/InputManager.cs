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
        this.fighterComponentManager.Fighter.playerIndex = playerIndex;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (fighterComponentManager == null)
        {
            return;
        }

        fighterComponentManager.Fighter.LastInput.horiz = (sbyte)context.ReadValue<Vector2>().x;
        fighterComponentManager.Fighter.LastInput.vert = (sbyte)context.ReadValue<Vector2>().y;
        //fighterComponentManager.FighterInputHandler.OnMove(context.ReadValue<Vector2>());
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
            fighterComponentManager.Fighter.LastInput.light = true;
            //fighterComponentManager.FighterInputHandler.PerformPunchFast();
        }
        else if (context.canceled)
        {
            fighterComponentManager.Fighter.LastInput.light = false;
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
            //fighterComponentManager.FighterInputHandler.PerformButtAttackHopKick();
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
        }
        else if (context.canceled)
        {
            //fighterComponentManager.FighterInputHandler.ReleaseBlock();
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
            //fighterComponentManager.FighterInputHandler.PerformTryGrab();
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
