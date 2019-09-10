using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
[Serializable]
public class ParseToAimModWin32ConfigFile
{
    public string Version;
    public string InstallMode;
}

public class CanvasManager : MonoBehaviour
{
    [SerializeField]
    private GameObject myOpenModPanel_ = null;
    [SerializeField]
    private GameObject myEditModPanel_ = null;

    private BuildModCheckFIles myFileChecker_ = new BuildModCheckFIles();

    private string vsCodePath_ = "";

    private void setCanvasManagerToChildManager()
    {
        myOpenModPanel_.GetComponent<CreateModManager>().SetParentManager(this);
        myEditModPanel_.GetComponent<UploadManager>().SetParentManager(this);
    }

    private void Start()
    {


        setCanvasManagerToChildManager();
        EnterOpenModPanel();


    }



    private void closeAllPanel()
    {
        myOpenModPanel_.SetActive(false);
        myEditModPanel_.SetActive(false);
    }

    // ----- Controller -----
    private void tryCreateFloderForPathByStrArr(string[] arr, string rootPath = "")
    {
        var nowPath = rootPath;
        for (int i = 0; i < arr.Length; ++i)
        {
            nowPath = nowPath + "\\" + arr[i];
            if (!Directory.Exists(nowPath))
            {
                Directory.CreateDirectory(nowPath);
            }
        }
    }

    // ----- 对外接口 -----
    public void EnterOpenModPanel()
    {
        closeAllPanel();
        myOpenModPanel_.SetActive(true);
    }
    public void EnterEditModPanel(string xblProjPath, string xblAimVersion, string vsCodePath)
    {
        if (myFileChecker_ != null)
        {
            vsCodePath_ = myFileChecker_.GetFileFolderPath(vsCodePath);
            UnityEngine.Debug.Log("vsCodePath_:" + vsCodePath_);
        }
        // 创建文件夹路径
        string[] arrPathCache = { "xiaobanlong5.2.0", "Win32TestRes", "Mod", "Cache" };
        string[] arrPathVersion = { "xiaobanlong5.2.0", "Win32TestRes", "Mod", xblAimVersion };
        tryCreateFloderForPathByStrArr(arrPathCache, xblProjPath);
        tryCreateFloderForPathByStrArr(arrPathVersion, xblProjPath);

        // 打开界面
        closeAllPanel();
        myEditModPanel_.SetActive(true);

        // 设置数据
        myEditModPanel_.GetComponent<UploadManager>().PathXBLProj = xblProjPath;
        myEditModPanel_.GetComponent<UploadManager>().VersionXBLMod = xblAimVersion;

    }
    private void safeDeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    public void DeleteAllConfigFIle(string xblProjPath, string xblAimVersion)
    {
        var listOfDeleteFiles = new string[]{
            xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion +"\\downloadComplie.json"
            //xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion + "\\DragonBoneCheckList.json",
            //xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion + "\\ModAllFilesList.json"
        };
        for (int i = 0; i < listOfDeleteFiles.Length; ++i)
            safeDeleteFile(listOfDeleteFiles[i]);
    }

    /// <summary>
    /// 指导win32启动mod 的指导文件
    /// </summary>
    private static readonly string STR_WIN32AIMMODFILE_NAME = "xiaobanlong5.2.0\\Win32TestRes\\Win32ModAimConfig.json";

    /// <summary>
    /// versionModData 
    /// </summary>
    private static readonly string STR_VERSIONMODDATA_FILENAME = "xiaobanlong5.2.0\\Win32TestRes\\ModMangerData.json";
    /// <summary>
    /// 生成指定win32 Mod 测试文件
    /// </summary>
    /// <param name="xblProjPath">小伴龙项目根目录</param>
    /// <param name="xblAimVersion">小伴龙Mod目标版本</param>
    public void GenerateWin32AimModFile(string xblProjPath, string xblAimVersion, bool isPretentDevice)
    {
        var p = new ParseToAimModWin32ConfigFile();
        p.Version = xblAimVersion;
        if (isPretentDevice)
        {
            p.InstallMode = "isPretentDevice";
        }
        else
        {
            p.InstallMode = "notPretentDevice";
        }
        var str = JsonUtility.ToJson(p);
        File.WriteAllText(xblProjPath + "\\" + STR_WIN32AIMMODFILE_NAME, str);
        var pathOfVersionModManagerData = xblProjPath + "\\" + STR_VERSIONMODDATA_FILENAME;
        if (File.Exists(pathOfVersionModManagerData))
        {
            File.Delete(pathOfVersionModManagerData);
        }
    }


    private static readonly string STR_PROJRESOURCEPATH = "xiaobanlong5.2.0\\Resources";
    private static readonly string STR_MODPATH = "xiaobanlong5.2.0\\Win32TestRes\\Mod";
    /// <summary>
    /// 生成Mod差异文件
    /// </summary>
    /// <param name="xblProjPath">小伴龙项目根目录</param>
    /// <param name="xblAimVersion">小伴龙Mod目标版本</param>
    public void GenerateDiffConfig(string xblProjPath, string xblAimVersion)
    {
        if (myFileChecker_ != null)
        {
            myFileChecker_.BuildFile(xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion, xblProjPath + "\\" + STR_PROJRESOURCEPATH);
        }
    }

    /// <summary>
    /// 生成Mod中所有的文件 用来自检文件完整性
    /// </summary>
    /// <param name="xblProjPath"> xbl项目路径 </param>
    /// <param name="xblAimVersion"> xblmod目标版本号 </param>
    public void GenerateModAllFilesConfig(string xblProjPath, string xblAimVersion)
    {
        if (myFileChecker_ != null)
        {
            myFileChecker_.BuildAllFilesListConfigFile(
                xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion
                );
        }
    }

    /// <summary>
    /// 生成下载完成文件
    /// </summary>
    /// <param name="xblProjPath"> xbl项目路径 </param>
    /// <param name="xblAimVersion"> xblmod目标版本号 </param>
    public void GenerateDownloadComplieFile(string xblProjPath, string xblAimVersion)
    {
        if (myFileChecker_ != null)
        {
            myFileChecker_.BuildDownloadComplieFile(xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion);
        }
    }

    /// <summary>
    /// 生成测试补丁文件
    /// </summary>
    /// <param name="xblProjPath"> xbl项目路径 </param>
    /// <param name="xblAimVersion"> xblmod目标版本号 </param>
    public void GenerateTestModFile(string xblProjPath, string xblAimVersion)
    {
        var testModFilePath = xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion + "\\debugMode.json";
        var strData = "";
        if (File.Exists(testModFilePath)) { File.Delete(testModFilePath); }
        File.WriteAllText(testModFilePath, strData);
    }

    public void GenerateShipStone(string xblProjPath, string xblAimVersion)
    {
        var copyToPath = xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion + "\\ShipHeaveStone.png";
        if (File.Exists(copyToPath)) { File.Delete(copyToPath); }
        var copyFromPath = "ShipHeaveStone.png";
        File.Copy(copyFromPath, copyToPath);
    }

    /// <summary>
    /// 缓存准本上传的Mod
    /// </summary>
    /// <param name="xblProjPath"></param>
    /// <param name="xblAimVersion"></param>
    public void CacheToUploadMod(string xblProjPath, string xblAimVersion)
    {
        var modRootPath = xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion;
        var cachePath = xblProjPath + "\\" + STR_MODPATH + "\\" + "tmpCache";
        if (Directory.Exists(cachePath))
        {
            Directory.Delete(cachePath, true);
        }
        Directory.CreateDirectory(cachePath);
        copyDir(modRootPath, cachePath);

    }

    /// <summary>
    /// 从缓存中恢复准本上传的Mod
    /// </summary>
    /// <param name="xblProjPath"></param>
    /// <param name="xblAimVersion"></param>
    public void RecoverToUploadMod(string xblProjPath, string xblAimVersion)
    {
        var modRootPath = xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion;
        var cachePath = xblProjPath + "\\" + STR_MODPATH + "\\" + "tmpCache";
        if (Directory.Exists(cachePath))
        {
         //   if (Directory.Exists(modRootPath))
            {
               // Directory.Delete(modRootPath, true);
            }
            
            //UnityEngine.Debug.LogError("copy start");
       //     Directory.CreateDirectory(modRootPath);
            copyDir(cachePath+"\\"+ xblAimVersion, xblProjPath + "\\" + STR_MODPATH + "\\");
            DeleteFileByExName(modRootPath,"luac");
            //UnityEngine.Debug.LogError("copy finish");

            // Directory.Delete(cachePath,true);
        }
    }

    /// <summary>
    /// 生成加密文件
    /// </summary>
    /// <param name="xblProjPath"> xbl项目路径 </param>
    /// <param name="xblAimVersion"> xblmod目标版本号 </param>
    public void GenerateEncryptLuaScript(string xblProjPath, string xblAimVersion)
    {
        //todo 
        //1:先将lua32和lua64文件夹都创建出来（用lua32复制一份lua64出来）
        //2:加密整个mod
        //3:删除所有lua 源文件
        var modRootPath = xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion;
        var strLua64Path = modRootPath + "\\lua64";
        var strLua32Path = modRootPath + "\\lua32\\baseScripts";
        if (!Directory.Exists(strLua32Path)) { Directory.CreateDirectory(strLua32Path); }
        if (Directory.Exists(strLua64Path))
        {
            Directory.Delete(strLua64Path, true);
        }
        copyDir(strLua32Path, strLua64Path);


        RunCmd(
             "python " + System.Environment.CurrentDirectory + "/cocos2d-console/bin/" + "cocos.py luacompile -s "
             + modRootPath + " -d " //lua 源文件路径
             + modRootPath +        //lua 加密后文件路径
             " -e -k YbLuAK123 -b YbSiGn312"
        );
        AddNameTailFileByExName(modRootPath,"lua","64");
        RunCmd(
             "python " + System.Environment.CurrentDirectory + "/cocos2d-console/bin/" + "cocos.py luacompile -s "
             + modRootPath + " -d " //lua 源文件路径
             + modRootPath +        //lua 加密后文件路径
             " -e -k YbLuAK123 -b YbSiGn312 --bytecode-64bit"
        );
        UnityEngine.Debug.Log("startTo deleteFile");
        DeleteFileByExName(modRootPath, "lua");
    }


    /// <summary>
    /// 递归修改文件名文件夹下面某个扩展名的所有文件
    /// </summary>
    /// <param name="path">目标根文件夹</param>
    /// <param name="exName">扩展文件名</param>
    private void AddNameTailFileByExName(string path, string exName,string addTail)
    {

        DirectoryInfo theFolder = new DirectoryInfo(path);

        //遍历文件
        foreach (FileInfo NextFile in theFolder.GetFiles())
        {
            Console.WriteLine(NextFile.Name);
            string[] strArr = NextFile.Name.Split('.');
            var exNameWithPoint = "." + exName;
            if (NextFile.Extension == exNameWithPoint)
            {
                var noExName = NextFile.FullName.Remove(NextFile.FullName.Length - exNameWithPoint.Length, exNameWithPoint.Length);
                UnityEngine.Debug.Log("noExName:"+ noExName);
                File.Move(NextFile.FullName, noExName+ addTail+ exNameWithPoint);
            }
        }

        //遍历文件夹
        foreach (DirectoryInfo NextFolder in theFolder.GetDirectories())
        {
            Console.WriteLine(NextFolder.FullName);
            AddNameTailFileByExName(NextFolder.FullName, exName,addTail);
        }
    }

    /// <summary>
    /// 递归删除文件夹下面某个扩展名的所有文件
    /// </summary>
    /// <param name="path">目标根文件夹</param>
    /// <param name="exName">扩展文件名</param>
    private void DeleteFileByExName(string path, string exName)
    {

        DirectoryInfo theFolder = new DirectoryInfo(path);

        //遍历文件
        foreach (FileInfo NextFile in theFolder.GetFiles())
        {
            Console.WriteLine(NextFile.Name);
            string[] strArr = NextFile.Name.Split('.');
            UnityEngine.Debug.Log(" NextFile.Extension:" + NextFile.Extension);
            if (NextFile.Extension == "."+exName)
            {
                File.Delete(NextFile.FullName);
            }
        }

        //遍历文件夹
        foreach (DirectoryInfo NextFolder in theFolder.GetDirectories())
        {
            Console.WriteLine(NextFolder.FullName);
            DeleteFileByExName(NextFolder.FullName, exName);
        }
    }

    /// <summary>
    /// 删除所有lua文件
    /// </summary>
    /// <param name="xblProjPath"> xbl项目路径 </param>
    /// <param name="xblAimVersion"> xblmod目标版本号 </param>
    public void DeleteAllLuaFile(string xblProjPath, string xblAimVersion)
    {

    }

    /// <summary>
    /// 删除测试补丁文件
    /// </summary>
    /// <param name="xblProjPath"></param>
    /// <param name="xblAimVersion"></param>
    public void DeleteTestModFile(string xblProjPath, string xblAimVersion)
    {
        var testModFilePath = xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion + "\\debugMode.json";
        if (File.Exists(testModFilePath)) { File.Delete(testModFilePath); }
    }

    /// <summary>
    /// 生成压缩包放到上传缓存中
    /// </summary>
    /// <param name="xblProjPath"> xbl项目路径 </param>
    /// <param name="xblAimVersion"> xblmod目标版本号 </param>
    public void GenerateModZipPackageToUploadCache(string xblProjPath, string xblAimVersion)
    {

        var waitToZipPath = "d:/Mod";
        if (Directory.Exists(waitToZipPath))
        {
            Directory.Delete(waitToZipPath, true);
        }
        copyDir(xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion, waitToZipPath);
        Directory.Move(waitToZipPath + "/" + xblAimVersion, waitToZipPath + "/Cache");

        /**/
        if (File.Exists(xblProjPath + "\\" + STR_MODPATH + "\\readyToUploadMod.zip"))
        {
            File.Delete(xblProjPath + "\\" + STR_MODPATH + "\\readyToUploadMod.zip");
        }
        var winRarPath = System.Environment.CurrentDirectory + "/WinRAR/WinRAR.exe";
        UnityEngine.Debug.Log("mark path:" + winRarPath);
        UnityEngine.Debug.Log("path mark 1:" + System.Environment.CurrentDirectory + "/Resource");
        var cmd = winRarPath + " a -r  -ibck " + xblProjPath + "\\" + STR_MODPATH + "\\readyToUploadMod.zip" + " " + waitToZipPath;
        UnityEngine.Debug.Log("cmd:" + cmd);
        CanvasManager.RunCmd(cmd);

    }

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
        catch(Exception e)
        {
            UnityEngine.Debug.LogError("无法复制"+e.Message);
            Console.WriteLine("无法复制");
        }
    }


    /// <summary>E:\\xblTempPackage5\\xiaobanlong\\xiaobanlong5.2.0\\Win32TestRes\\Mod
    /// 打开vsCode 编辑Mod
    /// </summary>
    /// <param name="xblProjPath">小伴龙项目路径</param>
    /// <param name="xblAimVersion">xblmod目标版本号 </param>
    public void OpenVSCodeToEditMod(string xblProjPath, string xblAimVersion)
    {
        CanvasManager.RunCmd("cd " + System.Environment.CurrentDirectory + "/VsCode");
        CanvasManager.RunCmd("Code " + xblProjPath + "\\" + STR_MODPATH + "\\" + xblAimVersion);
    }



    private static string CmdPath = @"C:\Windows\System32\cmd.exe";
    public static string RunCmd(string cmd)
    {
        UnityEngine.Debug.Log("RunCmd1");
        cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
        using (Process p = new Process())
        {
            UnityEngine.Debug.Log("RunCmd2");

            p.StartInfo.FileName = CmdPath;
            p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口写入命令
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.AutoFlush = true;

            //获取cmd窗口的输出信息
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            UnityEngine.Debug.Log("RunCmd3");

            return output;
        }
    }

}
