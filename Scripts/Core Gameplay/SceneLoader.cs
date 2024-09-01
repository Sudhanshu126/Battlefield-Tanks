using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public event Action onSceneLoaded;

    [SerializeField] private GameObject loadingScreen;

    public void LoadScene(SceneCode sceneCode)
    {
        StartCoroutine(LoadSceneAsync((int)sceneCode));
    }

    private IEnumerator LoadSceneAsync(int sceneCode)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneCode);
        operation.completed += SceneLoaded;
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    private void SceneLoaded(AsyncOperation operation)
    {
        onSceneLoaded?.Invoke();
        Debug.LogWarning("New Scene Loaded");
    }
}
