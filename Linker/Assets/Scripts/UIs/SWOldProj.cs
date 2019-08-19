using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SWOldProj : MonoBehaviour {
    // ----- 预制体 -----
    public GameObject myPrefabCell_ = null;
    // ----- 私有成员 -----
    public RectTransform myRtContent_ = null;
    public VerticalLayoutGroup myVLGContent_ = null;
    public CellOldProjsClick myCellClickCb_ = null;
    // ----- 私有方法 -----
    private void onCellClick(Dictionary<string,object> dic) { // call view
        if (myCellClickCb_ != null) {
            //myCellClickCb_(data);
        }
    }
    // ----- 对外接口 -----
    public void UpdateDatas(Dictionary<string,object> dic) {
        /*
        // 计算content长度
        var count = l.Count;
        var contentHeight = count * (myPrefabCell_.GetComponent<RectTransform>().sizeDelta.y + myVLGContent_.spacing) - myVLGContent_.spacing;
        myRtContent_.sizeDelta = new Vector2(myRtContent_.sizeDelta.x,contentHeight);
        // 更新cells
        var countOfOld = myRtContent_.childCount;
        for (int i = 0; i < countOfOld;++i ) {
            Destroy(myRtContent_.GetChild(0).gameObject);
        }

        for (int i = 0; i < count;++i ) {
            var cell = GameObject.Instantiate(myPrefabCell_) as GameObject;
            cell.GetComponent<CellOldProjs>().UpdateData(l[i]);
            cell.GetComponent<CellOldProjs>().SetClickCallBack(this.onCellClick);
            cell.transform.SetParent(myRtContent_,false);
        }
        */
    }
    public void SetCellClickCallBack(CellOldProjsClick cb) { myCellClickCb_ = cb; }
}
