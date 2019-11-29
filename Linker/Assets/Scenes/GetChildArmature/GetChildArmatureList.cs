using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Diagnostics;

[Serializable]
public class ChildArmatureList {
    public List<string> ChildrenNames = new List<string>();
}


public class GetChildArmatureList : MonoBehaviour
{
    ChildArmatureList n_ = new ChildArmatureList();
    public void ReadXMLPrint(string xmlName)
    {

        string[] arr = File.ReadAllLines(xmlName);
        for (int i = 0; i < arr.Length; ++i)
        {
            // Console.WriteLine(arr[i]);
            if (arr[i].Contains("type=\"armature\""))
            {
                var index = arr[i].IndexOf("display name=");
                string[] str = arr[i].Split('\"');
                //Console.WriteLine(str[1]);
                n_.ChildrenNames.Add(str[1]);
            }
        }
        // Console.WriteLine("complie");

    }


  

    private void Start()
    {

        //GitUtil.STR_GIT_LOCAL = "";
        GitUtil.CreateGitAtFloder("");
       // RunCmd("cd "+System.Environment.CurrentDirectory+"/");
       // RunCmd("git status");


        /*

        DirectoryInfo theFolder = new DirectoryInfo("C://Users//BlackWings//Documents//xblWorkplace//xblt5//xiaobanlong//xiaobanlong5.2.0//Resources//theme//common//animation//hold//");
        //遍历文件
        foreach (FileInfo NextFile in theFolder.GetFiles())
        {
            if (NextFile.Name.Contains(".xml")) {
                ReadXMLPrint("C://Users//BlackWings//Documents//xblWorkplace//xblt5//xiaobanlong//xiaobanlong5.2.0//Resources//theme//common//animation//hold//"+NextFile.Name);
            }
        }
        var str = "C://Users//BlackWings//Documents//xblWorkplace//xblt5//xiaobanlong//xiaobanlong5.2.0//Resources//armAnimations//anixbl//";
        theFolder = new DirectoryInfo(str);
        //遍历文件
        foreach (FileInfo NextFile in theFolder.GetFiles())
        {
            if (NextFile.Name.Contains(".xml"))
            {
                ReadXMLPrint(str + NextFile.Name);
            }
        }
        Debug.Log("complie");


        var result = JsonUtility.ToJson(n_);
        File.WriteAllText(
            "C://Users//BlackWings//Documents//xblWorkplace//xblt5//xiaobanlong//xiaobanlong5.2.0//Resources//theme//common//animation//hold//childrenNames.json"
           , result
           );*/



    }
}
