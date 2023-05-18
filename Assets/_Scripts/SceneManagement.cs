using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        // Subscribe to the SceneManager.sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the SceneManager.sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance.LoadBoardAndMap();
        if (SceneManager.GetActiveScene().name == "MapScene")
        {
            GameManager.Instance.CurrentMap.gameObject.SetActive(true);
            GameManager.Instance.MapController.mapCameraLastPos = Camera.main.transform.position;
        }
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            GameManager.Instance.CurrentMap.gameObject.SetActive(false);
            GameManager.Instance.PlayerParty.RefreshParty();
            GameManager.Instance.ProjectilePools.RefreshPools();
        }
    }
}