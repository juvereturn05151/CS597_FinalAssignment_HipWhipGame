/*
File Name:    MatchState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System;

namespace RollbackSupport
{
    [Serializable]
    public class MatchState
    {
        public int[] lives;
        public bool isGameOver;
        public int winnerIndex;

        public void Initialize(int lifeCount = 3)
        {
            lives = new int[2];
            lives[0] = lifeCount;
            lives[1] = lifeCount;
            isGameOver = false;
            winnerIndex = -1;
        }

        public MatchState Clone()
        {
            return new MatchState
            {
                lives = (int[])lives.Clone(),
                isGameOver = isGameOver,
                winnerIndex = winnerIndex
            };
        }
    }
}
