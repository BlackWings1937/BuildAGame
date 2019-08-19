using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public delegate void CellOldProjsClick(Dictionary<string,object> data);
public class CellOldProjs : MonoBehaviour {
    // ----- 生命周期方法 -----
    void Start() {
        initView();
    }
    // ----- 私有成员 -----
    public Text myTexProjName_ = null;
    public Text myTexProjPath_ = null;
    public Button myBtnClick_ = null;
    private CellOldProjsClick myClickCb_ = null;
    // ----- 私有数据 -----
    private Dictionary<string,object> myData_ = null;
    // ----- 私有方法 -----
    private void initView() {
        myBtnClick_.onClick.AddListener(onBtnClick);
    }
    private void onBtnClick() {
        if (myClickCb_ != null){
            myClickCb_(myData_);
        }
    }
    private void updateView() {
        myTexProjPath_.text = (string)myData_["name"];
        myTexProjName_.text = (string)myData_["path"];
    }
    // ----- 对外接口 -----
    public void UpdateData(Dictionary<string, object> data) {
        myData_ = data;
        updateView();
    }
    public void SetClickCallBack(CellOldProjsClick cb) {
        myClickCb_ = cb;
    }
}
