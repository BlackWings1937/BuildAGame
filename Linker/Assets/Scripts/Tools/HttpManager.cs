using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class HttpManager 
{

    private AppController myAppController_ = null;
    private AppController getAppController() { return myAppController_; }

    // ----- 常量 -----
    private const string STR_PORT = "443";
    private const string STR_ZIP_FILE_NAME = "LinkerData";

    // ----- 私有成员 -----
    private HttpListener myHttpListener_ = null;
    private PackResManager myPackResManager_ = null;

    // ----- 私有方法 -----

    private void handleRequest(IAsyncResult ar) {
        myHttpListener_.BeginGetContext((IAsyncResult nar) => {
            this.handleRequest(nar);
        }, null);
        var exePath = System.Environment.CurrentDirectory+"\\";
        var zipResPath = exePath+ STR_ZIP_FILE_NAME+".zip";
        if (File.Exists(zipResPath))
        {
            var bytesData = File.ReadAllBytes(zipResPath);
            var sizeOfBytesData = bytesData.Length;

            var context = myHttpListener_.EndGetContext(ar);
            var req = context.Request;
            var resp = context.Response;
            resp.ContentType = "application/zip";
            resp.AddHeader("Content-type", "application/zip");
            resp.AddHeader("Content-Length", "" + sizeOfBytesData);
            resp.AddHeader("Accept-Ranges", "bytes");
            resp.AddHeader("Content-Disposition", "attachment; filename = "+ STR_ZIP_FILE_NAME+".zip");

            try {
                using (var stream = resp.OutputStream) {
                    stream.Write(bytesData,0,sizeOfBytesData);
                }
            } catch (Exception e) {
                UnityEngine.Debug.Log("Error: net fail:"+e.ToString());
            }
        }
        else {
            UnityEngine.Debug.Log("Error: file == null atPath:"+zipResPath);
        }
    }


    private void startHttpServer() {
        myHttpListener_ = new HttpListener();
        myHttpListener_.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        myHttpListener_.Prefixes.Add("http://+:"+ STR_PORT + "/");
        myHttpListener_.Start();
        myHttpListener_.BeginGetContext((IAsyncResult ar) => {
            this.handleRequest(ar);
        },null);
    }
    private void stopHttpServer() {
        myHttpListener_.Stop();
        myHttpListener_ = null;
    }

    private void initPackResManager() {
        myPackResManager_ = new PackResManager();
        myPackResManager_.SetHttpManager(this);
    }
    private void disposePackResManager() {
        myPackResManager_ = null;
    }

    // ----- 对外接口 -----
    public void Start() {
        startHttpServer();
        initPackResManager();
    }
    public void Stop() {
        stopHttpServer();
        disposePackResManager();
    }
    public void UpdateRes() {
        if (this.myPackResManager_ != null) {
            this.myPackResManager_.PrepareFloder();
            this.myPackResManager_.SetListOfMark(new List<string>());
            this.myPackResManager_.CopyLinkerDataToAim();
            this.myPackResManager_.ZipToServer();
        }
    }
    public void UpdateResByAimFloder(List<string> list) {
        if (this.myPackResManager_ != null)
        {
            this.myPackResManager_.PrepareFloder();
            this.myPackResManager_.SetListOfMark(list);
            this.myPackResManager_.CopyLinkerDataToAim();
            this.myPackResManager_.ZipToServer();
        }
    }
    public void SetAppController(AppController v) { myAppController_ = v; }
    public string GetLinkerDataOrigPath() {
        return getAppController().GetTargetPackageInfo()[ProjData.STR_PATH] as string;
    }
    public string GetZipDataFileName() {
        return STR_ZIP_FILE_NAME + ".zip";
    }
}
