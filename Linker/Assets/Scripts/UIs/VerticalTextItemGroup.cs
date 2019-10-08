using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalTextItemGroup : MonoBehaviour
{

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
        var childCount = transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            var child = transform.GetChild(i);
            child.GetComponent<TextItem>().DrawLine();
        }
    }

    public List<GameObject> GetTextItems() {
        return listOfTextItem_;
    }


}
