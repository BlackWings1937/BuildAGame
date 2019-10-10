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

    private void saveData() {
        GetController<SceneController>().GetParentController().GetPackageController().SaveData();
    }

    // ----- 对外接口 -----
    public void UpdateData()
    {
        callUpdateEvent();
        saveData();
    }

    public void ParseBgConfigToData(string bgConfigName,SceneNodeData data) {
        var dic = data.MySceneInfoData.PLotData.DicOfNpcsOptions;
        dic.Clear();

        var countOfNpcsNum = 100;
        for (int i = 0;i<countOfNpcsNum;++i) {
            var npcName = "Npc" + i;
            var listOfOptions = new List<NpcOption>();
            dic.Add(npcName,listOfOptions);
        }
        Debug.Log("catch:"+dic.Count);
    }



    public List<string> GetLuaScriptsResList() {
        var l = new List<string>();
        for (int i = 0;i<10;++i) {
            l.Add("PlayMent"+i+".lua");
        }
        return l;
    }
    public List<string> GetProductConfigsList()
    {
        var l = new List<string>();
        for (int i = 0; i < 10; ++i)
        {
            l.Add("ProductConfig" + i + ".json");
        }
        return l;
    }
    public List<string> GetAnimationConfigsList()
    {
        var l = new List<string>();
        for (int i = 0; i < 10; ++i)
        {
            l.Add("AnimationConfig" + i + ".json");
        }
        return l;
    }

    public List<string> GetBgConfigsList() {
        var l = new List<string>();
        for (int i = 0; i < 10; ++i)
        {
            l.Add("BgConfig" + i + ".json");
        }
        return l;
    }
}
