/*
File Name:    Hitbox.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame
{
    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour
    {
        public FighterController Owner { get; private set; }
        public MoveData Move { get; private set; }
        public bool IsActive { get; private set; }

        int _lifetimeFrames;
        float _age;

        Collider _col;

        public void Init(FighterController owner, MoveData move)
        {
            Owner = owner;
            Move = move;
            _col = GetComponent<Collider>();
            _col.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Hitbox");
            IsActive = false;
            _age = 0f;
        }

        public void Activate() => IsActive = true;
        public void SetLifetimeFrames(int frames) => _lifetimeFrames = Mathf.Max(1, frames);

        void Update()
        {
            if (!IsActive) return;
            _age += Time.deltaTime;
            if (_age >= _lifetimeFrames / 60f) Destroy(gameObject);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!IsActive || Owner == null || Move == null) return;

            var hurt = other.GetComponent<Hurtbox>();
            if (hurt == null) return;
            if (hurt.owner == Owner) return; // don't hit self
            //if (hurt.owner.team == Owner.team) return; // friendly-fire off

            // Resolve hit
            HitResolver.Resolve(Owner, hurt.owner, Move, transform);

            //Debug.Log($"{Owner.name} hit {hurt.owner.name} with {Move.moveName}");
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (GetComponent<Collider>())
                Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
        }
    }
}