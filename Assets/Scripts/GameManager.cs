using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private string stage;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void LoadMainLevel(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void SetStage(string stage)
    {
        this.stage = stage;
    }
    public string GetStage() => stage;
}
