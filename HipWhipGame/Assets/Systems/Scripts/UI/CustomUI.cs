/*
File Name:    CustomUI.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using ComicUI;

public class CustomUI : MonoBehaviour
{
    [SerializeField]
    private GradientShapeAnim gradientShapeAnim;

    [SerializeField]
    private bool isPlayingAnimationOnStart = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isPlayingAnimationOnStart)
        {
            gradientShapeAnim.OnHoverAction();
            gradientShapeAnim.PlayHoverAnimation();
        }
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
