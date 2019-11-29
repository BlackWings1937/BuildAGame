using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileChange {
    public string FileName;
    public string FileAbsolutePath;
    public EnumChangeType ChangeType = EnumChangeType.E_NONE;
    public enum EnumChangeType {
        E_NONE,
        E_CHANGE,
        E_DELETE,
        E_ADD   
    }
}

public class GitFilesManager : MonoBehaviour
{

    // ----- 对外接口 -----
    public void StartSurveillanceFloder() {

    }

    public void StopSurveillanceFloder() {

    }

    public void UpdateFloderStatusAsNew() {

    }

    public List<FileChange> GetChangeFilesList() {
        return null;
    }

    public void DeleteGitAtFloder() {

    }


    // ----- 私有方法 -----

    private bool checkFloderExist(string floderPath) {
        return Directory.Exists(floderPath);
    }

    private bool checkGitExist() {

    }

    private bool checkGitIsClean() { }

}
