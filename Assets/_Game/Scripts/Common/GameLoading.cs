using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Common
{
    public class GameLoading : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single).allowSceneActivation = true;
        }
    }
}