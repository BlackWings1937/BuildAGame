using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppView : BaseView {

    //------------Const---------------
    private static string G_N_VIEWS = "Views";

    // Use this for initialization
    void Start () {
		
	}

    //-------------对外接口--------------------
    public void InitView() {
        // 初始化程序界面
        Transform views = transform.Find(G_N_VIEWS);
        for (int i = 0;i<views.childCount;++i) {
            Transform tc = views.GetChild(i);
            tc.gameObject.SetActive(false);
        }
    }
}
