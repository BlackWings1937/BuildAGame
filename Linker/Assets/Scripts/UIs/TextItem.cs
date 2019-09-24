using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextItem : PointParent
{
    // Start is called before the first frame update
    void Start()
    {
        //InitPoints();
    }

    protected override void InitPoints()
    {
        var p1 = generatePointByLocalPos(new Vector2(0, 0));
        var p2 = generatePointByLocalPos(new Vector2(10, 0));
        listPoints_.Add(p1);
        listPoints_.Add(p2);
    }
    public void InitNow()
    {
        InitPoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
