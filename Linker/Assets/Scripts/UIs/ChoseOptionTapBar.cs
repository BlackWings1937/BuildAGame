using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChoseOptionTapBar : ChoseOptionBase
{
    [SerializeField]
    private Button btnCancle_;
    public void SetBtnCancleEvent(TapButtonCallBack cb) {
        btnCancle_.onClick.RemoveAllListeners();
        btnCancle_.onClick.AddListener(()=> {
            if (cb!=null) { cb(); }
        });
    }
    protected override void updateOptionsByDic(Dictionary<string, TapButtonCallBack> dic)
    {
        if (rtOfContent_ != null && prefabItem_ != null)
        {
            clearListOfBtns();
            var dicCount = dic.Count;
            var verticalGroup = rtOfContent_.GetComponent<VerticalLayoutGroup>();
            var contentHeight = verticalGroup.padding.top + verticalGroup.padding.bottom +
                verticalGroup.spacing * (Mathf.Max(0, dicCount - 1)) + ((RectTransform)prefabItem_.transform).sizeDelta.y * dicCount;
            rtOfContent_.sizeDelta = new Vector2(rtOfContent_.sizeDelta.x, contentHeight);
            foreach (var pair in dic)
            {
                var cb = pair.Value;
                var g = GameObject.Instantiate(prefabItem_) as GameObject;
                g.GetComponent<BtnAdaptText>().SetText(pair.Key);
                g.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (cb != null && gameObject.activeSelf)
                    {
                        gameObject.SetActive(false);
                        cb();
                    }

                });
                g.transform.SetParent(rtOfContent_, false);
                listOfBtns_.Add(g.GetComponent<Button>());
            }
        }
    }
    public void UpdateOptionsByDic(Dictionary<string, TapButtonCallBack> dic) { updateOptionsByDic(dic); }
}
