using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointParent : MonoBehaviour
{
    protected List<GPoint> listPoints_ = new List<GPoint>();
    protected virtual void InitPoints() { }
    protected void DisposePoints()
    {
        var count = listPoints_.Count;
        for (int i = 0; i < count; ++i)
        {
            PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName("GPoint").Recycle(listPoints_[i].gameObject);
            //GameObject.Destroy(listPoints_[i].gameObject);
        }
        listPoints_.Clear();
    }
    protected GPoint generatePointByLocalPos(Vector2 localPos) {
        var gp1 = PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName("GPoint").Get();
        gp1.transform.SetParent(this.transform, false);
        gp1.transform.localPosition = localPos;
        return gp1.GetComponent<GPoint>();
    }

    public int GetPointCount() { return listPoints_.Count; }
    public GPoint GetPointByIndex( int index) {
        if (index<0||index>=listPoints_.Count) { return null; }
        return listPoints_[index]; }
}
