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

    private enum EnumUploadDeviceType {
        E_Android = 0,// 安卓
        E_Apple ,     // 苹果
        E_All,
    }
    private EnumUploadDeviceType myUploadDeviceType_;


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
    private void setLogBoard(string word) {
        if (texLogBoard_) {
            texLogBoard_.text = "";
            texLogBoard_.text = word;
        }
    }

    private void Start()
    {
        myUploadDeviceType_ = (EnumUploadDeviceType)selectDeviceDropType_.value;
        registerEvents();

        StartCoroutine(updateResToCloudAndroid("https://testxblapi.youban.com/srcupdate/addnewsrc",
            "https:\\/\\/youbansrc.youban.com\\/upsource\\/srcurl1566892472.zip", "8.5.7"));
    }

    private void selectUploadFile() {
        var path = OpenDialogUtils.OpenFile();
        if (path != "") {
            setSelectFilePath(path);
        }
    }


    [SerializeField]
    private Dropdown selectDeviceDropType_;
    public void UpdateSelectDeviceType() {
        if (selectDeviceDropType_ != null) {
            Debug.Log("value:"+ selectDeviceDropType_.value);
            myUploadDeviceType_ = (EnumUploadDeviceType)selectDeviceDropType_.value;
        }
    }


    private readonly static string STR_UPLOADRESULTFILE = "uploadResultConfigFlie.json";

    private void saveResultFile(string str) {
        UploadResult r = new UploadResult();
        r.Url = str;
        string strJson = JsonUtility.ToJson(r);
        File.WriteAllText(STR_UPLOADRESULTFILE,strJson);
    }
    /*
    public static IEnumerator PostHttpRequest(object requestBody, HandleResponse handler)
    {
        string body = JsonUtils.ObjectToJson(requestBody);
        byte[] rawData = Encoding.UTF8.GetBytes(body);

        WWWForm form = new WWWForm();
        Hashtable headers = form.headers;
        headers["Content-Type"] = "application/json";
        headers["Accept"] = "application/json";


        //request的body有压缩 
        //headers["Content-Encoding"] = "gzip"; 
        //客户端支持response body的压缩,接收到客户端的Accept-Encoding:gzip后,服务端根据实际情况对response的body进行gzip压缩 
        //headers["Accept-Encoding"] = "gzip"; 


        WWW www = new WWW(url, rawData, headers);


        yield return www;


        if (www.error != null)
        {
            handler(new XHttpResponseObject { code = -32767, id = -32767, desc = www.error });
        }
        else
        {
            Dictionary<string, string> responseHeader = www.responseHeaders;
            XHttpResponseObject response = JsonUtils.JsonToObject<XHttpResponseObject>(www.text);
            handler(response);
        }
    }
    */



    private IEnumerator updateResToCloudAndroid(string url,string resurl,string ver) {
        ParseToParam1 p = new ParseToParam1();
        p.device = 1;
        p.ver = ver;
        p.srcurl = resurl;
        var str = JsonUtility.ToJson(p);
        Debug.Log("向服务器发送请求：" + str);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(str);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            // todo request fail
        }
        else
        {
            // 
            string result = request.downloadHandler.text;
            Debug.Log("result:"+result);
        }
    }

    private IEnumerator updateResToCloudApple(string url, string resurl, string ver)
    {
        WWWForm form = new WWWForm();
        form.AddField("srcurl", resurl);
        form.AddField("ver", ver);
        form.AddField("device", 0);
        UnityWebRequest req = UnityWebRequest.Post(url, form);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return req.SendWebRequest();
        if (req.isHttpError || req.isNetworkError)
        {
            setLogBoard("网络错误 上传失败");
        }
        else
        {
            Debug.Log(req.downloadHandler.text);
        }
    }

    private IEnumerator UploadFile(string url,string path) {
        byte[] b = File.ReadAllBytes(path);
        WWWForm form = new WWWForm();
        form.AddBinaryData("xblfile",b);
        //form.AddField("xblfile",);
        UnityWebRequest req = UnityWebRequest.Post(url,form);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return req.SendWebRequest();
        if (req.isHttpError||req.isNetworkError) {
            setLogBoard("网络错误 上传失败");
        } else {
            parseUploadRes p = JsonUtility.FromJson<parseUploadRes>(req.downloadHandler.text);
            setLogBoard("上传成功"+ p.srcurl);
            Debug.Log("上传成功"+ p.srcurl);
            saveResultFile(p.srcurl);
        }
    }

    [SerializeField]
    private InputField myIFVerisonNum_;

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

    private void uploadFile() {
        if (checkCouldUpload())
        {
            StartCoroutine(UploadFile("http://testxblapi.youban.com/common/upload", texSelectFilePath_.text));

        }
        else {

        }
    }
}
