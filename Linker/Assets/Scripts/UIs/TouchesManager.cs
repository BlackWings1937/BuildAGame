﻿/*
 * 管理所有用户触碰信息的单例
 * 
 * 自动创建一个空的游戏对象实例化自身
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchesManager : MonoBehaviour 
{
    /*
     * 自动初始化自身的单例
     */
    private static readonly string STR_INSTANCE_PREFAB_NAME = "TouchesManager";
    private static TouchesManager _instance = null;
    //判断单例是否被销毁
    public static bool IsInstanceBeDestroyed() { return _instance == null; }
    public static TouchesManager GetInstance() {
        if (_instance == null) {
            var prefab = Resources.Load("prefab/" + STR_INSTANCE_PREFAB_NAME);
            var instanceGameObj = GameObject.Instantiate(prefab) as GameObject;
            _instance = instanceGameObj.GetComponent<TouchesManager>();
        }
        return _instance;
    }
    private TouchesManager(){ myListOfTouchObjects_ = new List<TouchObject>(); }

    // touchobjects list
    private List<TouchObject> myListOfTouchObjects_;

    // 排序touchobject 根据 order
    private void sortTouchObjectsByOrder() {
        myListOfTouchObjects_.Sort((TouchObject t1,TouchObject t2)=> {
            if (t1.Order > t2.Order) {
                return 1;
            } else if (t1.Order == t2.Order) {
                return 0;
            } else {
                return -1;
            }
        });
    }


    private bool isMouseKeyDown_ = false;

    private Touch touch_ = null;

    private void Update()
    {
        sortTouchObjectsByOrder();

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            isMouseKeyDown_ = true;
            touch_ = new Touch();
            touch_.SetGetTouchObjects(()=> { return myListOfTouchObjects_; });
            touch_.OnTouchBegan(Input.mousePosition);
        }
        if (isMouseKeyDown_) {
            if (touch_!= null) {  touch_.OnTouchMoved(Input.mousePosition); }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            isMouseKeyDown_ = false;
            if (touch_!=null) {
                touch_.OnTouchEnded(Input.mousePosition);
                touch_ = null;
            }
        }
    }


    // ----- 对外接口 -----
    public void Add(TouchObject t,int order = 0) {
        if (t == null) { return; }
        myListOfTouchObjects_.Add(t);
    }
    public void Remove(TouchObject t) {
        if (t == null||(!myListOfTouchObjects_.Contains(t))) { return; }
        myListOfTouchObjects_.Remove(t);
    }
    public void RemoveAll() { myListOfTouchObjects_.RemoveRange(0, myListOfTouchObjects_.Count); }
    public void Dispose() { RemoveAll(); }


}