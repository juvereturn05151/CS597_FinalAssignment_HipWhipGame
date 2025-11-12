using System.Collections.Generic;
using UnityEngine;

namespace RollbackSupport
{
    public class PhysicsWorld
    {
        public static readonly PhysicsWorld Instance = new PhysicsWorld();
        private readonly List<KinematicBody> bodies = new List<KinematicBody>();
        private const float GravityPerFrame = -0.016f;

        private StageBounds bounds = new StageBounds
        {
            min = new Vector3(-12.5f, -0.125f, -12.5f),
            max = new Vector3(12.5f, 5.375f, 12.5f)
        };

        public void SetStageBounds(StageBounds newBounds)
        {
            bounds = newBounds;
        }

        public void Register(KinematicBody body)
        {
            if (!bodies.Contains(body)) 
            {
                bodies.Add(body);
            } 
        }

        public void Step()
        {
            foreach (var b in bodies)
            {


                if (b.isKinematic) 
                {
                    continue;
                }
                if (b.useGravity) 
                {
                    b.velocity.y += GravityPerFrame;
                }

                b.position += b.velocity;

                if (bounds.IsOutside(b.position))
                {
                    //Debug.Log("Is Outside");
                }
                else 
                {
                    if (b.position.y < 0)
                    {
                        b.position.y = 0;
                        b.velocity.y = 0;
                        b.grounded = true;
                    }
                    else
                    {
                        b.grounded = false;
                    }
                }
            }
        }


    }

}