using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DataUpdate(object obj);
//public delegate void DataUpdateParamObject<T>(T obj);

public class BaseData : MonoBehaviour {
    private BaseController myController_ = null;
    public void SetController(BaseController c) { myController_ = c; }
    protected T GetController<T>()where T:BaseController {
        return (T)myController_;
    }

    public virtual void init() { }
    public virtual void wakeup() { }
    public virtual void dispear() { }
    public virtual void dispose() { }

    public DataUpdate eventOfDataUpdates_;
   // public DataUpdateParamObject
}
