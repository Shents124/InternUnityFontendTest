using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecyclingPanelMapViewItem : MonoBehaviour
{
    private RecyclingPanelMapView parentList;
    public RecyclingPanelMapView ParentList {
        get => parentList;
    }

    private int currentRow;
    public int CurrentRow {
        get => currentRow;
    }

    private RectTransform rectTransform;
    public RectTransform RectTransform {
        get {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void NotifyCurrentAssignment(RecyclingPanelMapView v, int row) {
        parentList = v;
        currentRow = row;
    }
    
    
}
