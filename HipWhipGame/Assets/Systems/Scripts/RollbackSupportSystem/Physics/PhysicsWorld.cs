using System.Collections.Generic;
using UnityEngine;

namespace RollbackSupport
{
    public class PhysicsWorld
    {
        public static readonly PhysicsWorld Instance = new PhysicsWorld();
        private readonly List<KinematicBody> bodies = new List<KinematicBody>();
        private const float GravityPerFrame = -0.016f;

        public void Register(KinematicBody body)
        {
            if (!bodies.Contains(body)) bodies.Add(body);
        }

        public void Step()
        {
            foreach (var b in bodies)
            {
                if (b.isKinematic) continue;
                if (b.useGravity) b.velocity.y += GravityPerFrame;
                b.position += b.velocity;

                if (b.position.y < 0)
                {
                    b.position.y = 0;
                    b.velocity.y = 0;
                    b.grounded = true;
                }
                else b.grounded = false;
            }
        }
    }

}