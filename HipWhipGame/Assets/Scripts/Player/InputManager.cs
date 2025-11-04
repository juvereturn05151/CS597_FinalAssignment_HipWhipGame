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
        //if (sumo == null)
        //{
        //    return;
        //}

        //if (context.performed)
        //{
        //    sumo.TrySumoHeadbutt();
        //}
        //else if (context.canceled)
        //{
        //    //Debug.Log("Button South Released!");
        //}
    }

    public void OnButtonWest(InputAction.CallbackContext context)
    {
        //if (sumo == null)
        //{
        //    return;
        //}

        if (context.performed)
        {

        }
        else if (context.canceled)
        {
            //Debug.Log("Button South Released!");
        }
    }

    public void OnButtonNorth(InputAction.CallbackContext context)
    {
        //if (sumo == null)
        //{
        //    return;
        //}

        //if (context.started)
        //{
        //    sumo.OnGrow(true);
        //}
        //else if (context.canceled)
        //{
        //    sumo.OnGrow(false);
        //}
    }

    public void OnButtonSouth(InputAction.CallbackContext context)
    {
        if (fighterController == null)
        {
            return;
        }

        if (context.performed)
        {
            fighterController.PerformButtAttack();
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
