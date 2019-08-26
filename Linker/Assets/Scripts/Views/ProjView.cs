using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ProjView : BaseView {
	
	// ----- const -----
	private static string BTN_N_START_NEW = "BtnStartNew";
    private static string BTN_N_START_OLD = "BtnStartOld";

    // ----- 私有成员 -----
    public RectTransform myRtContent_ = null;
    public VerticalLayoutGroup myGroup_ = null;
    public GameObject myPrefabCell_ = null;
    public ScrollRect myScrollView_ = null;

    // ----- 私有方法 -----

    public override void UpdateView(Dictionary<string, object> info)
    {
        base.UpdateView(info);
        // delete cell
        var childCount = myRtContent_.childCount;
        for (int i = 0;i<childCount;++i) {
            var child = myRtContent_.GetChild(i);
            GameObject.Destroy(child.gameObject);
        }
        // re size
        childCount = info.Count;
        
        var contentHeight =Mathf.Max(myGroup_.padding.top + myGroup_.padding.bottom + ((RectTransform)myPrefabCell_.transform).sizeDelta.y * childCount+ (childCount - 1) * myGroup_.spacing,0);
        myRtContent_.sizeDelta = new Vector2(myRtContent_.sizeDelta.x,contentHeight);
        // create cell
        foreach (var pair in info) {
            var dicInfo = (Dictionary<string, object>)pair.Value;
            var cell = GameObject.Instantiate(myPrefabCell_) as GameObject;
            cell.GetComponent<CellOldProjs>().UpdateData(dicInfo);
            cell.GetComponent<CellOldProjs>().SetClickCallBack((Dictionary<string,object> dic)=> {
                GetController<ProjController>().OnEnterProjByFilePath((string)dic[ProjData.STR_CONFIG_FILE_PATH]);
            });
            cell.transform.SetParent(myRtContent_,false);
        }
    }

    override protected void registerViewEvent(){
		transform.Find(BTN_N_START_NEW).GetComponent<Button>().onClick.AddListener(()=>{ Debug.Log("start new"); GetController<ProjController>().OnCreateNewProj(); });// 
        transform.Find(BTN_N_START_OLD).GetComponent<Button>().onClick.AddListener(()=>{ Debug.Log("start old"); GetController<ProjController>().OnEnterOldProj(); });
	}

    public override void init()
    {
        base.init();
        registerViewEvent();



    }

    // ----- 生命周期方法 -----
}
