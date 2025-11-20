/*
File Name:    FighterCollisionComponent.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    [RequireComponent(typeof(FighterComponentManager))]
    public class FighterCollisionComponent : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager fighterComponentManager;

        private bool isPushedThisFrame;
        public bool IsPushedThisFrame
        {
            get => isPushedThisFrame;
            set => isPushedThisFrame = value;
        }

        public HurtboxComponent Hurtboxes = new HurtboxComponent();

        // Capsule-based pushbox (cylinder in XZ)
        public CollisionCapsule PushCapsule = new CollisionCapsule
        {
            localCenter = new Vector3(0, 1.0f, 0),
            radius = 0.4f,
            height = 2.0f,
            enabled = true
        };

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;

            // Simple single hurtbox; you can tweak dimensions later
            Hurtboxes.AddBox(new Vector3(0, 1.0f, 0), new Vector3(0.6f, 2.0f, 0.6f));
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // Draw hurtboxes
            Gizmos.color = Color.green;
            foreach (var hb in Hurtboxes.ActiveBoxes)
            {
                Bounds b = hb.ToWorld(transform);
                Gizmos.DrawWireCube(b.center, b.size);
            }

            // Draw capsule pushbox
            PushCapsule.DrawGizmos(transform, Color.cyan);
        }
#endif
    }
}
