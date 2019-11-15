using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertView : MonoBehaviour
{
    [SerializeField]
    private GameObject myAlert_;

    [SerializeField]
    private GameObject myLoading_;

    [SerializeField]
    private Text myText_;

    [SerializeField]
    private Button myBtnOk_;

    private void registerEvent() {

        if (myBtnOk_!=null) {
            myBtnOk_.onClick.AddListener(()=> {
                closeAlertView();
            });
        }
    }

    private void closeAlertView() {
        if (myAlert_ !=null) {
            myAlert_.SetActive(false);
        }
    }
    private void showAlertView() {
        if (myAlert_ != null) {
            myAlert_.SetActive(true);
        }
    }

    private void closeLoading() {
        if (myLoading_ != null) {
            myLoading_.SetActive(false);
        }
    }
    private void showLoading() {
        if (myLoading_ != null) {
            myLoading_.SetActive(true);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        registerEvent();
    }


    public void SetText(string str) {
        if (myText_!=null) {
            myText_.text = str;
        }
    }

    public void ShowAlert() { showAlertView(); }
    public void ShowAlert(string str) { SetText(str); showAlertView(); }

    public void ShowLoading() { showLoading(); }
    public void CloseLoading() { closeLoading(); }

}
