using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class NpcOption {
    //public NpcOption() { timeStamp = TimeUtils.GetTimeStamp(); }
    //public string timeStamp = "0";
    public const  string STR_WaitForPointCN = "等待点击";
    public const string STR_PlayJsonCN = "播放Json:";
    public const string STR_NoneCN = "空操作";

    public const string STR_T_POINT = "STR_T_POINT";
    public const string STR_T_SOUND = "STR_T_SOUND";
    public const string STR_T_SHAKE = "STR_T_SHAKE";


    public enum State {
        E_None = 0,
        E_PlayJson,
        E_Listen ,
        E_Exit,
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
    public string BornTimeStamp = "";
    public static NpcOption Copy(NpcOption data) {
        var n = new NpcOption();
        n.MyState = data.MyState;
        n.Npc = data.Npc;
        n.ExData = data.ExData;
        return n;
    }
}

[Serializable]
public class NpcOptions {
    public string NpcName;
    public List<NpcOption> listOfOptions = new List<NpcOption>();

    public static NpcOptions Copy(NpcOptions orignal) {
        var n = new NpcOptions();
        var count = orignal.listOfOptions.Count;
        for (int i = 0;i<count;++i) {
            var opNow = orignal.listOfOptions[i];
            var opCopy = NpcOption.Copy(opNow);
            n.listOfOptions.Add(opCopy);
        }
        return n;
    }
}


[Serializable]
public class PlotInfoData {
    public string BgConfigName = "";
    public List<NpcOptions> ListOfNpcOptions = new List<NpcOptions>();
    public static PlotInfoData Copy(PlotInfoData data) {
        var p = new PlotInfoData();
        p.BgConfigName = data.BgConfigName;
        var count = data.ListOfNpcOptions.Count;
        for (int i = 0;i<count;++i) {
            var opsNow = data.ListOfNpcOptions[i];
            var opsCopy = NpcOptions.Copy(opsNow);
            p.ListOfNpcOptions.Add(opsCopy);
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
    public string BornTimeStamp = "";
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