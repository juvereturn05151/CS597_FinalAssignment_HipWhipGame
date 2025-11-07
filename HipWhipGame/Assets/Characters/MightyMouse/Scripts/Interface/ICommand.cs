/*
File Name:    ICommand.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame
{
    public interface ICommand
    {
        string Name { get; }
        void Execute();

        void Release();

        void UpdateVectorInput(Vector2 updatedVector);
    }
}