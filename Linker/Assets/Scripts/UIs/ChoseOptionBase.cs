using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoseOptionBase : MonoBehaviour
{
    [SerializeField]
    protected RectTransform rtOfContent_ = null;

    [SerializeField]
    protected GameObject prefabItem_ = null;

    protected List<Button> listOfBtns_ = new List<Button>();

    protected void clearListOfBtns()
    {
        for (int i = 0; i < listOfBtns_.Count; ++i)
        {
            GameObject.Destroy(listOfBtns_[i].gameObject);
        }
        listOfBtns_.Clear();
    }

    protected virtual void updateOptionsByDic(Dictionary<string, TapButtonCallBack> dic)
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
                g.transform.GetChild(0).GetComponent<Text>().text = pair.Key;
                g.GetComponent<Button>().onClick.AddListener(() => {
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
}
