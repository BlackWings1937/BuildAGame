﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageController : AppChildController {


    // ----- 对外接口 -----
    public void ShowTapBtnsGroup() {
        var dic = new Dictionary<string, TapButtonCallBack>();
        dic.Add("创建场景",()=> { Debug.Log("创建场景"); getView<PackageView>().CloseBtnsGroup(); });
        //debug
        dic.Add("粘贴",()=> { Debug.Log("粘贴节点"); getView<PackageView>().CloseBtnsGroup(); });
        dic.Add(TapButtonsGroup.STR_KEY_CANCLE,()=> { Debug.Log("取消");  getView<PackageView>().CloseBtnsGroup(); });
        getView<PackageView>().ShowBtnsGroupByDic(dic);
    }
}