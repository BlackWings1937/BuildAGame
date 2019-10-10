using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class NpcOption {
    public const  string STR_WaitForPointCN = "等待点击";
    public const string STR_PlayJsonCN = "播放Json:";
    public const string STR_NoneCN = "空操作";
    public enum State {
        E_None = 0,
        E_PlayJson,
        E_Listen ,
        // todo add listen type
    }
    public static String EToS(State e) {
        var result = STR_NoneCN;
        switch (e) {
            case State.E_Listen:
                result = STR_WaitForPointCN;
                break;
            case State.E_None:
                result = STR_NoneCN;
                break;
            case State.E_PlayJson:
                result = STR_PlayJsonCN;
                break;
        }
        return result;
    }
    public static State  SToE(string s) {
        var result = State.E_None;
        switch (s) {
            case STR_NoneCN:
                result = State.E_None;
                break;
            case STR_PlayJsonCN:
                result = State.E_PlayJson;
                break;
            case STR_WaitForPointCN:
                result = State.E_Listen;
                break;
        }
        return result;
    }
    public State MyState = State.E_None;
    public string Npc;
    public string ExData = "";
    public static NpcOption Copy(NpcOption data) {
        var n = new NpcOption();
        n.MyState = data.MyState;
        n.Npc = data.Npc;
        n.ExData = data.ExData;
        return n;
    }
}

[Serializable]
public class PlotInfoData {
    public string BgConfigName = "";
    public Dictionary<string,List<NpcOption>> DicOfNpcsOptions = new Dictionary<string, List<NpcOption>>();
    public static PlotInfoData Copy(PlotInfoData data) {
        var p = new PlotInfoData();
        p.BgConfigName = data.BgConfigName;
        foreach (var pair in data.DicOfNpcsOptions) {
            var list = new List<NpcOption>();
            var count = pair.Value.Count;
            for (int i = 0;i<count;++i) {
                var nop = NpcOption.Copy(pair.Value[i]);
                list.Add(nop);
            }
            p.DicOfNpcsOptions.Add(pair.Key,list);
        }
        return p;
    }
}

[Serializable]
public class PlaymentInfoData {
    public string LuaScriptName = "";
    public string ProductConfigName = "";
    public string AnimationConfigName = "";

    public static PlaymentInfoData Copy(PlaymentInfoData data) {
        var p = new PlaymentInfoData();
        p.LuaScriptName = data.LuaScriptName;
        p.ProductConfigName = data.ProductConfigName;
        p.AnimationConfigName = data.AnimationConfigName;
        return p;
    }
}

[Serializable]
public class OutputPortData {
    public enum PortState {
        E_Empty = 0,
        E_Full = 1,
    }
    public PortState State = PortState.E_Empty;
    public string SceneNodeID = "-1";
    public static OutputPortData Copy(OutputPortData orignal) {
        var copy = new OutputPortData();
        copy.State = PortState.E_Empty;
        copy.SceneNodeID = "-1";
        return copy;
    }
}

[Serializable]
public class SceneInfoData {
    public enum State {
        E_None = 0,
        E_Playment,
        E_PLot,
    }
    public State MyState = State.E_None;
    public string SceneNodeID;
    public PlaymentInfoData PlayMentData = new PlaymentInfoData();
    public PlotInfoData PLotData = new PlotInfoData();

    public static SceneInfoData Copy(SceneInfoData orignal) {
        var copy = new SceneInfoData();
        copy.MyState = orignal.MyState;
        if (copy.MyState == State.E_Playment) {
            copy.PlayMentData =  PlaymentInfoData.Copy(orignal.PlayMentData);
        }
        else if (copy.MyState == State.E_PLot) {
            copy.PLotData = PlotInfoData.Copy(orignal.PLotData);
        }
        return copy;
    }
}

[Serializable]
public class SceneNodeData {
    public string ID;
    public string Name;
    public float X;
    public float Y;
    public float Z;
    //public List<SceneInfoData> scenesInfo = new List<SceneInfoData>();
    public SceneInfoData MySceneInfoData = null;
    public List<OutputPortData> LinkersInfo = new List<OutputPortData>();

    public static SceneNodeData Copy(SceneNodeData orignal) {
        SceneNodeData copy = new SceneNodeData();
        copy.X = orignal.X;
        copy.Y = orignal.Y;
        copy.Z = orignal.Z;
        copy.Name = orignal.Name;
        copy.ID = TimeUtils.GetTimeStamp();

        copy.MySceneInfoData = SceneInfoData.Copy(orignal.MySceneInfoData);
        copy.MySceneInfoData.SceneNodeID = copy.ID;

        var count = orignal.LinkersInfo.Count;
        for (int i = 0;i<count;++i) {
            var orignalOutputPortData = orignal.LinkersInfo[i];
            var copyResult = OutputPortData.Copy(orignalOutputPortData);
            copy.LinkersInfo.Add(copyResult);
        }
        return copy;
    }
}

[Serializable]
public class PackageInfoData {
    public List<SceneNodeData> ScenesList = new List<SceneNodeData>();
}