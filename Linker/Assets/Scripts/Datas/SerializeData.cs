using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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
    public static SceneInfoData Copy(SceneInfoData orignal) {
        var copy = new SceneInfoData();
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
    public List<SceneInfoData> scenesInfo = new List<SceneInfoData>();
    public List<OutputPortData> LinkersInfo = new List<OutputPortData>();

    public static SceneNodeData Copy(SceneNodeData orignal) {
        SceneNodeData copy = new SceneNodeData();
        copy.X = orignal.X;
        copy.Y = orignal.Y;
        copy.Z = orignal.Z;
        copy.Name = orignal.Name;
        copy.ID = TimeUtils.GetTimeStamp();
        var count = orignal.scenesInfo.Count;
        for (int i = 0;i<count;++i) {
            var orignalSceneInfo = orignal.scenesInfo[i];
            var copyResult = SceneInfoData.Copy(orignalSceneInfo);
            copy.scenesInfo.Add(copyResult);
        }
        count = orignal.LinkersInfo.Count;
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