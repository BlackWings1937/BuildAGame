using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class AppController : BaseController {

    public static Vector2 VISIBLE_SIZE = new Vector2(1280,720);
    // ----- 私有成员 ----- 
    public AppChildController myProjController_ = null;
    public AppChildController myPackageController_ = null;
    public AppChildController mySceneController_ = null;

    // ----- 生命周期 -----
    protected override void Start()
    {

        Serv serv = new Serv();
        serv.Start("127.0.0.1", 1234);


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



    private Process processOfWin32exe_;
    private void startWin32Process() {
        if (processOfWin32exe_== null) {
            var path = GetWin32ProjPath() + "\\xiaobanlong\\xiaobanlong5.2.0\\proj.win32\\Debug.win32\\xiaobanlong.exe";
            UnityEngine.Debug.Log("path:"+path);
            processOfWin32exe_ = Process.Start(path);
        }
    }

    private void stopWin32Process ()
    {
        if (processOfWin32exe_!= null) {
            processOfWin32exe_.Kill();
            processOfWin32exe_ = null;
        }
    }

    // ----- 对外接口 -----

    public void SetTargetPackageInfo(Dictionary<string ,object> info) { this.getData<AppData>().SetTargetPackageInfo(info); }
    public void SetTargetSceneInfo(SceneNodeData data) { this.getData<AppData>().SetTargetSceneInfo(data); }
    public Dictionary<string,object> GetTargetPackageInfo() { return this.getData<AppData>().GetTargetPackageInfo(); }
    public SceneNodeData GetTargetSceneInfo() { return this.getData<AppData>().GetTargetSceneInfo(); }


    public PackageController GetPackageController() { return (PackageController)myPackageController_; }

    public void SetWin32ProjPath(string p)
    {
        this.getData<AppData>().SetWin32ProjPath(p);
    }
    public string GetWin32ProjPath()
    {
        return this.getData<AppData>().GetWin32ProjPath();
    }

    public void StartWin32Exe() {
        startWin32Process();
    }
    public void StopWin32Exe() {
        stopWin32Process();
    }

}
