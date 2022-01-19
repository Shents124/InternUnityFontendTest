using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text stageText;

    // Start is called before the first frame update
    void Start()
    {
        stageText.text = GameManager.Instance.GetStage();
    }

    public void LoadMenu() => GameManager.Instance.LoadMainLevel("MenuScene");

    public void LoadNextLevel()
    {
        GameManager.Instance.LoadMainLevel("MainLevel");
        int nextStage = int.Parse(stageText.text) + 1;
        GameManager.Instance.SetStage(nextStage.ToString());
    }
}
