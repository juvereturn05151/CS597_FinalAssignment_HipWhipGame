/*
File Name:    PlayerManager.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Settings")]
    public int maxPlayers = 4;

    public List<PlayerInput> players = new List<PlayerInput>();
    public List<InputDevice> inputDevices = new List<InputDevice>(); // Track each player's paired device

    // ?? Events
    public event Action<PlayerInput> OnPlayerRegistered;
    public event Action<PlayerInput> OnPlayerUnregistered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int PlayerCount => players.Count;

    public bool IsDeviceRegistered(InputDevice device)
    {
        // Debug log all currently registered devices
        Debug.Log($"[Check] Checking device {device.displayName}, ID={device.deviceId}");
        foreach (var d in inputDevices)
        {
            Debug.Log($" -> Registered device: {d.displayName}, ID={d.deviceId}");
        }

        // Check if the deviceId exists in the stored list
        return inputDevices.Exists(d => d.deviceId == device.deviceId);
    }

    public bool CanRegisterMorePlayers() => PlayerCount < maxPlayers;

    public void RegisterPlayer(PlayerInput playerInput)
    {
        if (playerInput == null) return;

        playerInput.gameObject.transform.parent = this.transform;
        players.Add(playerInput);

        playerInput.GetComponent<InputManager>().playerIndex = players.Count;

        OnPlayerRegistered?.Invoke(playerInput);
    }

    public void UnregisterPlayer(PlayerInput playerInput)
    {
        if (playerInput == null) return;

        if (players.Contains(playerInput))
        {
            // Remove device from tracking
            if (playerInput.devices.Count > 0)
            {
                InputDevice device = playerInput.devices[0];
                inputDevices.Remove(device);
                Debug.Log($"[Leave] Device {device.displayName} removed for Player {playerInput.playerIndex}");
            }

            players.Remove(playerInput);

            // ?? Fire event
            OnPlayerUnregistered?.Invoke(playerInput);

            Destroy(playerInput.gameObject);
            Debug.Log($"Player unregistered");
        }
    }

    public void ClearPlayers()
    {
        foreach (var p in players)
        {
            if (p != null)
            {
                // Remove device
                if (p.devices.Count > 0)
                {
                    InputDevice device = p.devices[0];
                    inputDevices.Remove(device);
                    Debug.Log($"[Clear] Device {device.displayName} removed for Player {p.playerIndex}");
                }

                OnPlayerUnregistered?.Invoke(p);
                Destroy(p.gameObject);
            }
        }

        players.Clear();
        inputDevices.Clear();
    }

    public void PlayerShakeController(int i) 
    {
        players[i].GetComponent<InputManager>().OnShakeController();
    }

    public void SetInputManagersEnabled(bool enabled)
    {
        foreach (var player in players)
        {
            if (player == null) continue;

            var inputManager = player.GetComponent<InputManager>();
            if (inputManager != null)
            {
                inputManager.enabled = enabled;
                Debug.Log($"[InputManager] {(enabled ? "Enabled" : "Disabled")} for Player {player.playerIndex}");
            }
            else
            {
                Debug.LogWarning($"[InputManager] Missing on Player {player.playerIndex}");
            }
        }
    }
}
