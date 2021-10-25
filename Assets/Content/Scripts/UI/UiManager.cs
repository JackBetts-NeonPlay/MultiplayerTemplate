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
            GameController.OnGameStateChanged += OnGameStateChanged; 
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; 
            GameController.OnGameStateChanged -= OnGameStateChanged;
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

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.End)
            {
                OpenMenu("Results");
            }
        }

        private void OnLoadingMainMenu()
        {
            OpenMenu("Main");
            SetBackgroundActive(true);
        }
    }
}
