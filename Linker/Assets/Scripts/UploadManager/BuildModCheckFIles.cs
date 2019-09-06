using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
public partial class BuildModCheckFIles
{
    private static readonly string CONFIG_FILE = "MODECONFIG.cfg";
    private static readonly string PROJRES = "PROJRES";
    private static readonly string MODRES = "MODRES";
    private static readonly string MOD = "Mod";
    private static readonly string RES = "Resources";
    private static readonly string STR_TYPE = "Type";
    private static readonly string STR_ADD = "Add";
    private static readonly string STR_CHANGE = "Change";
    private static readonly string STR_PLIST = "Plist";
    private static readonly string STR_XML = "Xml";
    private static readonly string STR_FILE_TYPE = "FileType";
    private static readonly string STR_UNKNOW = "Unknow";
    private static readonly string STR_DIC_ADD_ITEM = "AddItems";
    private static readonly string STR_DIC_Change_ITEM = "ChangeItems";
    public Dictionary<string, object> config_;

    public BuildModCheckFIles()
    {
    }

    private void MbLabelTextBox2_Load(object sender, EventArgs e)
    {

    }

    private void MbLabelTextBox1_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 龙骨增改文件列表
    /// </summary>
    private readonly static string STR_DRAGONBONECHECKLIST_FILE = "DragonBoneCheckList.json";

    /// <summary>
    /// 创建龙骨增改文件描述文件
    /// </summary>
    /// <param name="modRootPath">mod 路径</param>
    /// <param name="projResourcePath">项目 res 路径</param>
    public void BuildFile( string modRootPath,string projResourcePath)
    {
        Dictionary<string, object> dicChangeAddItem = new Dictionary<string, object>();
        Dictionary<string, object> dicAddItem = new Dictionary<string, object>();
        Dictionary<string, object> dicall = new Dictionary<string, object>();
        listDirectory(projResourcePath, modRootPath, dicChangeAddItem, dicAddItem);
        dicall.Add(STR_DIC_Change_ITEM, dicChangeAddItem);
        dicall.Add(STR_DIC_ADD_ITEM, dicAddItem);
        File.WriteAllText(modRootPath + "\\"+ STR_DRAGONBONECHECKLIST_FILE, JsonConvert.SerializeObject(dicall));
    }


    /// <summary>
    /// 描述mod 全部文件的列表
    /// </summary>
    private readonly static string STR_FILES_LIST_CONFIG_FILE_NAME = "ModAllFilesList.json";

    /// <summary>
    /// 创建补丁全文件描述文件
    /// </summary>
    /// <param name="modRootPath">mod 路径</param>
    public void BuildAllFilesListConfigFile(string modRootPath) {
        List<string> listFileList = new List<string>();
        foreachFolderForCreateAllFilesConfig(modRootPath,listFileList);
        var str = JsonConvert.SerializeObject(listFileList);
        Debug.Log(str);
        File.WriteAllText(modRootPath+"\\"+ STR_FILES_LIST_CONFIG_FILE_NAME, str);
    }



    private void foreachFolderForCreateAllFilesConfig(string path,List<string> list)
    {

        DirectoryInfo theFolder = new DirectoryInfo(path);

        //遍历文件
        foreach (FileInfo NextFile in theFolder.GetFiles())
        {
            Console.WriteLine(NextFile.Name);
            if (NextFile.Name != STR_DOWNLOADCOMPLIEFILE &&NextFile.Name!= STR_FILES_LIST_CONFIG_FILE_NAME&&NextFile.Name != STR_DRAGONBONECHECKLIST_FILE) {
                string[] strArr = NextFile.FullName.Split('\\');
                string aimPath = this.cutStringByStr(strArr, MOD, false, 1);
                list.Add(aimPath);
            }
        }

        //遍历文件夹
        foreach (DirectoryInfo NextFolder in theFolder.GetDirectories())
        {
            Console.WriteLine(NextFolder.FullName);
            foreachFolderForCreateAllFilesConfig(NextFolder.FullName,list);

        }
    }

    /// <summary>
    /// 模拟下载完成创建出来的文件
    /// </summary>
    private readonly static string STR_DOWNLOADCOMPLIEFILE = "downloadComplie.json";
    public void BuildDownloadComplieFile(string modRootPath) {
        var dic = new Dictionary<string, object>();
        dic.Add("IsDownLoadComplie","true");
        var str = JsonConvert.SerializeObject(dic);
        Debug.Log(str);
        File.WriteAllText(modRootPath + "\\" + STR_DOWNLOADCOMPLIEFILE, str);
    }


    public string GetFileFolderPath(string path) {
        string[] strArr = path.Split('\\');
        var str = "";
        for (int i = 0;i<strArr.Length-1;++i) {
            str = str + strArr[i];
        }
        return str;
    }


    private void listDirectory(string projResPath, string path, Dictionary<string, object> dicChangeAddItem, Dictionary<string, object> dicAddItem)
    {

        DirectoryInfo theFolder = new DirectoryInfo(path);

        //遍历文件
        foreach (FileInfo NextFile in theFolder.GetFiles())
        {
            Console.WriteLine(NextFile.Name);
            if (NextFile.Name.Contains(".plist") || NextFile.Name.Contains(".xml"))
            {
                InsertAddChangeFileItem(projResPath, NextFile.FullName, dicChangeAddItem, dicAddItem);
            }
        }

        //遍历文件夹
        foreach (DirectoryInfo NextFolder in theFolder.GetDirectories())
        {
            Console.WriteLine(NextFolder.FullName);
            listDirectory(projResPath, NextFolder.FullName, dicChangeAddItem, dicAddItem);

        }
    }
    private string cutStringByStr(string[] strArr, string str, bool isFront, int diff = 0)
    {
        var aimIndex = -1;
        for (int i = 0; i < strArr.Length; ++i)
        {
            if (str == strArr[i]) { aimIndex = i; break; }
        }
        var result = "";
        if (aimIndex != -1)
        {
            aimIndex = aimIndex + diff;
            var startindex = 0;
            var endindex = 0;
            if (isFront)
            {
                startindex = 0; endindex = aimIndex;
            }
            else
            {
                startindex = aimIndex + 1; endindex = strArr.Length;
            }
            for (int i = startindex; i < Math.Max(startindex, endindex - 1); ++i) { result = result + strArr[i] + "/"; }
            var lastIndex = Mathf.Max(Mathf.Min(strArr.Length-1, Math.Max(startindex, endindex - 1)),0);
            result = result + strArr[lastIndex];
        }
        return result;
    }

    private void InsertAddChangeFileItem(string projResPath, string path, Dictionary<string, object> dicChangeAddItem, Dictionary<string, object> dicAddItem)
    {

        string[] strArr = path.Split('\\');
        string aimPath = this.cutStringByStr(strArr, MOD, false, 1);
        strArr = projResPath.Split('\\');
        string oriPath = this.cutStringByStr(strArr, RES, true) + "\\" + RES + "\\" + aimPath;
        
        Debug.Log("oriPath:" + oriPath);
        Debug.Log("aimPath:" + aimPath);
        var dicItem = new Dictionary<string, object>();
        if (aimPath.Contains(".plist")) { dicItem.Add(STR_FILE_TYPE, STR_PLIST); } else if (aimPath.Contains(".xml")) { dicItem.Add(STR_FILE_TYPE, STR_XML); } else { dicItem.Add(STR_FILE_TYPE, STR_UNKNOW); }
        if (File.Exists(oriPath))
        {
            // 增加修改项
            dicItem.Add(STR_TYPE, STR_CHANGE);
            dicChangeAddItem.Add(aimPath, dicItem);
        }
        else
        {
            // 增加增加项
            dicItem.Add(STR_TYPE, STR_ADD);
            dicAddItem.Add(aimPath, dicItem);
        }

    }

    private void CreateCheckFile()
    {

    }
}

