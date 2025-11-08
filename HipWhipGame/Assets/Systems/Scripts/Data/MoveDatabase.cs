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
    public MoveData hitStun;
    public MoveData punchFast;
    public MoveData buttAttackHopKick;
    public MoveData buttAttackMidPoke;
    public MoveData buttTornado;
    public MoveData buttLowAttack;
    public MoveData sideStepLeft;
    public MoveData sideStepRight;
    public MoveData tryGrab;

    //public MoveData special;
    //public MoveData jumpAttack;
}
