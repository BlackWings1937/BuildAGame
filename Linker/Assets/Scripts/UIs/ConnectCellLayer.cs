using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void UserCloseConnectLayer();
public class ConnectCellLayer : MonoBehaviour
{
    [SerializeField]
    private Text myText_;

    [SerializeField]
    private Button myCloseBtn_;

    private UserCloseConnectLayer cbOfClose_ = null;

    private void Start()
    {
        registerEvent();
    }

    private void registerEvent()
    {
        if (myCloseBtn_ != null) {
            myCloseBtn_.onClick.AddListener(()=> {
                this.onCloseLayer();
            });
        }

    }

    public void SetIP(string strIp) {
        if (myText_ != null) {
            myText_.text = "请用手机连接:" + strIp;
        }
    }
    public void SetCloseLayerCloseCallBack(UserCloseConnectLayer v) { cbOfClose_ = v; }
    private void onCloseLayer() {
        if (cbOfClose_ !=null) {
            cbOfClose_();
        }
    }



}
