using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : AppChildController {

    // ----- 对外接口 -----
    public void BackToPackageEditView() {
        DisposeController();
    }
    public void UpdateData() {
        getData<SceneData>().UpdateData();
    }
    public void ParseBgConfigToData(string bgConfigName, SceneNodeData data)
    {
        getData<SceneData>().ParseBgConfigToData(bgConfigName, data);
    }

    public List<string> GetLuaScriptsResList() {
        return getData<SceneData>().GetLuaScriptsResList();
    }
    public List<string> GetProductConfigsList() { return getData<SceneData>().GetProductConfigsList(); }
    public List<string> GetAnimationConfigsList() { return getData<SceneData>().GetAnimationConfigsList(); }
    public List<string> GetBgConfigsList() { return getData<SceneData>().GetBgConfigsList(); }
    public List<string> GetActionsList(string bgConfigName) { return getData<SceneData>().GetActionsList(bgConfigName); }

    /// <summary>
    /// 获取NpcOptions data 通过 npcName
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public NpcOptions GetOptionsByNpcName(string name) {
        return getData<SceneData>().GetOptionsByNpcName(name);
    }

    public void AddJsonNpcOptionOnNpcOption(NpcOptions nops, string actionJsonName) {
        getData<SceneData>().AddJsonNpcOptionOnNpcOption(nops, actionJsonName);
    }

    public void AddJsonNpcOptionOnNpcOptions(NpcOptions nops , string actionJsonName,NpcOption nop) {
        getData<SceneData>().AddJsonNpcOptionOnNpcOptions(nops, actionJsonName, nop);
    }

    public void OnRemoveAllChildByNpcOptions(NpcOptions ndatas)
    {
        getData<SceneData>().OnRemoveAllChildByNpcOptions(ndatas);
    }

    public void OnRemoveAllChildByNpcOptions(NpcOptions ndatas, NpcOption nop) {
        getData<SceneData>().OnRemoveAllChildByNpcOptions(ndatas, nop);
    }

    public void CopyANpcOption(NpcOption op) {
        getData<SceneData>().TmpCopyOptionData = op;
    }

    public NpcOption GetCopyNpcOption() {
        var r = getData<SceneData>().TmpCopyOptionData;
        
        getData<SceneData>().TmpCopyOptionData = null;
        return r;
    }


    public void PasteAtNpcOptions(NpcOptions nops)
    {
        getData<SceneData>().PasteAtNpcOptions(nops);
    }

    public void PasteAtNpcOptions(NpcOptions nops, NpcOption nop)
    {
        getData<SceneData>().PasteAtNpcOptions(nops, nop);
    }

    public bool IsCopyedNpcOption() { return getData<SceneData>().TmpCopyOptionData != null; }

    public void DeleteNpcOption(NpcOptions nops, NpcOption nop) {
        getData<SceneData>().DeleteNpcOption(nops,nop);
    }


    public void AddWaitPointNpcOption(NpcOptions nops) { getData<SceneData>().AddWaitPointNpcOption(nops); }
    public void AddWaitPointNpcOption(NpcOptions nops, NpcOption nop) { getData<SceneData>().AddWaitPointNpcOption(nops, nop); }

    public void AddWaitSoundNpcOption(NpcOptions nops) { getData<SceneData>().AddWaitSoundNpcOption(nops); }
    public void AddWaitSoundNpcOption(NpcOptions nops, NpcOption nop) { getData<SceneData>().AddWaitSoundNpcOption(nops, nop); }

    public void AddWaitShakeNpcOption(NpcOptions nops) { getData<SceneData>().AddWaitShakeNpcOption(nops); }
    public void AddWaitShakeNpcOption(NpcOptions nops, NpcOption nop) { getData<SceneData>().AddWaitShakeNpcOption(nops, nop); }


    public void AddOutputPort(NpcOptions nops) {
        getData<SceneData>().AddOutputPort(nops);
    }

}
