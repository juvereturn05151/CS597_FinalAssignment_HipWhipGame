/*
File Name:    PlayerSlotUI.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using TMPro;
using UnityEngine;

public class PlayerSlotUI: MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI status;   // "Press Button to Join" / "Player X Joined"
    public TextMeshProUGUI Status => status;

}
