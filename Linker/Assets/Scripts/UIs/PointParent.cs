using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointParent : MonoBehaviour
{
    protected List<GPoint> listPoints_ = new List<GPoint>();
    protected virtual void InitPoints() { }
    protected GPoint generatePointByLocalPos(Vector2 localPos) {
        var gp1 = new GameObject();
        gp1.transform.SetParent(this.transform, false);
        gp1.transform.localPosition = localPos;
        return  gp1.AddComponent<GPoint>();
    }
    public int GetPointCount() { return listPoints_.Count; }
    public GPoint GetPointByIndex( int index) { return listPoints_[index]; }
}
