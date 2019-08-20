/*
 * 用来将按钮的大小设置覆盖按钮内部的字体的大小
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnAdaptText : MonoBehaviour
{
    public Text MyText;
    public float LeftPadding = 0;
    public float RightPadding = 0;
    public void SetText(string str) {
        if (MyText!= null) {
            var length = TextUtils.CaculateTextLength(str, MyText);
            ((RectTransform)transform).sizeDelta = new Vector2(LeftPadding+length+RightPadding, ((RectTransform)transform).sizeDelta.y);
        }
    }
}
/*
    //debug
    private void Start()
    {
        SetText("拷贝");
    }*/
