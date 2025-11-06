/*
File Name:    Enums.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame
{
    public class Enums : MonoBehaviour
    {
        public enum FighterState { Idle, Moving, Attacking, Hitstun, Knockdown, Jump, Disabled, Blocking, }

        //In case we implement teams later
        //public enum Team { TeamA, TeamB }
        public enum GuardType { Block, LowBLock }
    }
}