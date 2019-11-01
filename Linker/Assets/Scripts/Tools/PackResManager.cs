using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PackResManager 
{


    // ----- 常量 -----
    private const string STR_DEVICE_NUM = "D";
    private string[] strPaths = {
        "57",
        "LinkerTestPackage",
        "task",
        "LinkerData"
    };

    // ----- 私有成员 -----
    private HttpManager myHttpManager_ = null;
    private string aimPath_ = "";

    // ----- 私有方法 -----
    public void copyDir(string srcPath, string aimPath)
    {
        try
        {

            //如果不存在目标路径，则创建之

            if (!System.IO.Directory.Exists(aimPath))
            {
                System.IO.Directory.CreateDirectory(aimPath);
            }
            //令目标路径为aimPath\srcPath
            string srcdir = System.IO.Path.Combine(aimPath, System.IO.Path.GetFileName(srcPath));
            //如果源路径是文件夹，则令目标目录为aimPath\srcPath\
            if (Directory.Exists(srcPath))
                srcdir += Path.DirectorySeparatorChar;
            // 如果目标路径不存在,则创建目标路径
            if (!System.IO.Directory.Exists(srcdir))
            {
                System.IO.Directory.CreateDirectory(srcdir);
            }
            //获取源文件下所有的文件
            String[] files = Directory.GetFileSystemEntries(srcPath);
            foreach (string element in files)
            {
                // UnityEngine.Debug.LogError("copyFIle:"+ element);
                //如果是文件夹，循环
                if (Directory.Exists(element))
                    copyDir(element, srcdir);
                else
                    File.Copy(element, srcdir + Path.GetFileName(element), true);
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("无法复制" + e.Message);
            Console.WriteLine("无法复制");
        }
    }

    // ----- 对外接口 -----
    public void PrepareFloder() {
        var oriPath = STR_DEVICE_NUM + ":";
        if (Directory.Exists(oriPath + "\\" + strPaths[0])) {
            Directory.Delete(oriPath + "\\" + strPaths[0], true);

        }
        var count = strPaths.Length;
        for (int i = 0;i<count;++i) {
            var strNode = strPaths[i];
            oriPath = oriPath + "\\" + strNode;
            if (Directory.Exists(oriPath)) {

            } else {
                Directory.CreateDirectory(oriPath);
            }
        }
        aimPath_ = oriPath;
    }
    public void CopyLinkerDataToAim() {
        var oriPath = GetHttpManager().GetLinkerDataOrigPath();
        var aimPath = aimPath_;
        copyDir(oriPath,aimPath);
    }
    public void ZipToServer() {
        var zipPath = System.Environment.CurrentDirectory + "\\" + GetHttpManager().GetZipDataFileName();
        var waitForZipPath = aimPath_;//+ "\\" + strPaths[strPaths.Length - 1];// LinkerData";
        var winRarPath = System.Environment.CurrentDirectory + "/WinRAR/WinRAR.exe";
        var cmd = winRarPath + " a -r  -ibck " + zipPath + " " + waitForZipPath;
        CmdUtils.Run(cmd);
    }
    public void SetHttpManager(HttpManager v) { myHttpManager_ = v; }
    public HttpManager GetHttpManager() { return myHttpManager_; }
}
