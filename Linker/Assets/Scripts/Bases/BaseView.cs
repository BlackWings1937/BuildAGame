using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseView : MonoBehaviour {
    private BaseController myController_ = null;
    public void SetController(BaseController c) { myController_ = c; }
    public T GetController<T>() where T : BaseController
    {
        return (T)myController_;
    }
    
    protected virtual void registerViewEvent(){}


    public virtual void init() { }
    public virtual void wakeup() { }
    public virtual void dispear() { }
    public virtual void dispose() { }

    public virtual void UpdateView(object info) { }


}
