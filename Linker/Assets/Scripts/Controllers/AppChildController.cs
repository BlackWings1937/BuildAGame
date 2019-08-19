using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppChildController : ChildController {

    public AppController GetParentController() { return this.getParentController<AppController>();}
    public void SetParentController(AppController a) { this.setParentController<AppController>(a); }

    public override void ActiveController()
    {
        gameObject.SetActive(true);
        init();
        getView<BaseView>().init();
        getData<BaseData>().eventOfDataUpdates_ += getView<BaseView>().UpdateView;
        getData<BaseData>().init();
    }

    public override void DispearController()
    {
        getView<BaseView>().dispear();
        getData<BaseData>().dispear();
        gameObject.SetActive(false);
    }

    public override void WakeUpController()
    {
        gameObject.SetActive(true);
        getView<BaseView>().wakeup();
        getData<BaseData>().wakeup();
    }

    public override void DisposeController()
    {
        dispose();
        getData<BaseData>().eventOfDataUpdates_ -= getView<BaseView>().UpdateView;
        getData<BaseData>().dispose();
        getView<BaseView>().dispose();
        gameObject.SetActive(false);
    }
}
