using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    void Start()
    {
        
    }

    void Update()
    {
        // Press Space to trigger attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Attack");
        }
    }
}
