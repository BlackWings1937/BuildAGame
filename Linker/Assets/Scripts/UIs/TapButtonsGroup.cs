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
    public static readonly string STR_KEY_CANCLE = "CANCLE";
    // tapButtonGroup两端的长度
    public float PaddingLeft = 0f;
    public float PaddingRight = 0f;

    //用来添加的按钮
    public GameObject PrefabBtn;

    //按钮的Content
    public RectTransform BtnsContent;
    public HorizontalLayoutGroup MyHorizontalLayoutGroup;

    [SerializeField]
    private Button btnCancle_ = null;

    //
    private Dictionary<string, TapButtonCallBack> myInfo_;

    private void Start()
    {
        if (btnCancle_ != null) {
            btnCancle_.onClick.AddListener(() => {
                if (myInfo_.ContainsKey(STR_KEY_CANCLE)){
                    myInfo_[STR_KEY_CANCLE]();
                }
            });
        }
    }
    private bool isDefaultEvent(string str) { return ((str == TapButtonsGroup.STR_KEY_CANCLE));  }
    // 设置tapButoonGroup 事件面板
    public void SetEventByDic(Dictionary<string, TapButtonCallBack> dic) {
        clearBeforeItem();
        myInfo_ = dic;

        if (BtnsContent != null && PrefabBtn != null ) {

            var lengthOfContent = 0f;

            //计算左右空出距离长度
            var paddingOfTapBtnsGroup = PaddingLeft + PaddingRight;

            var itemCount = 0;

            //计算按钮s长度
            var btnsLength = 0f;
            var btnHeight = 0f;
            foreach( var pair in dic) {
                var strTitle = pair.Key;
                var cb = pair.Value;

                if (isDefaultEvent(strTitle)) continue;

                //累加按钮长度
                var btnItem = GameObject.Instantiate(PrefabBtn) as GameObject;
                btnItem.GetComponent<BtnAdaptText>().SetText(strTitle);
                btnItem.transform.SetParent(BtnsContent,false);
                btnsLength = btnsLength + ((RectTransform)btnItem.transform).sizeDelta.x;
                btnHeight = ((RectTransform)btnItem.transform).sizeDelta.y;
                //注册按钮方法
                btnItem.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (cb!=null) { cb(); }
                }); 

                //累加按钮数量
                itemCount = itemCount + 1;
            }

            // 计算按钮间隔长度
            var btnsSpacing = 0f;
            btnsSpacing = MyHorizontalLayoutGroup.spacing * (Mathf.Max(0, itemCount - 1) );

            lengthOfContent = paddingOfTapBtnsGroup + btnsLength + btnsSpacing; // 内容大小

            BtnsContent.sizeDelta = new Vector2(lengthOfContent, btnHeight);
            ((RectTransform)transform).sizeDelta = BtnsContent.sizeDelta;
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
