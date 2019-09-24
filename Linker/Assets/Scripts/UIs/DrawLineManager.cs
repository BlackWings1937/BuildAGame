using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineManager : MonoBehaviour
{
    /// <summary>
    /// 单例
    /// </summary>
    private static readonly string STR_INSTANCE_PREFAB_NAME = "DrawLineManager";
    private static DrawLineManager _instance = null;
    public static DrawLineManager GetInstance() {
        if (_instance == null) {
            var prefab = Resources.Load("prefab/"+ STR_INSTANCE_PREFAB_NAME);
            var instanceGameObj = GameObject.Instantiate(prefab) as GameObject;
            _instance = instanceGameObj.GetComponent<DrawLineManager>();
        }
        return _instance;
    }

    private void Start()
    {
        transform.localPosition = Vector3.zero;
    }


    // ----- 对外接口 -----
    //public DrawLine AddLineByEmptyGameObjects(GPoint p1,GPoint p2) { }
    //public void RemoveLineByPoints(GPoint p1,GPoint p2) { }
    //public void RemoveByDrawLine(DrawLine l) { }
}
