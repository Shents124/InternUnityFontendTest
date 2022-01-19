using UnityEngine;
using UnityEngine.UI;

public class ButtonListControll : RecyclingPanelMapViewItem
{
    public int panelIndex;
    public int panelIndexUnlock;
    public int levelRemainders;

    private PanelData panelData;
    public PanelData PanelData
    {
        get { return panelData; }
        set
        {
            panelData = value;
            panelIndex = panelData.panelIndex;
            panelIndexUnlock = panelData.panelIndexUnlock;
            levelRemainders = panelData.levelRemainders;
            ResetPanel();
            UpdateButton();
        }
    }

    public Button[] buttons;

    public void ResetPanel()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
             LevelButton levelButton = buttons[i].GetComponent<LevelButton>();
             levelButton.LockLevel();
        }   
    }

    public void UpdateButton()
    {
        if (panelIndexUnlock == 0 && panelIndex == 0)
        {
            for (int i = 0; i < levelRemainders; i++)
            {
                LevelButton levelButton = buttons[i].GetComponent<LevelButton>();
                if (i == 0)
                    levelButton.SetTutorialLevel();
                else
                    levelButton.SetLevelNumber((i + 1).ToString());

                levelButton.UnLockLevel();
                levelButton.RandomStar();
            }

            for (int i = levelRemainders; i < buttons.Length; i++)
            {
                LevelButton levelButton = buttons[i].GetComponent<LevelButton>();

                levelButton.SetLevelNumber((i + 1).ToString());
            }
        }

        if (panelIndexUnlock > panelIndex && panelIndexUnlock > 0)
        {
            if (panelIndex == 0)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    LevelButton levelButton = buttons[i].GetComponent<LevelButton>();
                    if (i == 0)
                        levelButton.SetTutorialLevel();
                    else
                        levelButton.SetLevelNumber((i + 1).ToString());

                    levelButton.RandomStar();
                    levelButton.UnLockLevel();
                }
            }
            else
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    LevelButton levelButton = buttons[i].GetComponent<LevelButton>();
                    levelButton.SetLevelNumber((i + 8 * panelIndex + 1).ToString());
                    levelButton.RandomStar();
                    levelButton.UnLockLevel();
                }
            }

        }

        if (panelIndex == panelIndexUnlock && panelIndexUnlock > 0)
        {
            if (levelRemainders > 0)
            {
                for (int i = 0; i < levelRemainders; i++)
                {
                    LevelButton levelButton = buttons[i].GetComponent<LevelButton>();
                    levelButton.SetLevelNumber((i + 8 * panelIndex + 1).ToString());
                    levelButton.RandomStar();
                    levelButton.UnLockLevel();
                }

                for (int i = levelRemainders; i < buttons.Length; i++)
                {
                    LevelButton levelButton = buttons[i].GetComponent<LevelButton>();
                    levelButton.SetLevelNumber((i + 8 * panelIndex + 1).ToString());
                }
            }
            else
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    LevelButton levelButton = buttons[i].GetComponent<LevelButton>();
                    levelButton.SetLevelNumber((i + 8 * panelIndex + 1).ToString());
                }
            }

        }

        if (panelIndex > panelIndexUnlock)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                LevelButton levelButton = buttons[i].GetComponent<LevelButton>();
                levelButton.SetLevelNumber((i + 8 * panelIndex + 1).ToString());
            }
        }
    }

}
