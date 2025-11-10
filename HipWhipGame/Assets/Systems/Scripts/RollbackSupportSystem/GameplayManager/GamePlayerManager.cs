/*
File Name:    GamePlayerManager.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RollbackSupport
{
    public class GamePlayerManager : MonoBehaviour
    {
        public static GamePlayerManager Instance { get; private set; }

        [Header("References")]
        public GameObject playerPrefab;
        public GameObject dummyPrefab;
        public Transform[] spawnPoints;
        public GameSimulation simulation;

        private List<GameObject> activePlayers = new List<GameObject>();
        public List<GameObject> ActivePlayers => activePlayers;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            SpawnCharacters();
            AdjustSplitScreen();
        }

        private void SpawnCharacters()
        {
            // Clear existing players
            foreach (var player in activePlayers)
            {
                if (player != null)
                    Destroy(player);
            }
            activePlayers.Clear();

            // Spawn registered players
            foreach (var playerInfo in PlayerManager.Instance.players)
            {
                if (playerInfo.playerIndex >= spawnPoints.Length)
                    continue;

                SpawnPlayer(playerInfo);
            }

            // If only one player, spawn a dummy opponent
            if (PlayerManager.Instance.players.Count <= 1)
            {
                SpawnDummy(1);
            }
        }

        private void SpawnPlayer(PlayerInput playerInput)
        {
            int index = playerInput.playerIndex;

            if (index >= spawnPoints.Length)
                return;

            Transform spawn = spawnPoints[index];
            GameObject playerObj = Instantiate(playerPrefab, spawn.position, spawn.rotation);

            FighterComponentManager fcm = playerObj.GetComponent<FighterComponentManager>();
            var inputManager = playerInput.GetComponent<InputManager>();
            if (inputManager != null)
            {
                inputManager.SetFightingComponentManager(fcm);
            }

            fcm.Fighter.body.Teleport(spawn.position);

            //Hack
            if (fcm.Fighter.playerIndex == 2)
            {
                fcm.Fighter.lookAtTarget = activePlayers[0].transform;
                activePlayers[0].GetComponent<Fighter>().lookAtTarget = fcm.Fighter.transform;
                simulation.fighter1 = activePlayers[0].GetComponent<Fighter>();
                simulation.fighter2 = fcm.Fighter;
                simulation.Initialize();
            }

            activePlayers.Add(playerObj);
        }

        private void SpawnDummy(int index)
        {
            if (index >= spawnPoints.Length)
                return;

            Transform spawn = spawnPoints[index];
            GameObject dummyObj = Instantiate(dummyPrefab, spawn.position, spawn.rotation);
            activePlayers.Add(dummyObj);
        }

        //SPLIT SCREEN CAMERA SETUP
        private void AdjustSplitScreen()
        {
            // Guard: no players
            if (activePlayers.Count == 0)
                return;

            // Iterate all players
            for (int i = 0; i < activePlayers.Count; i++)
            {
                var fighter = activePlayers[i].GetComponent<FighterComponentManager>();
                if (fighter == null || fighter.Cam == null)
                    continue;

                Camera cam = fighter.Cam;
                cam.enabled = true;

                // Disable extra AudioListeners (keep only one)
                AudioListener listener = cam.GetComponent<AudioListener>();
                if (i == 0)
                {
                    if (listener != null)
                        listener.enabled = true;
                }
                else
                {
                    if (listener != null)
                        listener.enabled = false;
                }

                // Assign split-screen viewports
                // Vertical split (Player1 left, Player2 right)
                if (activePlayers.Count == 2)
                {
                    if (i == 0)
                        cam.rect = new Rect(0f, 0f, 0.5f, 1f);
                    else
                        cam.rect = new Rect(0.5f, 0f, 0.5f, 1f);
                }
                // Top/Bottom split example (for 2 or more)
                else if (activePlayers.Count > 2)
                {
                    // You can extend logic for 4-player grid here
                    float width = 0.5f;
                    float height = 0.5f;
                    float x = (i % 2) * width;
                    float y = (i / 2) * height;
                    cam.rect = new Rect(x, y, width, height);
                }
                else
                {
                    // Single player full screen
                    cam.rect = new Rect(0f, 0f, 1f, 1f);
                }
            }
        }
    }
}