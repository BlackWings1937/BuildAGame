using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SceneNode : PointParent
{
    [SerializeField]
    private InputField myInputField_;

    [SerializeField]
    private VerticalTextItemGroup myOutPutBroard_;

    private GPoint p1_;
    private void Start()
    {
        registerEvent();
        setText("");
        InitPoints();
        debugInit();
    }

    private List<DrawLine> listOfLines_ = new List<DrawLine>();
    private void DrawInternalLine() {
        if (myOutPutBroard_!= null) {
            var textItems = myOutPutBroard_.GetTextItems();
            for (int i = 0;i<textItems.Count;++i) {
                var tlp = textItems[i].GetComponent<PointParent>();
                var l1 = DrawLine.Create(tlp.GetPointByIndex(0),tlp.GetPointByIndex(1),Color.green);
                var l2 = DrawLine.Create(tlp.GetPointByIndex(0),GetPointByIndex(0),Color.green);
                listOfLines_.Add(l1);
                listOfLines_.Add(l2);
            }
        }
    }
    private void ClearInternalLine() {
        for (int i = 0;i< listOfLines_.Count;++i) {
            GameObject.Destroy(listOfLines_[i].gameObject);
        }
        listOfLines_.Clear();
    }

    protected override void InitPoints()
    {
        var p = generatePointByLocalPos(new Vector2(0,0));
        listPoints_.Add(p);
    }

    private void debugInit()
    {
        setOutPutItemList(new List<string>() { "123", "abcdefg", "xxxxxx", "123", "abcdefg", "xxxxxxxx", "123", "abcdefg", "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" });
    }

    private void registerEvent()
    {
        var btnAdapt = GetComponent<BtnAdaptText>();
        var text = btnAdapt.MyText;
        var pointAt = text.GetComponent<PointAt>();
        if (pointAt != null)
        {
            pointAt.SetPointUpCallBack((Vector2 wp) =>
            {
                showInputField();
            });
            pointAt.SetPointCancleCallBack((Vector2 wp) =>
            {
                closeInputField();
                setText(myInputField_.text);
            });
        }
        if (myInputField_ != null)
        {
            myInputField_.onEndEdit.AddListener((string word) =>
            {
                closeInputField();
                setText(myInputField_.text);
            });
        }
    }

    [SerializeField]
    private float outputBoardPadding = 30;
    private void setText(string t)
    {
        if (t == "")
        {
            t = "未命名场景";
        }
        GetComponent<BtnAdaptText>().SetText(t);
        //myOutPutBroard_.transform.localPosition.y
        var aimX = outputBoardPadding + ((RectTransform)transform).sizeDelta.x / 2 + ((RectTransform)myOutPutBroard_.transform).sizeDelta.x / 2;
        myOutPutBroard_.transform.localPosition = new Vector3(aimX, 0, myOutPutBroard_.transform.localPosition.z);
    }

    private void setOutPutItemList(List<string> l)
    {
        if (myOutPutBroard_ != null)
        {
            ClearInternalLine();
            myOutPutBroard_.UpdateTextItemsByStringList(l);
            DrawInternalLine();
        }
    }

    private void showInputField()
    {
        if (myInputField_ != null)
        {
            myInputField_.gameObject.SetActive(true);
            myInputField_.ActivateInputField();// = true;
        }
    }

    private void closeInputField()
    {
        if (myInputField_ != null)
        {
            myInputField_.gameObject.SetActive(false);
        }
    }
}
