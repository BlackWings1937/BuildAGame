using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneView : BaseView
{
    // ----- 私有成员 -----
    [SerializeField]
    private Button btnBack_ = null;


    // ----- 初始化 -----
    public override void dispose()
    {
        btnBack_.onClick.RemoveAllListeners();
    }
    protected override void registerViewEvent()
    {
        base.registerViewEvent();
        if (btnBack_ != null) {
            btnBack_.onClick.AddListener(()=> {
                // todo back to package edit 
                GetController<SceneController>().BackToPackageEditView();
            });
        }
    }

    public override void init()
    {
        registerViewEvent();
    }

    public override void UpdateView(object info)
    {
        Debug.Log("sceneView update view");
        var data = info as SceneNodeData;
        Debug.Log("data:"+data.MySceneInfoData.SceneNodeID);
        Debug.Log("data:"+data.MySceneInfoData.MyState);
    }
}
