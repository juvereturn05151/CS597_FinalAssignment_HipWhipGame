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

        public CollisionBox Pushbox = new CollisionBox
        {
            localCenter = new Vector3(0, 1.0f, 0),
            size = new Vector3(0.6f, 2.0f, 0.6f),
            enabled = true
        };

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
            Hurtboxes.AddBox(new Vector3(0, 1.0f, 0), new Vector3(0.6f, 2.0f, 0.6f));
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (var hb in Hurtboxes.ActiveBoxes)
            {
                Bounds b = hb.ToWorld(transform);
                Gizmos.DrawWireCube(b.center, b.size);
            }

            if (!Pushbox.enabled) 
            {
                return;
            } 

            Gizmos.color = Color.cyan;
            Bounds p = Pushbox.ToWorld(transform);
            Gizmos.DrawWireCube(p.center, p.size);
        }
#endif

    }
}