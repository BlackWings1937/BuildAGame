using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : BaseData
{
    // ----- 私有成员 -----
    private SceneNodeData data_ = null;
    // ----- 初始化 -----
    private void initData() {
        var parentController = GetController<SceneController>().GetParentController() as AppController;
        data_ = parentController.GetTargetSceneInfo();
        callUpdateEvent();
    }
    public override void init()
    {
        base.init();
        initData();
    }

    /// <summary>
    /// 更新数据给监听者
    /// </summary>
    private void callUpdateEvent()
    {
        eventOfDataUpdates_(data_);
    }
}
