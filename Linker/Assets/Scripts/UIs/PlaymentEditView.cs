using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaymentEditView : SceneViewChildView
{
    [SerializeField]
    private Button btnLuaScriptEdit_;

    [SerializeField]
    private Button btnProductConfigEdit_;

    [SerializeField]
    private Button btnAnimationConfigEdit_;


    private PlaymentInfoData data_;


    // ----- 初始化 -----
    private void Start()
    {
        registerEvent();
    }

    private void registerEvent() {
        if (btnLuaScriptEdit_ != null) {
            btnLuaScriptEdit_.onClick.AddListener(()=> { showLuaScriptChoseBox(); });
        }
        if (btnProductConfigEdit_!= null) {
            btnProductConfigEdit_.onClick.AddListener(()=> { showProductChoseBox(); });
        }
        if (btnAnimationConfigEdit_!=null) {
            btnAnimationConfigEdit_.onClick.AddListener(()=> { showAnimationChoseBox(); });
        }
    }

    // ----- 私有方法 ------
    private void setEditBtnByName(Button btn, string name,string defaultName) {
        if (btn != null) {
            if (name == "") { name = defaultName; }
            var adapt = btn.GetComponent<BtnAdaptText>();
            adapt.SetText(name);
        }
    }

    private void showLuaScriptChoseBox() {
        if (btnLuaScriptEdit_!=null) {
            MySceneView.ShowLuaScriptChoseBoxByEditBtn(btnLuaScriptEdit_);
        }
    }
    private void showAnimationChoseBox() {
        if (btnAnimationConfigEdit_ != null) {
            MySceneView.ShowAnimationConfigChoseBoxByEditBtn(btnAnimationConfigEdit_);
        }
    }
    private void showProductChoseBox() {
        if (btnProductConfigEdit_ != null) {
            MySceneView.ShowProductConfigBoxByEditBtn(btnProductConfigEdit_);
        }
    }

    // ----- 对外接口 -----
    public void UpdateByData(PlaymentInfoData data) {
        data_ = data;
        if (data_ != null)
        {
            setEditBtnByName(btnLuaScriptEdit_, data_.LuaScriptName,"LuaScript");
            setEditBtnByName(btnProductConfigEdit_, data_.ProductConfigName,"ProductConfig");
            setEditBtnByName(btnAnimationConfigEdit_, data_.AnimationConfigName, "AnimationConfig");
        }
        else {
            Debug.LogError("UpdateByData(PlaymentInfoData data) data == null");
        }

    }
}
