/*
File Name:    Enums.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

namespace RollbackSupport
{
    public enum FighterState
    {
        Idle, Walk, Jump, Attack, Block, BlockStun, Hitstun, TryGrab, Disabled, Sidestep,
        Grabbing,
        BeingGrabbed,
        Ultimate,
        Victory,
        Defeat
    }
}