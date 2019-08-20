/*
 * 可以自由添加事件回调的buttonGroup
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void TapButtonCallBack();

public class TapButtonsGroup : MonoBehaviour
{

    // tapButtonGroup两端的长度
    public float PaddingLeft = 0f;
    public float PaddingRight = 0f;

    //用来添加的按钮
    public GameObject PrefabBtn;

    //按钮的Content
    public RectTransform BtnsContent;
    public HorizontalLayoutGroup MyHorizontalLayoutGroup;

    //
    private Dictionary<string, TapButtonCallBack> myInfo_;

    // 设置tapButoonGroup 事件面板
    public void SetEventByDic(Dictionary<string, TapButtonCallBack> dic) {
        clearBeforeItem();
        myInfo_ = dic;

        if (BtnsContent != null && PrefabBtn != null ) {

            var lengthOfContent = 0f;

            var paddingOfTapBtnsGroup = PaddingLeft + PaddingRight;

            var itemCount = 0;

            var btnsLength = 0f;

            foreach( var pair in dic) {
                var strTitle = pair.Key;
                var cb = pair.Value;

                var btnItem = GameObject.Instantiate(PrefabBtn) as GameObject;
                btnItem.GetComponent<BtnAdaptText>().SetText(strTitle);


                itemCount = itemCount + 1;
            }


            var btnsSpacing = 0f;
            btnsSpacing = MyHorizontalLayoutGroup.spacing * (itemCount - 1);

            lengthOfContent = paddingOfTapBtnsGroup + btnsLength + btnsSpacing; // 内容大小

            BtnsContent.sizeDelta = new Vector2(lengthOfContent, BtnsContent.sizeDelta.y);

        }


    }

    private void clearBeforeItem() {
        if (BtnsContent!= null) {
            for (int i = 0;i<BtnsContent.childCount;++i) {
                GameObject.Destroy(BtnsContent.GetChild(i).gameObject);
            }
        }
    }
}
