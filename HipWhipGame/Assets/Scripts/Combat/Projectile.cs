/*
File Name:    Projectile.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame
{
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        public FighterController owner;
        public MoveData move;
        public float speed = 10f;
        public float lifetime = 3f;

        float _age;

        void Awake()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            _age += Time.deltaTime;
            if (_age >= lifetime) Destroy(gameObject);
        }

        void OnTriggerEnter(Collider other)
        {
            var hurt = other.GetComponent<Hurtbox>();
            if (hurt == null) return;
            if (hurt.owner == owner) return;

            HitResolver.Resolve(owner, hurt.owner, move, transform);
            Destroy(gameObject);
        }
    }
}