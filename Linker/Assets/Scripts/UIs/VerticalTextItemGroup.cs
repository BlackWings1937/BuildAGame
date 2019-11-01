using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalTextItemGroup : MonoBehaviour
{
    public enum State{
        E_LEFT,
        E_RIGHT,
    }

    public State MyState = State.E_RIGHT;


    [SerializeField]
    private GameObject myTextItemPrefab_;

    private List<GameObject> listOfTextItem_ = new List<GameObject>();

    private PackageView pv_;

    public void SetPackageView(PackageView pv) { pv_ = pv; }

    public void UpdateTextItemsByStringList(List<OutputPortData> l,SceneNodeData sd) {
        // 删除之前的节点
        listOfTextItem_.Clear();
        var childCount = transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            var child = transform.GetChild(i);
            child.GetComponent<TextItem>().Dispose();
            GameObject.Destroy(child.gameObject);
        }
        //创建新的节点
        for (int i = 0;i<l.Count;++i) {
            var g = GameObject.Instantiate(myTextItemPrefab_) as GameObject;
            g.transform.SetParent(this.transform,false);
            listOfTextItem_.Add(g);
            g.GetComponent<TextItem>().InitTextItemByData(l[i],i,pv_,sd);
        }
    }

    public void InfoItemToDrawLines() {
        var childCount = listOfTextItem_.Count;
        for (int i = 0; i < childCount; ++i)
        {

            var child = listOfTextItem_[i];
            if (MyState == State.E_RIGHT)
            {
                child.GetComponent<TextItem>().DrawLineRight();
            }
            else {
                child.GetComponent<TextItem>().DrawLineLeft();
            }
        }
    }

    public List<GameObject> GetTextItems() {
        return listOfTextItem_;
    }


}
