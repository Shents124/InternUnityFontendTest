using System.Collections.Generic;
using UnityEngine;

public struct PanelData
{
    public int panelIndex;
    public int panelIndexUnlock;
    public int levelRemainders;

    public PanelData(int panelIndex, int panelIndexUnlock, int levelRemainders)
    {
        this.panelIndex = panelIndex;
        this.panelIndexUnlock = panelIndexUnlock;
        this.levelRemainders = levelRemainders;
    }
}

public class ScrollViewController : MonoBehaviour
{
    public RecyclingPanelMapView theList;
    public int numberOfLevel = 640;
    private List<PanelData> data = new List<PanelData>();

    private int numberOfPanel;
    private int lastUnLockLevel;
    private void Start()
    {
        GerenateMap();
    }

    private void RetrieveData()
    {
        data.Clear();

        int panelIndexUnlock = lastUnLockLevel / 8;
        int levelRemainders = lastUnLockLevel - panelIndexUnlock * 8;
        for (int i = 0; i < numberOfPanel; ++i)
        {
            data.Add(new PanelData(i, panelIndexUnlock, levelRemainders));
        }
    }

    private void PopulateItem(RecyclingPanelMapViewItem item, int rowIndex)
    {
        var child = item as ButtonListControll;
        child.PanelData = data[rowIndex];
    }

    public void ResetAllLevel()
    {
        PlayerPrefs.DeleteKey("LastUnLockLevel");
        GerenateMap();
    }

    private void GerenateMap()
    {
        numberOfPanel = numberOfLevel / 8;

        GetLastUnLockLevel();

        theList.ItemCallback = PopulateItem;

        RetrieveData();

        // This will resize the list and cause callbacks to PopulateItem for
        // items that are needed for the view
        theList.RowCount = data.Count;

        theList.ScrollToRow(lastUnLockLevel / 8);
    }

    private void GetLastUnLockLevel()
    {
        if (PlayerPrefs.HasKey("LastUnLockLevel") == false)
        {
            lastUnLockLevel = Random.Range(1, numberOfPanel * 8);
            PlayerPrefs.SetInt("LastUnLockLevel", lastUnLockLevel);
        }
        else
            lastUnLockLevel = PlayerPrefs.GetInt("LastUnLockLevel");
    }
}
