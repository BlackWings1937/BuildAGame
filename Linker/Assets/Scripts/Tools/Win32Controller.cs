using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public delegate void EventCallBack(object obj);

[Serializable]
public class IPConfig {
    public string IP;
}

[Serializable]
public class MessageCommon
{
    public const string STR_MN_RELOAD_RES = "RELOAD_RES";
    public const string STR_MN_RELOAD_RES_COMPLIE = "RELOAD_RES_COMPLIE";
    public const string STR_MN_START_SCENE = "START_SCENE";
    public const string STR_MN_STOP_SCENE = "STOP_SCENE";
    public const string STR_MN_PLAY_STATUE_CHANGE = "PLAY_STATUE_CHANGE";
    public const string STR_MN_LOAD_RES = "LOAD_RES";
    public const string STR_MN_LOAD_RES_STATUE_UPDATE = "LOAD_RES_STATUE_UPDATE";
    public const string STR_MN_PLAY_SCENE_BY_NPCOPID = "PLAY_SCENE_BY_NPCOPID";
    public const string STR_MN_CELL_HELLO = "CELL_HELLO";
    public const string STR_MN_LINKER_HELLO = "LINKER_HELLO";

    public const string STR_MN_HEART_BEAT = "HEART_BEAT";
    public const string STR_MN_HEART_BEAT_COMPLIE = "HEART_BEAT_COMPLIE";

    public string EventName;
}

[Serializable]
public class MessageHeartBeat : MessageCommon {
    public MessageHeartBeat() {
        EventName = STR_MN_HEART_BEAT;
    }
}

[Serializable]
public class MessageHeartBeatComplie : MessageCommon {
    public MessageHeartBeatComplie() {
        EventName = STR_MN_HEART_BEAT_COMPLIE;
    }
}

[Serializable]
public class MessageCellHello : MessageCommon
{
    public MessageCellHello() {
        EventName = STR_MN_CELL_HELLO;
    }
}

[Serializable]
public class MessageLinkerHello : MessageCommon
{
    public MessageLinkerHello() {
        EventName = STR_MN_LINKER_HELLO;
    }
    public string DeviceName;
}

[Serializable]
public class MessageReload : MessageCommon
{
    public MessageReload()
    {
        EventName = STR_MN_RELOAD_RES;
    }
}

[SerializeField]
public class MessageReloadComplie : MessageCommon
{
    public bool Result;
}

[Serializable]
public class MessagePlayScene : MessageCommon
{
    public MessagePlayScene()
    {
        EventName = STR_MN_START_SCENE;
    }
    public string SceneID;
    public bool IsReloadDragonBoneData;
}

[Serializable]
public class MessageStopScene : MessageCommon
{
    public MessageStopScene()
    {
        EventName = STR_MN_STOP_SCENE;
    }
    public string SceneID;
}

[Serializable]
public class MessageScenePlayStatueChange : MessageCommon 
{
    public MessageScenePlayStatueChange() {
        EventName = STR_MN_PLAY_STATUE_CHANGE;
    }
    public string SceneID;
    public bool IsPlaying;
}

[Serializable]
public class MessageLoadRes : MessageCommon {
    public MessageLoadRes() {
        EventName = STR_MN_LOAD_RES;
    }
}

[Serializable]
public class MessageLoadResStatueChange : MessageCommon {
    public MessageLoadResStatueChange() {
        EventName = STR_MN_LOAD_RES_STATUE_UPDATE;
    }
    public bool IsLoading = false;
}

[Serializable]
public class MessagePlaySceneByIDNpcNameOPIndex :MessageCommon {
    public MessagePlaySceneByIDNpcNameOPIndex() {
        EventName = STR_MN_PLAY_SCENE_BY_NPCOPID;
    }
    public string SceneID;
    public string NpcName;
    public int OptionIndex;
}


public class Win32Controller
{
    private Dictionary<string, EventCallBack> dicOfEvents_ = new Dictionary<string, EventCallBack>();
    private Serv serv_ = null;
    public void Start()
    {
        serv_ = new Serv();
        serv_.Start("192.168.9.48", 1234);
        serv_.SetRecvCallBack((string str) =>
        {
            this.recv(str);
        });
    }
    public void Update()
    {
        serv_.ProcessMessage();
    }
    private object transformMessageObj(string eventName, string strData)
    {
        var obj = new object();
        switch (eventName)
        {
            case MessageCommon.STR_MN_RELOAD_RES_COMPLIE:
                obj = JsonUtility.FromJson<MessageReloadComplie>(strData);
                break;
            case MessageCommon.STR_MN_PLAY_STATUE_CHANGE:
                obj = JsonUtility.FromJson<MessageScenePlayStatueChange>(strData);
                break;
            case MessageCommon.STR_MN_LOAD_RES_STATUE_UPDATE:
                obj = JsonUtility.FromJson<MessageLoadResStatueChange>(strData);
                break;
            case MessageCommon.STR_MN_LINKER_HELLO:
                obj = JsonUtility.FromJson<MessageLinkerHello>(strData);
                break;
            case MessageCommon.STR_MN_HEART_BEAT_COMPLIE:
                obj = JsonUtility.FromJson<MessageHeartBeatComplie>(strData);
                break;
        }
        // todo parse str to aim data
        return obj;
    }
    private void recv(string str)
    {
        Debug.Log("recv1:" + str);
        MessageCommon mc = null;
        try
        {
            mc = JsonUtility.FromJson<MessageCommon>(str);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("messageParse fail" + str);
            return;
        }

        var eventName = mc.EventName;
        if (dicOfEvents_.ContainsKey(eventName))
        {
            var cb = dicOfEvents_[eventName];
            if (cb != null)
            {
                object obj = transformMessageObj(eventName, str);
                cb(obj);
            }
        }

    }


    public void RegisterEvent(string eventName, EventCallBack cb)
    {
        if (dicOfEvents_.ContainsKey(eventName))
        {
            dicOfEvents_[eventName] = cb;
        }
        else
        {
            dicOfEvents_.Add(eventName, cb);
        }
    }

    public void SendMessage(string str)
    {
        if (serv_ != null)
        {
            Debug.Log("serv_.Send:" + str);
            serv_.Send(str);
        }
    }

    public bool IsConnectWin32()
    {
        return false;
    }

    public void Dispose()
    {
        if (serv_ != null)
        {
            serv_.Dispose();
        }
    }

    public void LoadRes() {
        var m = new MessageLoadRes();
        var str = JsonUtility.ToJson(m);
        this.SendMessage(str);
    }
    public void ReloadRes()
    {
        var m = new MessageReload();
        var str = JsonUtility.ToJson(m);
        this.SendMessage(str);
    }

    public void PlayScene()
    {
        var m = new MessagePlayScene();
        m.IsReloadDragonBoneData = false;
        var str = JsonUtility.ToJson(m);
        this.SendMessage(str);
    }
    public void PlaySceneReloadDragonBone() {
        var m = new MessagePlayScene();
        m.IsReloadDragonBoneData = true;
        var str = JsonUtility.ToJson(m);
        this.SendMessage(str);
    }

    public void StopScene()
    {
        var m = new MessageStopScene();
        var str = JsonUtility.ToJson(m);
        this.SendMessage(str);
    }

    public void PlaySceneBySceneIDAndNpcNameAndOptionIndex(string sceneId,string npcName,int optionIdex ) {
        var m = new MessagePlaySceneByIDNpcNameOPIndex();
        m.SceneID = sceneId;
        m.NpcName = npcName;
        m.OptionIndex = optionIdex;
        var str = JsonUtility.ToJson(m);
        Debug.Log("PlaySceneByIndex:"+str);
        this.SendMessage(str);
    }

    public void HeartBeat() {
        var m = new MessageHeartBeat();
        var str = JsonUtility.ToJson(m);
        Debug.Log("HeartBeat:"+str);
        this.SendMessage(str);
    }

    public void HelloCell() {
        var m = new MessageCellHello();
        var str = JsonUtility.ToJson(m);
        this.SendMessage(str);
    }

    public static void GenerateIpConfigFileByPath(string p,string ip) {
        var ipconfig = new IPConfig();
        ipconfig.IP = ip;
        var str = JsonUtility.ToJson(ipconfig);
        File.WriteAllText(p,str);

    }
    public void SetDeviceFailCallBack(SendFailCallBack cb) {
        serv_.SetSendFail(cb);
    }
}
