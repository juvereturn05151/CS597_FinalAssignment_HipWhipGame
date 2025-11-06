/*
File Name:    InputBuffer.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections.Generic;
using UnityEngine;

namespace HipWhipGame
{
    public class InputBuffer : MonoBehaviour
    {
        [System.Serializable]
        public struct BufferedPress
        {
            public string action; // e.g. "Light", "Heavy", "Special", "Jump"
            public float time;
        }

        public float bufferWindow = 0.15f;
        readonly List<BufferedPress> _buffer = new();

        public void Push(string action)
        {
            _buffer.Add(new BufferedPress { action = action, time = Time.time });
        }

        public bool Consume(string action)
        {
            float cutoff = Time.time - bufferWindow;
            for (int i = _buffer.Count - 1; i >= 0; --i)
            {
                if (_buffer[i].action == action && _buffer[i].time >= cutoff)
                {
                    _buffer.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void Prune()
        {
            float cutoff = Time.time - bufferWindow;
            _buffer.RemoveAll(p => p.time < cutoff);
        }
    }
}