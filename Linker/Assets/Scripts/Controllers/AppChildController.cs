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
    public override void ActiveController()
    {
        gameObject.SetActive(true);
        init();
        getView<BaseView>().init();
        getData<BaseData>().eventOfDataUpdates_ += getView<BaseView>().UpdateView;
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
