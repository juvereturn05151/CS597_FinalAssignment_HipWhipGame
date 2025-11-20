using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField]
    private CharacterSelectController []controller;

    private void Start()
    {
        PlayerManager.Instance.OnPlayerRegistered += OnPlayerJoined;
        PlayerManager.Instance.OnPlayerUnregistered += OnPlayerLeft;
    }

    private void OnDisable()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerRegistered -= OnPlayerJoined;
            PlayerManager.Instance.OnPlayerUnregistered -= OnPlayerLeft;
        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        
        controller[playerInput.GetComponent<InputManager>().playerIndex - 1].Animator.SetBool("CharacterSelected", true);
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {

    }
}
