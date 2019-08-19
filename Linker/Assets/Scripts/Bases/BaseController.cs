using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseView))]
[RequireComponent(typeof(BaseData))]
public class BaseController : MonoBehaviour {
    // ----- 私有成员 -----
    private BaseView myView_ = null;
    private BaseData myData_ = null;

    // ----- 生命周期方法 -----
    protected virtual void Start(){  }

    // ----- 私有方法 -----
    protected virtual bool init() {
        myView_ = GetComponent<BaseView>();
        myData_ = GetComponent<BaseData>();
        myView_.SetController(this);
        myData_.SetController(this);
        return true;
    }
    protected virtual void dispose() {
        myView_.SetController(null);
        myData_.SetController(null);
        myView_ = null;
        myData_ = null;
    }

    protected T getView<T>() where T:BaseView { return (T)myView_; }
    protected T getData<T>() where T:BaseData { return (T)myData_; }
}
