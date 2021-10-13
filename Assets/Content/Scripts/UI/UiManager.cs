using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class UiManager : UiManagerTemplate<UiManager>
    {
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == NetworkManager.Instance.menuSceneName)
            {
                OnLoadingMainMenu();
            }
            else if (scene.name == NetworkManager.Instance.gameSceneName)
            {
                OnLoadingGameScene();
            }
        }

        private void OnLoadingGameScene()
        {
            OpenMenu("Game");
            SetBackgroundActive(false);
        }

        private void OnLoadingMainMenu()
        {
            OpenMenu("Main");
            SetBackgroundActive(true);
        }
    }
}
