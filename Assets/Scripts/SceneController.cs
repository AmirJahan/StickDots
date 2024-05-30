using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD;
using FMODUnity;
using GameEvents;

public class SceneController : MonoBehaviour
{
    [SerializeField] BoolEventAsset _sceneChangeEvent;
    public void SwitchScene(int sceneID)
    {
        if(sceneID == 3)
        {
            _sceneChangeEvent.Invoke(true);
        }
        SceneManager.LoadScene(sceneID);
    }
}
