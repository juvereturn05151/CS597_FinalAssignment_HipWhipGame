/*
File Name: StageData.cs
Author: Ju-ve Chankasemporn
*/

using UnityEngine;

[CreateAssetMenu(menuName = "Fighter/Stage Data")]
public class StageData : ScriptableObject
{
    public AudioClip stageBGM;

    public string stageName;
    public Sprite stagePreviewImage;
}
