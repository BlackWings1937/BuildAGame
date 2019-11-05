using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoseOptionsBox : ChoseOptionBase
{
    public enum NearWayType {
        E_UP,
        E_DOWN,
        E_LEFT,
        E_RIGHT,
    }

    // ----- 属性 -----
    public NearWayType MyNearWayType {
        get { return myNearWayType_; }
        set { myNearWayType_ = value; }
    }

    // ----- 私有成员 -----
    [SerializeField]
    private Text texName_ = null;






    [SerializeField]
    private NearWayType myNearWayType_ = NearWayType.E_LEFT;


    // ----- 私有方法 -----

    private Vector2 caculateSelfNearVector() {
        var rtBox = transform as RectTransform;
        var sizeOfBox = rtBox.sizeDelta;
        var vec = Vector2.zero;
        switch (myNearWayType_) {
            case NearWayType.E_UP:
                vec = new Vector2(0,sizeOfBox.y/2);
                break;
            case NearWayType.E_DOWN:
                vec = new Vector2(0,- sizeOfBox.y / 2);
                break;
            case NearWayType.E_RIGHT:
                vec = new Vector2(sizeOfBox.x/2,0);
                break;
            case NearWayType.E_LEFT:
                vec = new Vector2(-sizeOfBox.x / 2, 0);
                break;
        }
        return vec;
    }

    private Vector2 caculateNearObjNearVector(RectTransform rt) {
        var rtSize = rt.sizeDelta;
        var vec = Vector2.zero;
        switch (myNearWayType_)
        {
            case NearWayType.E_UP:
                vec = new Vector2(0, rtSize.y / 2);
                break;
            case NearWayType.E_DOWN:
                vec = new Vector2(0, -rtSize.y / 2);
                break;
            case NearWayType.E_RIGHT:
                vec = new Vector2(rtSize.x / 2, 0);
                break;
            case NearWayType.E_LEFT:
                vec = new Vector2(-rtSize.x / 2, 0);
                break;
        }
        return vec;
    }

    

    private void updateBoxName(string name) {
        if (texName_ != null) {
            texName_.text = name;
        }
    }

    // ----- 对外接口 -----
    public void UpdateOptionsByDic(Dictionary<string,TapButtonCallBack> dic) { updateOptionsByDic(dic); }
    public void UpdateBoxName(string name) { updateBoxName(name); }


    public void ShowChoseOptionBoxByNameAndDicAndWorldPos(
        string name,
        Dictionary<string,TapButtonCallBack> dic,
        Vector2 wp
        ) {
        this.gameObject.SetActive(true);
        var localPos = TransformUtils.WorldPosToNodePos(wp,transform.parent);
        var nearDistance = caculateSelfNearVector();
        transform.localPosition = new Vector3(localPos.x + nearDistance.x, localPos.y + nearDistance.y, 0);
        updateBoxName(name);
        UpdateOptionsByDic(dic);
    }

    public void ShowChoseOptionBoxByNameAnidDicAndNearObj(
        string name,
        Dictionary<string, TapButtonCallBack> dic,
        RectTransform rt
        ) {
        var pos = rt.position;
        var vec = caculateNearObjNearVector(rt);
        vec = rt.parent.transform.TransformVector(vec);
        ShowChoseOptionBoxByNameAndDicAndWorldPos(
            name,
            dic,
            new Vector2(
                pos.x+vec.x ,pos.y+vec.y
                ));

    }
    public void CloseChoseOptionBox() {
        if (gameObject.activeSelf) {
            gameObject.SetActive(false);
        }
    }
}
