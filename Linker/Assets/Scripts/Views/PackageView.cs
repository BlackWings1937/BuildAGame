/*
 * package 界面的 view
 * 
 * title: view
 * 
 * 用来管理packageView 的所有UI
 */


using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class PackageView : BaseView
{

    //重写UI注册事件
    protected override void registerViewEvent()
    {

    }

    //重写UI初始化方法
    public override void init()
    {
        registerViewEvent();
    }
}
