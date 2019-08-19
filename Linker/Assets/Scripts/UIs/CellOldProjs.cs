using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellOldProjsData {
    public string myProjPath_;
    public string myProjName_;
}
public delegate void CellOldProjsClick(CellOldProjsData data);
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
    private CellOldProjsData myData_ = null;
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
        myTexProjPath_.text = myData_.myProjPath_;
        myTexProjName_.text = myData_.myProjName_;
    }
    // ----- 对外接口 -----
    public void UpdateData(CellOldProjsData data) {
        myData_ = data;
        updateView();
    }
    public void SetClickCallBack(CellOldProjsClick cb) {
        myClickCb_ = cb;
    }
}
