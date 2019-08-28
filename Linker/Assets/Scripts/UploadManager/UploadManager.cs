using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System.Text;
using System;


[Serializable]
public class UploadResult {
    public string Url;
}

[Serializable]
public class parseUploadRes {
    public int rc;
    public int tm;
    public string srcurl;
}

[Serializable]
public class ParseToParam1 {
    public string srcurl;
    public string ver;
    public int device;
}

[Serializable]
public class ParseToParam2 {
    public int csmInUs;
    public int patchts;
    public string res;
}

[Serializable]
public class ParseToParam3 {
    public int rc;
    public int tm;
    public ParseToParam2 info;
}

public delegate void UploadComplieCallBack();

public class UploadManager : MonoBehaviour
{
    [SerializeField]
    private Button btnSelectFile_;

    [SerializeField]
    private Button btnUploadFile_;

    [SerializeField]
    private Text texSelectFilePath_;
    [SerializeField]
    private Text texLogBoard_;


    [SerializeField]
    private InputField myIFVerisonNum_;


    [SerializeField]
    private Dropdown selectDeviceDropType_;


    /// <summary>
    /// 描述上传设备类型的枚举
    /// </summary>
    private enum EnumUploadDeviceType {
        E_Android = 0,// 安卓
        E_Apple ,     // 苹果
        E_All,
    }
    private EnumUploadDeviceType myUploadDeviceType_;


    /// <summary>
    /// 描述目前上传的资源类型
    /// </summary>
    public enum EnumResState {
        E_WaitToUpLoadState = 0,    //等待上传
        E_WaitToUpdateState,        //等待更新
    }
    private EnumResState myResState_;
    public EnumResState MyResState {
        get { return myResState_; }
        set {
            if (value == EnumResState.E_WaitToUpLoadState) {
                this.uploadCdnResUrl_ = "";
            }
            this.myResState_ = value;
        }
    }


    private void registerEvents() {
        if (btnSelectFile_) {
            btnSelectFile_.onClick.AddListener(()=> {
                selectUploadFile();
            });
        }
        if (btnUploadFile_) {
            btnUploadFile_.onClick.AddListener(()=> {
                uploadFile();
            });
        }
    }

    private void setSelectFilePath(string path) {
        if (texSelectFilePath_) {
            texSelectFilePath_.text = path;
        }
    }

    private string strLog_ = "";
    private void setLogBoard(string word) {
        strLog_ = strLog_+"\n" + word;
        if (texLogBoard_) {
            texLogBoard_.text = strLog_;
        }
    }

    private void Start()
    {
        myUploadDeviceType_ = (EnumUploadDeviceType)selectDeviceDropType_.value;
        registerEvents();
    }

    private void selectUploadFile() {
        var path = OpenDialogUtils.OpenFile();
        if (path != "") {
            setSelectFilePath(path);

            // 将当前状态记录为等待上传状态
            MyResState = EnumResState.E_WaitToUpLoadState;
        }
    }

    /// <summary>
    /// 更新设备类型UI 事件
    /// </summary>
    public void UpdateSelectDeviceType() {
        if (selectDeviceDropType_ != null) {
            myUploadDeviceType_ = (EnumUploadDeviceType)selectDeviceDropType_.value;
        }
    }


    private readonly static string STR_UPLOADRESULTFILE = "uploadResultConfigFlie.json";
    /// <summary>
    /// 保存上传结果文件
    /// </summary>
    /// <param name="str">上传结果的内容</param>
    private void saveResultFile(string str) {
        UploadResult r = new UploadResult();
        r.Url = str;
        string strJson = JsonUtility.ToJson(r);
        File.WriteAllText(STR_UPLOADRESULTFILE,strJson);
    }

    // 更新资源请求的URL
    private readonly static string STR_UPDATE_RES_URL = "https://testxblapi.youban.com/srcupdate/addnewsrc";

    /// <summary>
    /// 更新补丁网址的方法
    /// </summary>
    /// <param name="resurl">要更新资源cdn的下载地址</param>
    /// <param name="ver">要更新补丁的目标版本</param>
    /// <returns></returns>
    private IEnumerator updateResToCloudAndroid(string resurl,string ver ) {
        setLogBoard("目标版本:" + ver);
        setLogBoard("开始更新安卓补丁:"+resurl);
        ParseToParam1 p = new ParseToParam1();
        p.device = 1;
        p.ver = ver;
        p.srcurl = resurl;
        var str = JsonUtility.ToJson(p);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(str);
        UnityWebRequest request = new UnityWebRequest(STR_UPDATE_RES_URL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            setLogBoard("失败 更新安卓补丁到线上失败！");
        }
        else
        {
            ParseToParam3 p3 = JsonUtility.FromJson<ParseToParam3>(request.downloadHandler.text);
            if (p3 != null)
            {
                if (p3.info != null)
                {
                    if (p3.info.res == "OK")
                    {
                        setLogBoard("成功 更新安卓补丁到线上成功");
                    }
                    else { setLogBoard("失败 更新安卓补丁到线上失败！"); }
                }
                else { setLogBoard("失败 更新安卓补丁到线上失败！"); }
            }
            else { setLogBoard("失败 更新安卓补丁到线上失败！"); }
        }
    }

    /// <summary>
    /// 更新补丁网址的方法
    /// </summary>
    /// <param name="resurl">要更新资源cdn的下载地址</param>
    /// <param name="ver">要更新补丁的目标版本</param>
    /// <returns></returns>
    private IEnumerator updateResToCloudApple( string resurl, string ver)
    {
        setLogBoard("目标版本:" + ver);
        setLogBoard("开始更新苹果补丁:" + resurl);
        ParseToParam1 p = new ParseToParam1();
        p.device = 0;
        p.ver = ver;
        p.srcurl = resurl;
        var str = JsonUtility.ToJson(p);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(str);
        UnityWebRequest request = new UnityWebRequest(STR_UPDATE_RES_URL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            setLogBoard("失败 更新苹果补丁到线上失败！");
        }
        else
        {
            ParseToParam3 p3 = JsonUtility.FromJson<ParseToParam3>(request.downloadHandler.text);
            if (p3 != null)
            {
                if (p3.info != null)
                {
                    if (p3.info.res == "OK")
                    {
                        setLogBoard("成功 更新苹果补丁到线上成功");
                    }
                    else { setLogBoard("失败 更新苹果补丁到线上失败！"); }
                }
                else { setLogBoard("失败 更新苹果补丁到线上失败！"); }
            }
            else { setLogBoard("失败 更新苹果补丁到线上失败！"); }
        }
    }

    // 上传文件到cdn 请求的URL
    private static readonly string STR_UPLOAD_REQ_URL = "http://testxblapi.youban.com/common/upload";
    //上传后资源的地址
    private string uploadCdnResUrl_ = "";
    /// <summary>
    /// 上传文件的请求
    /// </summary>
    /// <param name="path">上传文件的绝对路径</param>
    /// <returns></returns>
    private IEnumerator UploadFile(string path, UploadComplieCallBack cb) {
        byte[] b = File.ReadAllBytes(path);
        WWWForm form = new WWWForm();
        form.AddBinaryData("xblfile",b);
        UnityWebRequest req = UnityWebRequest.Post(STR_UPLOAD_REQ_URL, form);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return req.SendWebRequest();
        if (req.isHttpError||req.isNetworkError) {
            setLogBoard("网络错误 上传失败");
        } else {
            parseUploadRes p = JsonUtility.FromJson<parseUploadRes>(req.downloadHandler.text);
            setLogBoard("上传成功"+ p.srcurl);
            Debug.Log("上传成功"+ p.srcurl);
            saveResultFile(p.srcurl);
            uploadCdnResUrl_ = p.srcurl;

            // 将当前状态记录为等待更新状态
            MyResState = EnumResState.E_WaitToUpdateState;


            if (cb!=null) {
                cb();
            }


        }
    }

    /// <summary>
    /// 更新资源到目标
    /// </summary>
    private void updateResToOnline() {
        // 更新文件到目标平台
        if (this.myUploadDeviceType_ == EnumUploadDeviceType.E_All)
        {
            StartCoroutine(updateResToCloudApple(uploadCdnResUrl_, this.myIFVerisonNum_.text));// 加个回调？
            StartCoroutine(updateResToCloudAndroid(uploadCdnResUrl_, this.myIFVerisonNum_.text));
        }
        else if (this.myUploadDeviceType_ == EnumUploadDeviceType.E_Android)
        {
            StartCoroutine(updateResToCloudAndroid(uploadCdnResUrl_, this.myIFVerisonNum_.text));
        }
        else if (this.myUploadDeviceType_ == EnumUploadDeviceType.E_Apple)
        {
            StartCoroutine(updateResToCloudApple(uploadCdnResUrl_, this.myIFVerisonNum_.text));// 加个回调？
        }
    }

    /// <summary>
    /// 检查上传配置是否ok
    /// </summary>
    /// <returns></returns>
    private bool checkCouldUpload() {
        if (texSelectFilePath_.text == "") {
            setLogBoard("上传文件路径不能为空，请填写上传文件路径");
            return false;
        }

        if (!File.Exists(texSelectFilePath_.text)) {
            setLogBoard("上传文件不存在，请检查路径下，文件是否存在");
            return false;
        }

        if (myIFVerisonNum_.text == "") {
            setLogBoard("版本号不能为空，请填写补丁对应的版本号");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 是否正在上传文件的标记 防止上传过程中又上传内容
    /// </summary>
    private bool isUploading_ = false;

    /// <summary>
    /// 上传文件的指令
    /// </summary>
    private void uploadFile() {
        if (checkCouldUpload()) {
            if (this.MyResState == EnumResState.E_WaitToUpLoadState) {
                if (!isUploading_) {
                    isUploading_ = true;
                    StartCoroutine(UploadFile(texSelectFilePath_.text, () => {
                        this.updateResToOnline();
                        isUploading_ = false;
                    }));
                }
            } else if (this.MyResState == EnumResState.E_WaitToUpdateState) {
                if (this.uploadCdnResUrl_ != "") {
                    updateResToOnline();
                }
            }
        }
    }
}
