/*
File Name:    MoveDatabase.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using HipWhipGame;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveDatabase", menuName = "FightingGame/MoveDatabase")]
public class MoveDatabase : ScriptableObject
{
    public MoveData idle;    // optional, for reference
    public MoveData buttAttack;
    public MoveData hitStun;
    //public MoveData light;
    //public MoveData heavy;
    //public MoveData special;
    //public MoveData jumpAttack;
}
