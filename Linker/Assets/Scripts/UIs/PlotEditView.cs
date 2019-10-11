using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotEditView : SceneViewChildView
{

    // ----- 私有成员 -----
    private PlotInfoData data_;

    [SerializeField]
    private Button btnOfChoseInitBgConfg_;

    [SerializeField]
    private GameObject prefabNpcOptionsTrain_;

    [SerializeField]
    private RectTransform rtOfContent_;

    private List<NpcOtionsTrain> listOfItems_ = new List<NpcOtionsTrain>();

    // ----- 私有方法 -----

    private void clearListOfItems() {
        var count = listOfItems_.Count;
        for (int i = 0;i<count;++i) {
            GameObject.Destroy(listOfItems_[i].gameObject);
        }
        listOfItems_.Clear();
    }
    


    private void setChoseBtnInitBgConfig(string title,TapButtonCallBack cb) {
        if (btnOfChoseInitBgConfg_ != null) {
            btnOfChoseInitBgConfg_.onClick.RemoveAllListeners();
            btnOfChoseInitBgConfg_.onClick.AddListener(()=> {
                if (cb!= null) {
                    cb();
                }
            });
            var bat = btnOfChoseInitBgConfg_.GetComponent<BtnAdaptText>();
            if (bat!= null) {
                bat.SetText(title);
            }
        }
    }

    private void updatePlotViewAsEmptyByData() {
        setChoseBtnInitBgConfig("请选择初始化场景的BgConfig",()=> {
            if (MySceneView != null ) {
                MySceneView.ShowInitBgConfgChoseBoxByEditBtn(btnOfChoseInitBgConfg_);
            }
        });
        clearListOfItems();
    }
    private void updatePlotViewAsFillByData() {
        setChoseBtnInitBgConfig(data_.BgConfigName, () =>
        {
            if (MySceneView != null)
            {
                MySceneView.ShowInitBgConfgChoseBoxByEditBtn(btnOfChoseInitBgConfg_);
            }
        });
        clearListOfItems();

        if (prefabNpcOptionsTrain_!= null&& rtOfContent_ != null) {

            var count = data_.ListOfNpcOptions.Count;
            for (int i = 0;i<count;++i) {
                var opsNow = data_.ListOfNpcOptions[i];
                var g = GameObject.Instantiate(prefabNpcOptionsTrain_) as GameObject;
                var trainItem = g.GetComponent<NpcOtionsTrain>();
                trainItem.UpdateByPevAndNpcNameAndOptionLIst(this,opsNow);
                listOfItems_.Add(trainItem);
                trainItem.transform.SetParent(rtOfContent_, false);
            }

            reCaculateRtOfContentSize();
        }
    }

    private void reCaculateRtOfContentSize() {
        reCaculateRtOfContentWidth();
        reCaculateRtOfContentHeight();
    }
    private void reCaculateRtOfContentWidth() {
        var width = 0.0f;
        var count = listOfItems_.Count;
        for (int i = 0;i<count;++i) {
            var trainItem = listOfItems_[i];
            var nw = (trainItem.transform as RectTransform).sizeDelta.x;
            if (nw > width) {
                width = nw;
            }
        }

        var size = rtOfContent_.sizeDelta;
        rtOfContent_.sizeDelta = new Vector2(width,size.y);
    }
    private void reCaculateRtOfContentHeight() {
        var height = 0.0f;
        var verticalGroup = rtOfContent_.GetComponent<VerticalLayoutGroup>();
        var countOfItems = listOfItems_.Count;
        height = height + verticalGroup.padding.top + verticalGroup.padding.bottom +
            (Mathf.Max(0, countOfItems - 1)) * verticalGroup.spacing;
        for (int i = 0;i<countOfItems;++i) {
            var nh = (listOfItems_[i].transform as RectTransform).sizeDelta.y;
            height += nh;
        }
        var size = rtOfContent_.sizeDelta;
        rtOfContent_.sizeDelta = new Vector2(size.x, height);
    }

    // ----- 对外接口 -----
    public void UpdateByData(PlotInfoData data) {
        data_ = data;
        if (data_!= null) {
            Debug.Log("plot update do sth");
            if (data_.BgConfigName == "")
            {
                updatePlotViewAsEmptyByData();
            }
            else {
                updatePlotViewAsFillByData();
            }
        }
    }


    public void OnBtnClickAtNpcItem(RectTransform rt,NpcOptions data) {
        if (MySceneView != null) {
            MySceneView.ShowOptionsByUserClickNpcItem(rt, data);
        }
    }
    public void OnBtnClickAtOptionItem(RectTransform rt,NpcOption nop) {
        if (MySceneView!= null) {
            MySceneView.ShowOptionsByUserClickOptionItem(rt,nop);
        }
    }

}
