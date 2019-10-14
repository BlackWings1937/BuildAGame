using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneData : BaseData
{
    // ----- 私有成员 -----
    private SceneNodeData data_ = null;
    private Dictionary<string, object> pData_ = null;

    private NpcOption tmpCopyOptionData_ = null;


    // ----- 属性 -----
    public NpcOption TmpCopyOptionData {
        get { return tmpCopyOptionData_; }
        set { tmpCopyOptionData_ = value; }
    }
    // ----- 初始化 -----
    private void initData() {
        var parentController = GetController<SceneController>().GetParentController() as AppController;
        data_ = parentController.GetTargetSceneInfo();
        pData_ = (GetController<SceneController>().GetParentController() as AppController).GetTargetPackageInfo();
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

    // ----- 私有方法 -----
    private List<string> getBgConfigsByPath(string path) {
        var l = new List<string>();
        DirectoryInfo theFolder = new DirectoryInfo(path);
        //遍历文件
        foreach (FileInfo NextFile in theFolder.GetFiles())
        {
            if (NextFile.Name.Contains("_BgConfig.json"))
            {
                l.Add(NextFile.Name);
            }
        }
        return l;
    }

    private List<string> getActionsByBgConfig(string bgConfig) {
        var index =  bgConfig.IndexOf("_BgConfig.json");
        bgConfig = bgConfig.Remove(index, "_BgConfig.json".Length);
        var l = new List<string>();
        var path = pData_[ProjData.STR_PATH] + "\\json" ;
        DirectoryInfo theFolder = new DirectoryInfo(path);
        foreach (FileInfo NextFile in theFolder.GetFiles())
        {
            if (NextFile.Name.Contains(bgConfig)&& !NextFile.Name.Contains("_BgConfig.json"))
            {
                l.Add(NextFile.Name);
            }
        }
        return l;
    }

    private List<string> getNpcNamesByBgConfigName(string bgConfig) {
        var l = new List<string>();
        var path = pData_[ProjData.STR_PATH] + "\\json\\" + bgConfig;
        if (File.Exists(path)) {
            var str = File.ReadAllText(path);
            AnimationJsonParse p = JsonUtility.FromJson<AnimationJsonParse>(str);
            var count = p.layers.Count;
            for (int i = 0;i<count;++i) {
                if (p.layers[i].layername!= "camera"&& p.layers[i].framestype == "element") {
                    l.Add(p.layers[i].layername);
                }
            }
        }
        return l;
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

        var npcList = getNpcNamesByBgConfigName(bgConfigName);
        var countOfNpcsNum = npcList.Count;
        for (int i = 0;i<countOfNpcsNum;++i) {
            var ops = new NpcOptions();
            ops.NpcName = npcList[i];
            list.Add(ops);
        }
    }



    public List<string> GetLuaScriptsResList() {

        var path = pData_[ProjData.STR_PATH] + "\\luascript";

        var l = new List<string>();
        DirectoryInfo theFolder = new DirectoryInfo(path);
        //遍历文件
        foreach (FileInfo NextFile in theFolder.GetFiles())
        {
            if (NextFile.Name.Contains(".lua"))
            {
                l.Add(NextFile.Name);
            }
        }
        return l;


        /*
        var l = new List<string>();
        for (int i = 0;i<10;++i) {
            l.Add("PlayMent"+i+".lua");
        }*/
       // return l;
    }
    public List<string> GetProductConfigsList()
    {
        var path = pData_[ProjData.STR_PATH] + "\\productconfig";

        var l = new List<string>();
        DirectoryInfo theFolder = new DirectoryInfo(path);
        //遍历文件
        foreach (FileInfo NextFile in theFolder.GetFiles())
        {
            if (NextFile.Name.Contains(".json"))
            {
                l.Add(NextFile.Name);
            }
        }
        return l;
    }
    public List<string> GetAnimationConfigsList()
    {
        var path = pData_[ProjData.STR_PATH] + "\\animconfig";
        var l = new List<string>();
        DirectoryInfo theFolder = new DirectoryInfo(path);
        //遍历文件
        foreach (FileInfo NextFile in theFolder.GetFiles())
        {
            if (NextFile.Name.Contains(".json"))
            {
                l.Add(NextFile.Name);
            }
        }
        return l;
    }

    public List<string> GetBgConfigsList() {
        var path = pData_[ProjData.STR_PATH] + "\\json";
        var l = getBgConfigsByPath(path);
        return l;
    }

    public List<string> GetActionsList(string bgConfigName) {
        var l = new List<string>();

        var lActions = getActionsByBgConfig(bgConfigName);
        for (int i = 0; i < lActions.Count; ++i)
        {
            l.Add(lActions[i]);
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

    public void AddJsonNpcOptionOnNpcOptions(NpcOptions nops, string actionJsonName, NpcOption nop) {
        if (nops.listOfOptions.Contains(nop)) {
            var index = nops.listOfOptions.IndexOf(nop);
            var nopNew = new NpcOption();
            nopNew.MyState = NpcOption.State.E_PlayJson;
            nopNew.Npc = nops.NpcName;
            nopNew.ExData = actionJsonName;
            nops.listOfOptions.Insert(index+1,nopNew);
            UpdateData();
        }
    }

    public void OnRemoveAllChildByNpcOptions(NpcOptions ndatas)
    {
        ndatas.listOfOptions.Clear();
        UpdateData();
    }

    public void OnRemoveAllChildByNpcOptions(NpcOptions ndatas,NpcOption nop) {
        if (ndatas.listOfOptions.Contains(nop)) {
            var index = ndatas.listOfOptions.IndexOf(nop);
            if (index < ndatas.listOfOptions.Count - 1) {
                var count = ndatas.listOfOptions.Count - 1 - index;
                ndatas.listOfOptions.RemoveRange(index+1, count);
                UpdateData();
            } 
        }
    }

    public void PasteAtNpcOptions(NpcOptions nops)
    {
        if (TmpCopyOptionData!=null) {
            var copyResult = NpcOption.Copy(TmpCopyOptionData);
            nops.listOfOptions.Add(copyResult);
            copyResult.Npc = nops.NpcName;
            TmpCopyOptionData = null;
            UpdateData();
        }
    }

    public void PasteAtNpcOptions(NpcOptions nops, NpcOption nop)
    {
        if (TmpCopyOptionData != null && nops.listOfOptions.Contains(nop))
        {
            var copyResult = NpcOption.Copy(TmpCopyOptionData);
            var index = nops.listOfOptions.IndexOf(nop);
            nops.listOfOptions.Insert(index+1, copyResult);
            copyResult.Npc = nops.NpcName;
            TmpCopyOptionData = null;
            UpdateData();
        }
    }

    public void DeleteNpcOption(NpcOptions nops, NpcOption nop) {
        if (nop.MyState == NpcOption.State.E_Exit) {
            RemoveOutputPort(nops);
            return;
        }
        if (nops.listOfOptions.Contains(nop)) {
            nops.listOfOptions.Remove(nop);
            UpdateData();
        }
    }


    public void AddWaitPointNpcOption(NpcOptions nops) {
        var op = new NpcOption();
        op.MyState = NpcOption.State.E_Listen;
        op.Npc = nops.NpcName;
        op.ExData = NpcOption.STR_T_POINT;
        nops.listOfOptions.Add(op);
        UpdateData();
    }
    public void AddWaitPointNpcOption(NpcOptions nops, NpcOption nop) {
        if (nops.listOfOptions.Contains(nop)) {
            var index = nops.listOfOptions.IndexOf(nop);
            var op = new NpcOption();
            op.MyState = NpcOption.State.E_Listen;
            op.Npc = nops.NpcName;
            op.ExData = NpcOption.STR_T_POINT;
            nops.listOfOptions.Insert(index+1,op);
            UpdateData();
        }
    }

    public void AddWaitSoundNpcOption(NpcOptions nops)
    {
        var op = new NpcOption();
        op.MyState = NpcOption.State.E_Listen;
        op.Npc = nops.NpcName;
        op.ExData = NpcOption.STR_T_SOUND;
        nops.listOfOptions.Add(op);
        UpdateData();
    }
    public void AddWaitSoundNpcOption(NpcOptions nops, NpcOption nop)
    {
        if (nops.listOfOptions.Contains(nop))
        {
            var index = nops.listOfOptions.IndexOf(nop);
            var op = new NpcOption();
            op.MyState = NpcOption.State.E_Listen;
            op.Npc = nops.NpcName;
            op.ExData = NpcOption.STR_T_SOUND;
            nops.listOfOptions.Insert(index + 1, op);
            UpdateData();
        }
    }

    public void AddWaitShakeNpcOption(NpcOptions nops)
    {
        var op = new NpcOption();
        op.MyState = NpcOption.State.E_Listen;
        op.Npc = nops.NpcName;
        op.ExData = NpcOption.STR_T_SHAKE;
        nops.listOfOptions.Add(op);
        UpdateData();
    }
    public void AddWaitShakeNpcOption(NpcOptions nops, NpcOption nop)
    {
        if (nops.listOfOptions.Contains(nop))
        {
            var index = nops.listOfOptions.IndexOf(nop);
            var op = new NpcOption();
            op.MyState = NpcOption.State.E_Listen;
            op.Npc = nops.NpcName;
            op.ExData = NpcOption.STR_T_SHAKE;
            nops.listOfOptions.Insert(index + 1, op);
            UpdateData();
        }
    }

    public void AddOutputPort(NpcOptions nops) {
        if (nops.listOfOptions.Count>0) {
            var eop = nops.listOfOptions[nops.listOfOptions.Count - 1];
            if (eop.MyState != NpcOption.State.E_Exit) {
                var timeStamp = TimeUtils.GetTimeStamp();

                var op = new NpcOption();
                op.MyState = NpcOption.State.E_Exit;
                op.Npc = nops.NpcName;
                op.ExData = "出口:"+ timeStamp;
                nops.listOfOptions.Add(op);

                var outputData = new  OutputPortData();
                data_.LinkersInfo.Add(outputData);

                op.BornTimeStamp = timeStamp;
                outputData.BornTimeStamp = timeStamp;

                UpdateData();
            }
        }
    }
    public void RemoveOutputPort(NpcOptions nops) {
        if (nops.listOfOptions.Count > 0) {
            var eop = nops.listOfOptions[nops.listOfOptions.Count - 1];
            if (eop.MyState == NpcOption.State.E_Exit) {
                nops.listOfOptions.RemoveAt(nops.listOfOptions.Count - 1);
                var timeStamp = eop.BornTimeStamp;

                var count = data_.LinkersInfo.Count;
                for (int i = count-1;i>=0;--i) {
                    var d = data_.LinkersInfo[i];
                    if (d.BornTimeStamp == timeStamp) {
                        data_.LinkersInfo.RemoveAt(i);
                    }
                }

                UpdateData();
            }
        }
    }




}
