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
    public int SceneNodeID = -1;
}

[Serializable]
public class SceneInfoData {

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
}

[Serializable]
public class PackageInfoData {
    public List<SceneNodeData> ScenesList = new List<SceneNodeData>();
}