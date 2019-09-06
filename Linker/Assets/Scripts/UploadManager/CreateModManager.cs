using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

[Serializable]
public class AppConfigParse {
    public string XblProjPath;
    public string XblModAimVersion;
    public string VsCodePath;
}

public class CreateModManager : MonoBehaviour
{
    //小伴龙工程路径 text
    [SerializeField]
    private Text texXBLProjPath_ = null;

    //小伴龙Mod 版本号 inputfield
    [SerializeField]
    private InputField ifXBLVersion_ = null;

    //选择小伴龙项目路径的按钮
    [SerializeField]
    private Button btnSelectProjPath_ = null;

    //创建mod 的按钮
    [SerializeField]
    private Button btnCreateMod_ = null;

    //vscode 路径
    [SerializeField]
    private Text texVSCodePath_ = null;

    //按钮选择vscode 路径
    [SerializeField]
    private Button btnSelectVsCodePath_ = null;

    // ----- init Data -----
    private AppConfigParse myAppConfigData_ = null;
    private static readonly string STR_APPCONFIG_FILE_NAME = "appConfigFile.json";
    private void initAppConfig() {
        if (File.Exists(STR_APPCONFIG_FILE_NAME))
        {
            string str = File.ReadAllText(STR_APPCONFIG_FILE_NAME);
            myAppConfigData_ = JsonUtility.FromJson<AppConfigParse>(str);
        }
        else {
            myAppConfigData_ = new AppConfigParse();
            myAppConfigData_.XblModAimVersion = "";
            myAppConfigData_.XblProjPath = "";
            myAppConfigData_.VsCodePath = "";
            saveAppConfigDataToFile();
        }
        dataUpdated();
    }
    private void saveAppConfigDataToFile() {
        if (myAppConfigData_!= null) {
            string str = JsonUtility.ToJson(myAppConfigData_);
            File.WriteAllText(STR_APPCONFIG_FILE_NAME,str);
        }
    }
    private void dataUpdated() {
        updateUIFromData();
        saveAppConfigDataToFile();
    }

    private void setXBLProjPath(string p) {
        if (myAppConfigData_!= null) {
            myAppConfigData_.XblProjPath = p;
            dataUpdated();
        }
    }

    private void setVsCodePath(string p) {
        if (myAppConfigData_!= null) {
            myAppConfigData_.VsCodePath = p;
            dataUpdated();
        }
    }

    private void setXBLAimVersion(string p) {
        if (myAppConfigData_!=null) {
            myAppConfigData_.XblModAimVersion = p;
            dataUpdated();
        }
    }

    // ----- init UI -----
    private void registerUIEvent() {
        btnSelectProjPath_.onClick.AddListener(()=> {
            onBtnClickSelectPath();
        });
        btnCreateMod_.onClick.AddListener(()=> {
            onBtnClickCreateMod();
        });
        btnSelectVsCodePath_.onClick.AddListener(()=> {
            onBtnClickSelectVsCodePath();
        });
    }

    private void initUi() {
        registerUIEvent();
    }

    private void updateUIFromData()
    {
        if (texXBLProjPath_ != null)
        {
            texXBLProjPath_.text = myAppConfigData_.XblProjPath;
            ifXBLVersion_.text = myAppConfigData_.XblModAimVersion;
            texVSCodePath_.text = myAppConfigData_.VsCodePath;
        }
    }

    // ----- Init -----
    private void initApp() {
        initAppConfig();
        initUi();
    }
    void Start()
    {
        initApp();
    }

    private CanvasManager parentManager_ = null;
    public void SetParentManager(CanvasManager p) { parentManager_ = p; }

    // ----- Controller -----
    private readonly static string STR_XBLPROJPATHCHILDFLODER = "xiaobanlong5.2.0";
    private bool checkXBLProjPathCorrect() {
        if (myAppConfigData_ == null) return false;
        return Directory.Exists(myAppConfigData_.XblProjPath + "\\" + STR_XBLPROJPATHCHILDFLODER);
    }

    // ----- UI 事件 -----
    private void onBtnClickSelectPath() {
        var path = OpenDialogUtils.OpenDir();
        if (path!="") {
            setXBLProjPath(path);
        }
    }

    private readonly static string STR_XBLPROJPATHERROR = "选择的小伴龙项目路径错误";
    private void onBtnClickCreateMod() {
        setXBLAimVersion(ifXBLVersion_.text);

        if (checkXBLProjPathCorrect() == false) {
            texXBLProjPath_.text = STR_XBLPROJPATHERROR;
            return;
        }
        if (parentManager_!= null && myAppConfigData_ != null) {
            //debug
            Debug.Log("vscodepath:"+myAppConfigData_.VsCodePath);
            parentManager_.EnterEditModPanel(myAppConfigData_.XblProjPath,myAppConfigData_.XblModAimVersion,myAppConfigData_.VsCodePath);
        }
    }

    private void onBtnClickSelectVsCodePath() {
        var path = OpenDialogUtils.OpenFile();
        if (path!= "") {
            setVsCodePath(path);
        }
    }

    // ----- 私有成员方法 -----

}
