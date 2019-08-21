using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformUtils
{
    // 屏幕坐标转换到世界坐标
    public static Vector3 ScreenPosToWorldPos(Vector3 p) { return Camera.main.ScreenToWorldPoint(p); }

    //世界坐标到节点坐标
    public static Vector3 WorldPosToNodePos(Vector3 p, Transform t) { return t.InverseTransformPoint(p); }

    //节点坐标到世界坐标...
}
