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
        if (data_.MySceneInfoData.MyState == SceneInfoData.State.E_PLot) {
            if (data_.MySceneInfoData.PLotData.BgConfigName!= "") {
                initTracks();
            }
        }
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

    private Dictionary<string, List<string>> dicOfTrackUsedNpc_;

    private void initTrack(NpcOptions track) {
        var l = new List<string>();
        var str = track.NpcName;
        dicOfTrackUsedNpc_.Add(str,l);
    }

    private void updateTrack(NpcOptions track) {
        var str = track.NpcName;
        var l = new List<string>();
        dicOfTrackUsedNpc_[str] = l;
        var count = track.listOfOptions.Count;
        for (int i = 0;i<count;++i) {
            var op = track.listOfOptions[i];
            if (op.MyState == NpcOption.State.E_PlayJson) {
                insertNpcsList(l,op);
            }
        }
    }

    private void insertNpcsList(List<string> l,NpcOption op) {
        var jsonList = getNpcNamesByBgConfigName(op.ExData);
        var count = jsonList.Count;
        for (int i = 0;i<count;++i) {
            if (l.Contains(jsonList[i]) == false) {
                l.Add(jsonList[i]);
            } 
        }
    }

     
    private void initTracks() {
        dicOfTrackUsedNpc_ = new Dictionary<string, List<string>>();

        var tracks = data_.MySceneInfoData.PLotData.ListOfNpcOptions;
        var count = tracks.Count;
        for (int i = 0;i<count;++i) {
            initTrack(tracks[i]);
            updateTrack(tracks[i]);
        }
        Debug.Log("catch");
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
        var path = pData_[ProjData.STR_PATH] + "\\Jsons";
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
        var path = pData_[ProjData.STR_PATH] + "\\Jsons\\" + bgConfig;
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

        if (data_.MySceneInfoData.MyState == SceneInfoData.State.E_PLot)
        {
            if (data_.MySceneInfoData.PLotData.BgConfigName != "")
            {
                initTracks();
            }
        }
    }



    public List<string> GetLuaScriptsResList() {

        var path = pData_[ProjData.STR_PATH] + "\\LuaScripts";

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
        var path = pData_[ProjData.STR_PATH] + "\\Others\\ProductConfigs";

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
        var path = pData_[ProjData.STR_PATH] + "\\Others\\AnimatesConfigs";
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
        var path = pData_[ProjData.STR_PATH] + "\\Jsons";
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

    private bool checkJsonCouldAdd (string npcName, string jsonName) {
        var npcListFromJson = getNpcNamesByBgConfigName(jsonName);
        var count = data_.MySceneInfoData.PLotData.ListOfNpcOptions.Count;
        for (int i = 0;i<count;++i) {
            var nops = data_.MySceneInfoData.PLotData.ListOfNpcOptions[i];
            if (nops.NpcName!=npcName) {

                var listUsingNpcName =  dicOfTrackUsedNpc_[nops.NpcName];

                var countOfNewJsonUse = npcListFromJson.Count;
                for (int z = 0;z< countOfNewJsonUse; ++z) {
                    var nowNpcName = npcListFromJson[z];
                    if (listUsingNpcName.Contains(nowNpcName)) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void AddJsonNpcOptionOnNpcOption(NpcOptions nops,string actionJsonName) {
        if (checkJsonCouldAdd(nops.NpcName,actionJsonName)) {
            GetController<SceneController>().GetParentController().ShowAlertView("别的npc轨道中使用了此npc");
            return;
        }
        var nop = new NpcOption();
        nop.MyState = NpcOption.State.E_PlayJson;
        nop.Npc = nops.NpcName;
        nop.ExData = actionJsonName;
        nops.listOfOptions.Add(nop);
        UpdateData();
        updateTrack(nops);
    }

    public void AddJsonNpcOptionOnNpcOptions(NpcOptions nops, string actionJsonName, NpcOption nop) {
        if (nops.listOfOptions.Contains(nop)) {
            if (checkJsonCouldAdd(nops.NpcName, actionJsonName))
            {
                GetController<SceneController>().GetParentController().ShowAlertView("别的npc轨道中使用了此npc");
                return;
            }
            var index = nops.listOfOptions.IndexOf(nop);
            var nopNew = new NpcOption();
            nopNew.MyState = NpcOption.State.E_PlayJson;
            nopNew.Npc = nops.NpcName;
            nopNew.ExData = actionJsonName;
            nops.listOfOptions.Insert(index+1,nopNew);
            UpdateData();
            updateTrack(nops);
        }
    }

    public void OnRemoveAllChildByNpcOptions(NpcOptions ndatas)
    {
        ndatas.listOfOptions.Clear();
        UpdateData();
        updateTrack(ndatas);
    }

    public void OnRemoveAllChildByNpcOptions(NpcOptions ndatas,NpcOption nop) {
        if (ndatas.listOfOptions.Contains(nop)) {
            var index = ndatas.listOfOptions.IndexOf(nop);
            if (index < ndatas.listOfOptions.Count - 1) {
                var count = ndatas.listOfOptions.Count - 1 - index;
                ndatas.listOfOptions.RemoveRange(index+1, count);
                UpdateData();
                updateTrack(ndatas);

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
            updateTrack(nops);

        }
    }

    public void PasteAtNpcOptions(NpcOptions nops, NpcOption nop)
    {
        if (TmpCopyOptionData != null && nops.listOfOptions.Contains(nop))
        {
            if (nop.MyState == NpcOption.State.E_PlayJson) {
                if (checkJsonCouldAdd(nops.NpcName, nop.ExData))
                {
                    GetController<SceneController>().GetParentController().ShowAlertView("别的npc轨道中使用了此npc");
                    return;
                }
            }

            var copyResult = NpcOption.Copy(TmpCopyOptionData);
            var index = nops.listOfOptions.IndexOf(nop);
            nops.listOfOptions.Insert(index+1, copyResult);
            copyResult.Npc = nops.NpcName;
            TmpCopyOptionData = null;
            UpdateData();
            updateTrack(nops);

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
            updateTrack(nops);

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
                var op = new NpcOption();
                op.MyState = NpcOption.State.E_Exit;
                op.Npc = nops.NpcName;
                nops.listOfOptions.Add(op);



                List<string> l = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

                var count = data_.LinkersInfo.Count;
                for (int i = 0;i<count;++i) {
                    var opd = data_.LinkersInfo[i];
                    var countOfl = l.Count;
                    for (int z = 0;z<countOfl;++z) {
                        var bs = opd.BornTimeStamp;
                        if (bs == l[z]) {
                            l.RemoveAt(z);
                            break;
                        }
                    }
                }

                var timeStamp = l[0];
                op.ExData = "出口:" + timeStamp;
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

    public string GetSceneID() {
        return data_.ID;
    }




}
