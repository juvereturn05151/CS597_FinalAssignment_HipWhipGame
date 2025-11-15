/*
File Name:    ControllerAssignmentUI.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;   

public class ControllerAssignmentUI : MonoBehaviour
{
    [Header("UI Slots")]
    [SerializeField] private List<PlayerSlotUI> playerSlots = new List<PlayerSlotUI>();

    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    [SerializeField] private GameObject startGamePrompt;
    private void Start()
    {
        // Initialize slots
        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (playerSlots[i].Status != null) 
            {
                playerSlots[i].Status.text = "Press Button to Join";
            }   
        }

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

    private void Update()
    {
        startGamePrompt.SetActive(PlayerManager.Instance.PlayerCount == 2);

        // Detect keyboard join (Enter or Space)
        if (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame))
        {
            LoadGameScene();
        }

        // Detect gamepad join (Start button)
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.startButton.wasPressedThisFrame)
            {
                LoadGameScene();
            }
        }
    }

    private void LoadGameScene()
    {
        if (PlayerManager.Instance.PlayerCount == 2) 
        {
            Debug.Log("Loading game scene...");
            SceneManager.LoadScene(gameSceneName);
        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        int index = PlayerManager.Instance.players.IndexOf(playerInput);
        if (index >= 0 && index < playerSlots.Count)
        {
            playerSlots[index].Status.GetComponent<CustomUI>().StopAnimation();
            playerSlots[index].Status.text = $"Player {index + 1} Ready";
        }
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        // Reset the UI slot
        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (i >= PlayerManager.Instance.PlayerCount)
            {
                playerSlots[i].Status.text = "Press Button to Join";
            }
        }
    }
}
