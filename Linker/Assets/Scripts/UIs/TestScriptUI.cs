using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptUI : MonoBehaviour
{
    /*
    private LineRenderer myLineRender_;
    // Start is called before the first frame update
    void Start()
    {
        myLineRender_ = GetComponent<LineRenderer>();
        myLineRender_.positionCount = (3);
        myLineRender_.SetWidth(0.1f,0.1f);
       // GetComponent<VerticalTextItemGroup>().UpdateTextItemsByStringList(new List<string>() { "123","abcdefg","xxxxxx", "123", "abcdefg", "xxxxxxxx", "123", "abcdefg", "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" });
    }

    // Update is called once per frame
    void Update()
    {
        myLineRender_.SetPosition(0,new Vector3(0,0,90));
        myLineRender_.SetPosition(2,new Vector3(1,0,90));
        myLineRender_.SetPosition(2, new Vector3(1,188,90));
    }*/


    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 20;

    private List<Vector3> listOfPoes_ = new List<Vector3>() { new Vector3(0,0,0),new Vector3(0,25,0) };
    void Start()
    {
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = listOfPoes_.Count;

        /*
        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
        */
    }

    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        var t = Time.time;
        for (int i = 0; i < lengthOfLineRenderer; i++)
        {
            lineRenderer.SetPosition(i, listOfPoes_[i]);
        }
    }
}
