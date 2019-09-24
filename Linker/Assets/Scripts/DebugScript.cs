using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{

    public TapButtonsGroup myGroup_;
    // Start is called before the first frame update
    void Start()
    {
        var dic = new Dictionary<string, TapButtonCallBack>();
        dic.Add("创建场景", () => { Debug.Log("创建场景 方法回调"); });
        dic.Add("粘贴", () => { Debug.Log("粘贴节点  方法回调"); });
        myGroup_.SetEventByDic(dic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
