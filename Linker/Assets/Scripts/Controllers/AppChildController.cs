using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ControllerEventCallBack();

public class AppChildController : ChildController {

    public AppController GetParentController() { return this.getParentController<AppController>();}
    public void SetParentController(AppController a) { this.setParentController<AppController>(a); }


    public ControllerEventCallBack MyActiveCallBack  { get;  set; }
    public ControllerEventCallBack MyDisposeCallBack { get; set; }
    public ControllerEventCallBack MyDispearCallBack { get; set; }
    public ControllerEventCallBack MyWakeUpCallBack  { get;  set; }
    private void invokeEvent(ControllerEventCallBack cb) {
        if (cb!= null) { cb(); }
    }

    /// <summary>
    /// 初始化系统，每一次调用前，要确保系统是被dispose后的状态
    /// </summary>
    public override void ActiveController()
    {
        gameObject.SetActive(true);
        // 初始化控制器
        init();
        //初始化view（控件事件这些东西）
        getView<BaseView>().init();
        //注册更新事件
        getData<BaseData>().eventOfDataUpdates_ += getView<BaseView>().UpdateView;
        //初始化数据（数据更新自动到view）
        getData<BaseData>().init();
        invokeEvent(MyActiveCallBack);
    }

    public override void DispearController()
    {
        getView<BaseView>().dispear();
        getData<BaseData>().dispear();
        gameObject.SetActive(false);
        invokeEvent(MyDispearCallBack);
    }

    public override void WakeUpController()
    {
        gameObject.SetActive(true);
        getView<BaseView>().wakeup();
        getData<BaseData>().wakeup();
        invokeEvent(MyWakeUpCallBack);
    }

    /// <summary>
    /// 撤销系统的方法
    /// </summary>
    public override void DisposeController()
    {
        dispose();
        getData<BaseData>().eventOfDataUpdates_ -= getView<BaseView>().UpdateView;
        getData<BaseData>().dispose();
        getView<BaseView>().dispose();
        gameObject.SetActive(false);
        invokeEvent(MyDisposeCallBack);
    }
}
