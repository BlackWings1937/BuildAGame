using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoseOptionsBox : MonoBehaviour
{
    // ----- 私有成员 -----
    [SerializeField]
    private Text texName_ = null;

    [SerializeField]
    private RectTransform rtOfContent_ = null;

    [SerializeField]
    private GameObject prefabItem_ = null;

    private List<Button> listOfBtns_ = new List<Button>();


    // ----- 私有方法 -----

    private void clearListOfBtns() {
        for(int i = 0;i<listOfBtns_.Count;++i) {
            GameObject.Destroy(listOfBtns_[i].gameObject);
        }
        listOfBtns_.Clear();
    }
    

    private void updateBoxName(string name) {
        if (texName_ != null) {
            texName_.text = name;
        }
    }
    private void updateOptionsByDic(Dictionary<string, TapButtonCallBack> dic) {
        if (rtOfContent_ != null&& prefabItem_ != null) {
            clearListOfBtns();
            var dicCount = dic.Count;
            var verticalGroup = rtOfContent_.GetComponent<VerticalLayoutGroup>();
            var contentHeight = verticalGroup.padding.top + verticalGroup.padding.bottom + 
                verticalGroup.spacing * (Mathf.Max(0, dicCount - 1)) + ((RectTransform)prefabItem_.transform).sizeDelta.y * dicCount;
            rtOfContent_.sizeDelta = new Vector2(rtOfContent_.sizeDelta.x,contentHeight);
            foreach (var pair in dic) {
                var cb = pair.Value;
                var g = GameObject.Instantiate(prefabItem_) as GameObject;
                g.transform.GetChild(0).GetComponent<Text>().text = pair.Key;
                g.GetComponent<Button>().onClick.AddListener(()=> {
                    if (cb!=null&& gameObject.activeSelf) {
                        cb();
                        gameObject.SetActive(false);
                    }
                    
                });
                g.transform.SetParent(rtOfContent_,false);
                listOfBtns_.Add(g.GetComponent<Button>());
            }
        }
    }
    // ----- 对外接口 -----
    public void UpdateOptionsByDic(Dictionary<string,TapButtonCallBack> dic) { updateOptionsByDic(dic); }
    public void UpdateBoxName(string name) { updateBoxName(name); }
}
