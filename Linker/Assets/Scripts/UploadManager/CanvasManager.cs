using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Newtonsoft.Json;

[Serializable]
public class ParseToAimModWin32ConfigFile {
    public string Version;
}

public class CanvasManager : MonoBehaviour
{
    [SerializeField]
    private GameObject myOpenModPanel_ = null;
    [SerializeField]
    private GameObject myEditModPanel_ = null;

    private BuildModCheckFIles myFileChecker_ = new BuildModCheckFIles();

    private void setCanvasManagerToChildManager() {
        myOpenModPanel_.GetComponent<CreateModManager>().SetParentManager(this);
        myEditModPanel_.GetComponent<UploadManager>().SetParentManager(this);
    }

    private void Start()
    {


        setCanvasManagerToChildManager();
        EnterOpenModPanel();

   
    }



    private void closeAllPanel() {
        myOpenModPanel_.SetActive(false);
        myEditModPanel_.SetActive(false);
    }

    // ----- Controller -----
    private void tryCreateFloderForPathByStrArr( string [] arr,string rootPath = "") {
        var nowPath = rootPath ;
        for (int i = 0;i<arr.Length;++i) {
            nowPath = nowPath +"\\"+ arr[i];
            if (!Directory.Exists(nowPath)) {
                Directory.CreateDirectory(nowPath);
            }
        }
    }

    // ----- 对外接口 -----
    public void EnterOpenModPanel() {
        closeAllPanel();
        myOpenModPanel_.SetActive(true);
    }
    public void EnterEditModPanel(string xblProjPath,string xblAimVersion) { 
        // 创建文件夹路径
        string[] arrPathCache = { "xiaobanlong5.2.0", "Win32TestRes","Mod","Cache" };
        string[] arrPathVersion = { "xiaobanlong5.2.0", "Win32TestRes", "Mod",xblAimVersion };
        tryCreateFloderForPathByStrArr(arrPathCache,xblProjPath);
        tryCreateFloderForPathByStrArr(arrPathVersion,xblProjPath);

        // 打开界面
        closeAllPanel();
        myEditModPanel_.SetActive(true);

        // 设置数据
        myEditModPanel_.GetComponent<UploadManager>().PathXBLProj = xblProjPath;
        myEditModPanel_.GetComponent<UploadManager>().VersionXBLMod = xblAimVersion;
    }



    /// <summary>
    /// 指导win32启动mod 的指导文件
    /// </summary>
    private static readonly string STR_WIN32AIMMODFILE_NAME = "xiaobanlong5.2.0\\Win32TestRes\\Win32ModAimConfig.json";
   
    /// <summary>
    /// 生成指定win32 Mod 测试文件
    /// </summary>
    /// <param name="xblProjPath">小伴龙项目根目录</param>
    /// <param name="xblAimVersion">小伴龙Mod目标版本</param>
    public void GenerateWin32AimModFile(string xblProjPath,string xblAimVersion) {
        var p = new ParseToAimModWin32ConfigFile();
        p.Version = xblAimVersion;
        var str = JsonUtility.ToJson(p);
        File.WriteAllText(xblProjPath+"\\"+STR_WIN32AIMMODFILE_NAME,str);
    }

    private static readonly string STR_PROJRESOURCEPATH = "xiaobanlong5.2.0\\Resources";
    private static readonly string STR_MODPATH = "xiaobanlong5.2.0\\Win32TestRes\\Mod";
    /// <summary>
    /// 生成Mod差异文件
    /// </summary>
    /// <param name="xblProjPath">小伴龙项目根目录</param>
    /// <param name="xblAimVersion">小伴龙Mod目标版本</param>
    public void GenerateDiffConfig(string xblProjPath,string xblAimVersion) {
        Debug.Log("GenerateDiffConfig1");
        if (myFileChecker_!= null) {
            Debug.Log("GenerateDiffConfig2");

            myFileChecker_.BuildFile(xblProjPath+"\\"+ STR_MODPATH+"\\"+ xblAimVersion, xblProjPath+"\\"+ STR_PROJRESOURCEPATH);
        }
    }
}
