using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameLevelScrollViewPanelCellSizeInit : MonoBehaviour {
    public GridLayoutGroup gridLayoutGroup;
	// Use this for initialization
	void Start () {
        SetWidthHeightOfGridLayoutGroup();
	}
    void SetWidthHeightOfGridLayoutGroup()
    {
        int totalCellContain = 6;
        int totalCellSize = Screen.width / totalCellContain;
        gridLayoutGroup.cellSize = new Vector2(totalCellSize, totalCellSize + 10);//10 is offset
    }
}
