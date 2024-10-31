using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static Scene targetScene;

    public enum Scene
    {
        MainMenuScene, GameScene, LoadingScene
    }

    public static void Load(Scene scene)
    {
        SceneLoader.targetScene = scene;
        SceneManager.LoadScene(Scene.LoadingScene + "");
    }

    public static void LoadingSceneCallback()
    {
        SceneManager.LoadScene(SceneLoader.targetScene + "",LoadSceneMode.Single);
    }
}
