/*
File Name:    InputManager.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using RollbackSupport;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public int playerIndex;

    private FighterComponentManager fighterComponentManager;

    public void SetFightingComponentManager(FighterComponentManager fighterComponentManager)
    {
        this.fighterComponentManager = fighterComponentManager;
        this.fighterComponentManager.FighterController.SetPlayerIndex(playerIndex);
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
            fighterComponentManager.FighterController.LastInput.swingAttack = true;
        }
        else if (context.canceled)
        {
            fighterComponentManager.FighterController.LastInput.swingAttack = false;
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
            fighterComponentManager.FighterController.LastInput.special = true;
        }
        else if (context.canceled)
        {
            fighterComponentManager.FighterController.LastInput.special = false;
        }
    }



    #region ForMotionInput

    public void OnShakeController()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.light = true;
    }

    public void OnStopShakeController()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.light = false;
    }

    public void OnSwingController()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.grab = true;
    }

    public void OnStopSwingController()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.grab = false;
    }

    public void OnJumpController()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.swingAttack = true;
    }

    public void OnStopJumpController()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.swingAttack = false;
    }


    public void OnShakeYourButt()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.heavy = true;
    }

    public void OnStopShakeButt()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.heavy = false;
    }

    public void OnJump()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.swingAttack = true;
    }

    public void OnStopJump()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.swingAttack = false;
    }

    public void OnWiggleYourButtLeft()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.sidestep = -1.0f;
    }

    public void OnWiggleYourButtRight()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.sidestep = 1.0f;
    }

    public void OnStopWiggleYourButt()
    {
        if (fighterComponentManager == null) return;
        fighterComponentManager.FighterController.LastInput.sidestep = 0;
    }




    #endregion


}
