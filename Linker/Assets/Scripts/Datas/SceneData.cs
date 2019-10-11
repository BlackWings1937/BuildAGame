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

        var list = data.MySceneInfoData.PLotData.ListOfNpcOptions;
        list.Clear();

        var countOfNpcsNum = 100;
        for (int i = 0;i<countOfNpcsNum;++i) {
            var ops = new NpcOptions();
            ops.NpcName = "Npc" + i;

            list.Add(ops);
        }
        Debug.Log("catch:"+ list.Count);
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

    public List<string> GetActionsList(string bgConfigName) {
        var l = new List<string>();
        for (int i = 0; i < 10; ++i)
        {
            l.Add("ActionJson" + i + ".json");
        }
        return l;
    }


    /// <summary>
    /// 获取NpcOptions data 通过 npcName
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public NpcOptions GetOptionsByNpcName(string name) {
        var count = data_.MySceneInfoData.PLotData.ListOfNpcOptions.Count;
        for (int i = 0;i<count;++i) {
            var result = data_.MySceneInfoData.PLotData.ListOfNpcOptions[i];
            if (name == result.NpcName) {
                return result;
            }
        }
        return null;
    }

    public void AddJsonNpcOptionOnNpcOption(NpcOptions nops,string actionJsonName) {
        var nop = new NpcOption();
        nop.MyState = NpcOption.State.E_PlayJson;
        nop.Npc = nops.NpcName;
        nop.ExData = actionJsonName;
        nops.listOfOptions.Add(nop);
        UpdateData();
    }

    public void OnRemoveAllChildByNpcOptions(NpcOptions ndatas)
    {
        ndatas.listOfOptions.Clear();
        UpdateData();
    }
}
