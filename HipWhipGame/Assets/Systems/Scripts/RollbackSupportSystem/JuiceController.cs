using UnityEngine;
using System.Collections.Generic;

namespace RollbackSupport
{
    public class JuiceController : MonoBehaviour
    {
        public static JuiceController Instance;

        [System.Serializable]
        public class JuicePoolItem
        {
            public string name;                 // "FartExplosion"
            public ParticleSystem prefab;       // Prefab reference
            public int preloadAmount = 10;      // Pool size
        }

        [Header("Registered Juice Effects")]
        public List<JuicePoolItem> effects = new List<JuicePoolItem>();

        // Internal dictionary for pooling
        private Dictionary<string, Queue<ParticleSystem>> pool
            = new Dictionary<string, Queue<ParticleSystem>>();

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializePool();
        }

        void InitializePool()
        {
            Debug.Log("JuiceController: Initializing pool...");

            foreach (var item in effects)
            {
                
                if (item.prefab == null)
                {
                    Debug.LogError($"JuiceController: Missing prefab for {item.name}");
                    continue;
                }

                Queue<ParticleSystem> q = new Queue<ParticleSystem>();

                for (int i = 0; i < item.preloadAmount; i++)
                {
                    Debug.Log("is added");
                    var ps = Instantiate(item.prefab, transform);
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    q.Enqueue(ps);
                }

                pool[item.name] = q;
            }
        }

        /// <summary>
        /// Plays an effect from the pool at a world position.
        /// </summary>
        public void Play(string effectName, Vector3 position, Quaternion? rotation = null)
        {
            if (!pool.ContainsKey(effectName))
            {
                Debug.LogWarning($"JuiceController: No effect named '{effectName}'");
                return;
            }

            var q = pool[effectName];
            var ps = q.Dequeue();      // get from pool

            // Position + rotation
            ps.transform.position = position;
            ps.transform.rotation = rotation ?? Quaternion.identity;

            // Reset and play
            ps.Clear();
            ps.Play();

            q.Enqueue(ps);             // return to pool immediately
        }
    }

}
