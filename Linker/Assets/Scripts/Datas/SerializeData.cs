using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AnimationJsonLayerParse {
    public string layername;
    public string framestype;
}

[Serializable]
public class AnimationJsonParse {
    public List<AnimationJsonLayerParse> layers;
}


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
        n.BornTimeStamp = data.BornTimeStamp;
        return n;
    }

    public void InitTmpData() { }
}

[Serializable]
public class NpcOptions {
    public string NpcName;
    public List<NpcOption> listOfOptions = new List<NpcOption>();


    public void InitTmpData() {
        var count = listOfOptions.Count;
        for (int i = 0; i < count; ++i)
        {
            var l = listOfOptions[i];
            l.InitTmpData();
        }
    }

    public static NpcOptions Copy(NpcOptions orignal) {
        var n = new NpcOptions();
        n.NpcName = orignal.NpcName;
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

    public void InitTmpData() {
        var count = ListOfNpcOptions.Count;
        for (int i = 0; i < count; ++i)
        {
            var l = ListOfNpcOptions[i];
            l.InitTmpData();
        }
    }
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

    public void InitTmpData() { }
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
    public int readNum_;

    public void InitTmpData() {

    }
    public static OutputPortData Copy(OutputPortData orignal) {
        var copy = new OutputPortData();
        copy.State = PortState.E_Empty;
        copy.SceneNodeID = "-1";
        copy.BornTimeStamp = orignal.BornTimeStamp;
        copy.readNum_ = orignal.readNum_;
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

    public void InitTmpData() {
        PlayMentData.InitTmpData();
        PLotData.InitTmpData();
    }
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

    public void InitTmpData() {
        this.IsPlaying = false;
        if (MySceneInfoData !=null) {
            MySceneInfoData.InitTmpData();
        }
        var count = LinkersInfo.Count;
        for (int i = 0; i < count; ++i)
        {
            var ll = LinkersInfo[i];
            ll.InitTmpData();
        }
    }

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

    //runningTime tmp
    public bool IsPlaying = false;
}

/*
 var count = ScenesList.Count;
 for (int i = 0;i<count;++i) {

 }
*/
[Serializable]
public class PackageInfoData {
    public void InitTmpData() {
        var count = ScenesList.Count;
        for (int i = 0;i<count;++i) {
            var d = ScenesList[i];
            d.InitTmpData();
        }
    }
    public string StartSceneID = "";
    public List<SceneNodeData> ScenesList = new List<SceneNodeData>();
}


[Serializable]
public class SNpcOption {
    public int MyState =0;
    public string Npc;
    public string ExData = "";
    public string BornTimeStamp = "";
    public SNpcOption(NpcOption d) {
        MyState = (int)d.MyState;
        Npc = d.Npc;
        ExData = d.ExData;
        BornTimeStamp = d.BornTimeStamp;
    }
}
[Serializable]
public class SNpcOptions {
    public string NpcName;
    public List<SNpcOption> listOfOptions = new List<SNpcOption>();
    public SNpcOptions(NpcOptions d) {
        NpcName = d.NpcName;
        var count = d.listOfOptions.Count;
        for (int i = 0;i<count;++i) {
            var snpo = new SNpcOption(d.listOfOptions[i]);
            listOfOptions.Add(snpo);
        }
    }
}
[Serializable]
public class SPlotInfoData {
    public string BgConfigName = "";
    public List<SNpcOptions> ListOfNpcOptions = new List<SNpcOptions>();
    public SPlotInfoData(PlotInfoData d) {
        BgConfigName = d.BgConfigName;
        var count = d.ListOfNpcOptions.Count;
        for (int i = 0; i < count; ++i)
        {
            var snpo = new SNpcOptions(d.ListOfNpcOptions[i]);
            ListOfNpcOptions.Add(snpo);
        }
    }
}
[Serializable]
public class SPlaymentInfoData {
    public string LuaScriptName = "";
    public string ProductConfigName = "";
    public string AnimationConfigName = "";

    public SPlaymentInfoData(PlaymentInfoData d) {
        LuaScriptName = d.LuaScriptName;
        ProductConfigName = d.ProductConfigName;
        AnimationConfigName = d.AnimationConfigName;
    }
}
[Serializable]
public class SOutputPortData {
    public int  State = 0;
    public string SceneNodeID = "-1";
    public string BornTimeStamp = "";
    public int readNum_;
    public SOutputPortData(OutputPortData d) {
        State = (int)d.State;
        SceneNodeID = d.SceneNodeID;
        BornTimeStamp = d.BornTimeStamp;
        readNum_ = d.readNum_;
    }
}
[Serializable]
public class SSceneInfoData {
    public int MyState = 0;
    public string SceneNodeID;
    public SPlaymentInfoData PlayMentData;
    public SPlotInfoData PLotData;
    public SSceneInfoData(SceneInfoData d) {
        MyState = (int)d.MyState;
        SceneNodeID = d.SceneNodeID;
        PlayMentData = new SPlaymentInfoData(d.PlayMentData);
        PLotData = new SPlotInfoData(d.PLotData);
    }
}
[Serializable]
public class SSceneNodeData {
    public string ID;
    public string Name;
    public SSceneInfoData MySceneInfoData = null;
    public List<SOutputPortData> LinkersInfo = new List<SOutputPortData>();

    public SSceneNodeData(SceneNodeData d) {
        ID = d.ID;
        Name = d.Name;
        MySceneInfoData = new SSceneInfoData(d.MySceneInfoData);
        var count = d.LinkersInfo.Count;
        for (int i = 0;i<count;++i) {
            LinkersInfo.Add(new SOutputPortData(d.LinkersInfo[i]));
        }
    }
}
[Serializable]
public class SPackageInfoData {
    public List<SSceneNodeData> ScenesList = new List<SSceneNodeData>();
    public SPackageInfoData(PackageInfoData d) {
        var count = d.ScenesList.Count;
        StartSceneID = d.StartSceneID;
        for (int i = 0; i < count; ++i)
        {
            ScenesList.Add(new SSceneNodeData(d.ScenesList[i]));
        }
    }

    public string StartSceneID = "";
}
