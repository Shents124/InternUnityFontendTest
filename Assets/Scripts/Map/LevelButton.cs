using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public Text levelNumber;
    public GameObject lockImage;
    public GameObject[] stars;
    public int numberOfStars;
    private bool isLock;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(LoadMainLevel);
    }
    private void Start()
    {
        isLock = true;
    }

    public void SetTutorialLevel()
    {
        levelNumber.text = "Tutorial";
        levelNumber.fontSize = 55;
    }
    public void SetLevelNumber(string levelNumberText)
    {
        levelNumber.text = levelNumberText;
    }

    public void UnLockLevel()
    {
        isLock = false;
        lockImage.SetActive(false);
    }

    public void RandomStar()
    {
        numberOfStars = Random.Range(1, 4);
        for (int i = 0; i < numberOfStars; i++)
            stars[i].SetActive(true);
    }

    public void LockLevel()
    {
        isLock = true;
        lockImage.SetActive(true);
        for (int i = 0; i < stars.Length; i++)
            stars[i].SetActive(false);
    }
    private void LoadMainLevel()
    {
        if (isLock == false)
        {
            GameManager.Instance.SetStage(levelNumber.text);
            GameManager.Instance.LoadMainLevel("MainLevel");
        }        
    }
}
