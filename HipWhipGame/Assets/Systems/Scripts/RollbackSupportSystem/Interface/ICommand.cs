/*
File Name:    ICommand.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    public interface ICommand
    {
        string Name { get; }
        void Pressed();

        void Release();

        void UpdateVectorInput(Vector2 updatedVector);

        bool TryExecute();

        void TryStartMove();
    }
}