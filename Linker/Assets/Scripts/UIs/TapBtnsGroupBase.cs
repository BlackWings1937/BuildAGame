/*
 * 可以自由添加事件回调的buttonGroup
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class TapBtnsGroupBase : MonoBehaviour
{
    private List<GameObject> listOfTapBtns_ = new List<GameObject>();

    private const string STR_PREFAB_TAP_BTN_NAME = "TapBarBtn";

    private HorizontalLayoutGroup myHorizontalGroup_ = null;
    private void Start()
    {
        myHorizontalGroup_ = GetComponent<HorizontalLayoutGroup>();
    }

    public Vector2 GetWPByIndex(int i) {
        if (i>=0&&i<listOfTapBtns_.Count) {
            var g = listOfTapBtns_[i].gameObject;
            return g.transform.position;
        }
        return Vector2.zero;
    }

    public void SetEventByDic(Dictionary<string, UnityAction> dic) {
        
        var f = PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName(STR_PREFAB_TAP_BTN_NAME);
        var count = listOfTapBtns_.Count;
        for (int i = 0;i<count;++i) {
            var g = listOfTapBtns_[i];
            f.Recycle(g);
        }
        listOfTapBtns_.Clear();

        var countOfDic = dic.Count;
        var space = myHorizontalGroup_.spacing;
        var lp = myHorizontalGroup_.padding.left;
        var rp = myHorizontalGroup_.padding.right;
        var width = lp + rp + ((Mathf.Max(0, countOfDic - 1)) * space);

        foreach (var pair in dic) {
            var g = f.Get();
            g.GetComponent<BtnAdaptText>().SetText(pair.Key);
            g.GetComponent<Button>().onClick.AddListener(pair.Value);
            g.transform.SetParent(transform,false);
            var w = (g.transform as RectTransform).sizeDelta.x;
            width += w;
            listOfTapBtns_.Add(g);
        }
        (transform as RectTransform).sizeDelta = new Vector2(width, (transform as RectTransform).sizeDelta.y);
    }
}
