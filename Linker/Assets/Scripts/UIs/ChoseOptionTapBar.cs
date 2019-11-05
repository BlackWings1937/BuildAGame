using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChoseOptionTapBar : ChoseOptionBase
{
    [SerializeField]
    private Button btnCancle_;
    public void SetBtnCancleEvent(TapButtonCallBack cb) {
        btnCancle_.onClick.RemoveAllListeners();
        btnCancle_.onClick.AddListener(()=> {
            if (cb!=null) { cb(); }
        });
    }
    public void UpdateOptionsByDic(Dictionary<string, TapButtonCallBack> dic) { updateOptionsByDic(dic); }
}
