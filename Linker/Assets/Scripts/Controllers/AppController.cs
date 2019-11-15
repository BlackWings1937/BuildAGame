
#define FORMAT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public delegate void ReloadComplieCallBack(bool result);

public class AppController : BaseController {

    public static Vector2 VISIBLE_SIZE = new Vector2(1280,720);
    // ----- 私有成员 ----- 
    public AppChildController myProjController_ = null;
    public AppChildController myPackageController_ = null;
    public AppChildController mySceneController_ = null;

    [SerializeField]
    private AlertView myAlertView_ = null;


    private ReloadComplieCallBack cbOfReloadComplie_ = null;

    private Win32Controller win32Controller_ = null;
    private HttpManager httpManager_ = null;

    private bool isHeartBeatComplie_ = true; // 心跳是否返回


    private void heartBeatComplie() {
        isHeartBeatComplie_ = true;
        UnityEngine.Debug.Log("appcontroller heartbeat complie");
    }
    public void HeartBeat() {
        UnityEngine.Debug.Log("heart beat appcontroller");
        if (isHeartBeatComplie_ == true)
        {
            isHeartBeatComplie_ = false;
            if (this.win32Controller_ != null) {
                UnityEngine.Debug.Log("appcontroller heartbeatStart");
                this.win32Controller_.HeartBeat();
            }
        }
        else {
            if (GetPackageController().GetDeviceState() == PackageData.EnumLinkerDeviceStatue.E_Cell) {
                GetPackageController().StopCell();
            }
            else if (GetPackageController().GetDeviceState() == PackageData.EnumLinkerDeviceStatue.E_Win32) {
                GetPackageController().StopWin32();
            }
        }
    }

    private void startHeartBeat() {
        UnityEngine.Debug.Log("HeartBeat start");

        InvokeRepeating("HeartBeat",2,2);
    }
    private void stopHeartBeat() {
        UnityEngine.Debug.Log("HeartBeat stop");
        CancelInvoke();
        //CancelInvoke("HeartBeat");
    }

    // ----- 生命周期 -----
    protected override void Start()
    {
        //debug
        //IPHostEntry localhost = Dns.GetHostByName(Dns.GetHostName());    //方法已过期，可以获取IPv4的地址
        //IPAddress localaddr = localhost.AddressList[2];
        
       // UnityEngine.Debug.Log("hostName:"+ localaddr.ToString());

        // debug 
        DateTime dt = File.GetLastWriteTime("D:\\test2.png");
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
        long timeStamp = (long)(dt - startTime).TotalMilliseconds; // 相差毫秒数
        UnityEngine.Debug.Log("timeStamp:"+timeStamp);
        // debug
        httpManager_ = new HttpManager();
        httpManager_.SetAppController(this);
        httpManager_.Start();

        VISIBLE_SIZE = new Vector2(((RectTransform)transform).sizeDelta.x, ((RectTransform)transform).sizeDelta.y);

        base.Start();
        init();
        getView<AppView>().InitView();
        getData<AppData>().init();
        initChildSys();
        activeProjSys();

        //设置回调事件
        myProjController_.MyDispearCallBack = () => { activePackageSys(); };
        myPackageController_.MyDispearCallBack = () => { activeSceneSys(); };
        myPackageController_.MyDisposeCallBack = ()=> { wakeupProjSys(); };
        mySceneController_.MyDisposeCallBack = () => { wakeupPackageSys(); };
    }

    private void Update()
    {
        if (win32Controller_!= null) {
            win32Controller_.Update();
        }
    }
    // ----- 私有方法 -----

    private void initChildSys() { myPackageController_.SetParentController(this);myProjController_.SetParentController(this);mySceneController_.SetParentController(this); }

    private void activeProjSys() {myProjController_.ActiveController();}
    private void wakeupProjSys() { myProjController_.WakeUpController(); }
    private void dispearProjSys() { myProjController_.DispearController(); }
    private void disposeProjSys() { myProjController_.DisposeController(); }

    private void activePackageSys() { myPackageController_.ActiveController(); }
    private void wakeupPackageSys() { myPackageController_.WakeUpController(); }
    private void dispearPackageSys() { myPackageController_.DispearController(); }
    private void disposePackageSys() { myPackageController_.DisposeController(); }

    private void activeSceneSys() { mySceneController_.ActiveController(); }
    private void wakeupSceneSys() { mySceneController_.WakeUpController(); }
    private void dispearSceneSys() { mySceneController_.DispearController(); }
    private void disposeSceneSys() { mySceneController_.DisposeController(); }



    private void setReloadComplieCallBack(ReloadComplieCallBack cb) {
        cbOfReloadComplie_ = cb;
    }
    private void shutDownReloadComplieCallBack() {
        cbOfReloadComplie_ = null;
    }

    private Process processOfWin32exe_;
    private void startWin32Process() {
        if (processOfWin32exe_ == null)
        {
#if FORMAT
            var path = GetWin32ProjPath() + "\\xiaobanlong\\xiaobanlong5.2.0\\proj.win32\\Debug.win32\\xiaobanlong.exe";
            processOfWin32exe_ = Process.Start(path);
#endif
        }
        else
        {
         /*
         if (System.Diagnostics.Process.GetProcessesByName(processOfWin32exe_.ProcessName).Length>1)
         {
             UnityEngine.Debug.Log("win32 exist");
         }
         else {
             UnityEngine.Debug.Log("win32 not exist");
             processOfWin32exe_.Kill();
             processOfWin32exe_ = null;
         }*/
        }
    }

    private void stopWin32Process ()
    {
       
        if (processOfWin32exe_!= null ) {
            if (processOfWin32exe_.HasExited)
            {
                processOfWin32exe_.Close();
                processOfWin32exe_ = null;
                return;
            }

            processOfWin32exe_.Kill();
            processOfWin32exe_ = null;
        }
    }



    private void registerWin32ControllerEvent() {
        win32Controller_.SetDeviceFailCallBack(() => {
            GetPackageController().StopWin32();
        });
        win32Controller_.RegisterEvent(MessageCommon.STR_MN_HEART_BEAT_COMPLIE,(object obj)=> {
            UnityEngine.Debug.Log("appcontroller message heart beat complie..");
            //mark
            this.heartBeatComplie();
        });

        win32Controller_.RegisterEvent(MessageCommon.STR_MN_RELOAD_RES_COMPLIE, (object obj)=> {
            MessageReloadComplie m = obj as MessageReloadComplie;
            // reload Complie
            if (cbOfReloadComplie_ != null)
            {
                cbOfReloadComplie_(m.Result);
                UnityEngine.Debug.Log("reloadResComplie");
            }
            else {
                UnityEngine.Debug.Log("reloadResFail");
            }
        });

        win32Controller_.RegisterEvent(MessageCommon.STR_MN_LINKER_HELLO, (object obj) => {
            MessageLinkerHello m = obj as MessageLinkerHello;
            GetPackageController().OnUserCellConnect(m.DeviceName);
            startHeartBeat();
        });

        win32Controller_.RegisterEvent(MessageCommon.STR_MN_LOAD_RES_STATUE_UPDATE,(object obj)=> {
            MessageLoadResStatueChange m = obj as MessageLoadResStatueChange;
            GetPackageController().SetLoadResLayerActive(m.IsLoading);
        });

        win32Controller_.RegisterEvent(MessageCommon.STR_MN_PLAY_STATUE_CHANGE,(object obj)=> {
            if (obj != null) {
                MessageScenePlayStatueChange m = obj as MessageScenePlayStatueChange;
                GetPackageController().SetSceneRunningStatue(m.SceneID, m.IsPlaying);
            }
        });
    }


    private void startWin32Controller() {
        if (win32Controller_ == null) {
            win32Controller_ = new Win32Controller();
            win32Controller_.Start();
            registerWin32ControllerEvent();
        }
    }
    private void stopWin32Controller() {
        if (win32Controller_!= null) {
            win32Controller_.Dispose();
            win32Controller_ = null;
        }
        
    }


    private void prepareGotoFile() {
        var filePath = GetWin32ProjPath()+ "\\xiaobanlong\\xiaobanlong5.2.0\\goto.txt";
        var str = "LinkerTestPackage";
        File.WriteAllText(filePath,str);
    }

    private void prepareWin32() {
        var projPath = GetWin32ProjPath();
        if (projPath!= "") {
            prepareGotoFile();
            var oriPath = System.Environment.CurrentDirectory + "\\LinkerTestPackage";
            var configFilePath = oriPath + "\\task\\IpConfig.json";
            var ip = GetHostIP();
            Win32Controller.GenerateIpConfigFileByPath(configFilePath,ip);
            var aimPath = GetWin32ProjPath() + "\\xiaobanlong\\xiaobanlong5.2.0\\Win32TestRes\\57";
            DirectUtils.CopyDir(oriPath,aimPath);
        }
    }

    private void reloadRes() {
        if (win32Controller_ != null) {
            win32Controller_.ReloadRes();
        }
    }

    private void playScene() {
        if (win32Controller_!= null) {
            win32Controller_.PlayScene();
        }
    }
    private void playSceneReloadDragonbone() {
        if (win32Controller_!=null) {
            win32Controller_.PlaySceneReloadDragonBone();
        }
    }

    private void stopScene() {
        if (win32Controller_!=null) {
            win32Controller_.StopScene();

        }
    }

    private void playSceneBySceneIdAndNpcNameAndOptionIndex(string sceneId,string npcName,int opIndex) {
        if (win32Controller_!= null) {
            win32Controller_.PlaySceneBySceneIDAndNpcNameAndOptionIndex(sceneId,npcName,opIndex);
        }
    }

    private void updateProjToWin32() {
        // 初始化目录
        /**/
        GetPackageController().GenerateFormatProjFile();
        var pData = GetTargetPackageInfo();
        var path = pData[ProjData.STR_PATH] as string;
        var aimPath = GetWin32ProjPath() + "\\xiaobanlong\\xiaobanlong5.2.0\\Win32TestRes\\57\\LinkerTestPackage\\Task\\LinkerData";
        if (Directory.Exists(aimPath)) {
            Directory.Delete(aimPath,true);
        }
        Directory.CreateDirectory(aimPath);// 这里拷贝的应该用到的资源而已
        UnityEngine.Debug.Log(path);
           
        // 复制目录
        DirectUtils.CopyDir(path,aimPath);
        
    }


    private void OnDestroy()
    {
        if (processOfWin32exe_ != null) {   StopWin32Exe(); }
    }
    // ----- 对外接口 -----

    public void SetTargetPackageInfo(Dictionary<string ,object> info) { this.getData<AppData>().SetTargetPackageInfo(info); }
    public void SetTargetSceneInfo(SceneNodeData data) { this.getData<AppData>().SetTargetSceneInfo(data); }
    public Dictionary<string,object> GetTargetPackageInfo() { return this.getData<AppData>().GetTargetPackageInfo(); }
    public SceneNodeData GetTargetSceneInfo() { return this.getData<AppData>().GetTargetSceneInfo(); }



    public void StartCellController()
    {
        startWin32Controller();
    }
    public void StopCellController()
    {
        stopWin32Controller();
        stopHeartBeat();
    }

    public PackageController GetPackageController() { return (PackageController)myPackageController_; }

    public void SetWin32ProjPath(string p)
    {
        this.getData<AppData>().SetWin32ProjPath(p);
    }
    public string GetWin32ProjPath()
    {
        return this.getData<AppData>().GetWin32ProjPath();
    }

    public void PrepareWin32() {
        prepareWin32();
    }

    public void StartWin32Exe() {
        startWin32Controller();
        startWin32Process();
    }
    public void StopWin32Exe() {
        stopHeartBeat();
        stopWin32Process();
        stopWin32Controller();
    }

    public void PlayScene(bool isReloadDragonboneData) {
        if (isReloadDragonboneData)
        {
            playSceneReloadDragonbone();
        }
        else {
            playScene();
        }
    }
    

    public void StopScene() {
        stopScene();
    }

    public void LoadRes() {
        this.win32Controller_.LoadRes();
    }

    public void UpdateRes() {
        this.httpManager_.UpdateRes();
    }
    public void UpdateResByAimFloder(List<string> list)
    {
       UnityEngine.Debug.Log("update res to server xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        this.httpManager_.UpdateResByAimFloder(list);
    }

    public void PlaySceneBySceneIdAndNpcNameAndOptionIndex(string sceneId,string npcName,int opIndex) {
        this.playSceneBySceneIdAndNpcNameAndOptionIndex(sceneId,npcName,opIndex);
    }


    public string GetHostIP()
    {
        IPHostEntry localhost = Dns.GetHostByName(Dns.GetHostName());    //方法已过期，可以获取IPv4的地址
        var count = localhost.AddressList.Length;
        for (int i = 0; i < count; ++i)
        {
            var str = localhost.AddressList[i].ToString();
            if (str.Contains("192.168."))
            {
                return str;
            }
        }
        UnityEngine.Debug.LogError("Can t get host name");
        return "";
    }

    public void ShowAlertView(string str) { myAlertView_.ShowAlert(str); }

    public void ShowLoadingView() {
        myAlertView_.ShowLoading();
    }
    public void CloseLoadingView() {
        myAlertView_.CloseLoading();
    }
}
