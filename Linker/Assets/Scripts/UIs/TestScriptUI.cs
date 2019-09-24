using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptUI : MonoBehaviour
{
    private LineRenderer myLineRender_;
    // Start is called before the first frame update
    void Start()
    {
       // myLineRender_ = GetComponent<LineRenderer>();
        //myLineRender_.positionCount = (3);
        //myLineRender_.SetWidth(0.1f,0.1f);
       // GetComponent<VerticalTextItemGroup>().UpdateTextItemsByStringList(new List<string>() { "123","abcdefg","xxxxxx", "123", "abcdefg", "xxxxxxxx", "123", "abcdefg", "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" });
    }

    // Update is called once per frame
    void Update()
    {
        //myLineRender_.SetPosition(0,new Vector3(0,0,90));
       // myLineRender_.SetPosition(2,new Vector3(1,0,90));
       // myLineRender_.SetPosition(2, new Vector3(1,188,90));
    }
}
