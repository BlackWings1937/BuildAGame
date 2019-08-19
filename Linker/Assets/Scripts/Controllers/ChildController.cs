using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildController :BaseController {
    public virtual bool InitByParam(Dictionary<string, object> dic) { return false; }
    public virtual void ActiveController() { }
    public virtual void WakeUpController() { }
    public virtual void DispearController() { }
    public virtual void DisposeController() { }


    private BaseController parent_ = null;
    protected T getParentController<T>() where T : BaseController { return (T)parent_; }
    public void setParentController<T>(T controller) where T : BaseController  { parent_ = controller; }
}
