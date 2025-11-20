/*
File Name:    AudioFollowCenter.cs
Author(s):    Ju-ve Chankasemporn
*/

using UnityEngine;

public class AudioFollowCenter : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    private Vector3 offset = new Vector3(0, 2f, -5f); // optional, tweak based on arena

    void LateUpdate()
    {
        if (player1 == null || player2 == null)
            return;

        // center point between fighters
        Vector3 center = (player1.position + player2.position) * 0.5f;

        transform.position = center + offset;
        transform.LookAt(center);
    }

    public void SetPlayers(Transform player1, Transform player2) 
    {
        this.player1 = player1;
        this.player2 = player2;
    }
}
