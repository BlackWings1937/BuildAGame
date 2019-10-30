using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public delegate void EventCallBack(object obj);

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
    public string EventName;
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

    public void PlayScene(string sceneId)
    {
        var m = new MessagePlayScene();
        m.SceneID = sceneId;
        var str = JsonUtility.ToJson(m);
        this.SendMessage(str);
    }


    public void StopSceneByID(string sceneId)
    {
        var m = new MessageStopScene();
        m.SceneID = sceneId;
        var str = JsonUtility.ToJson(m);
        this.SendMessage(str);
    }

}
