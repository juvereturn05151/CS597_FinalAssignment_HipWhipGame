/*
File Name: BGMPlayer.cs
Author: Ju-ve Chankasemporn
*/


using RollbackSupport;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public StageData stageData;

    private void Start()
    {
        if (stageData != null && stageData.stageBGM != null)
        {
            AudioSystem.Instance.PlayBGM(stageData.stageBGM);
        }
    }
}
