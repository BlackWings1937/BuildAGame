using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GitUtil
{
    public const string STR_NAME_GIT_PROJ_NAME = "LinkerData";

    public const string STR_GIT_LOCAL = "";

    public static bool CreateGitAtFloder(string path) {
        path = path + STR_NAME_GIT_PROJ_NAME + "/";

        Win32Util.RunCmd("cd "+path);
        Win32Util.RunCmd("git clone "+STR_GIT_LOCAL);
        return false;
    }

    public static bool CheckGitExistAtFloder(string path) {
        path = path + STR_NAME_GIT_PROJ_NAME+"/";
        if (!Directory.Exists(path)) {
            return false;
        }

        Win32Util.RunCmd("cd " + path);
        string result = Win32Util.RunCmd("git status");
        if (result.Contains("fatal: not a git repository (or any of the parent directories): .git")) {
            return false;
        }

        return true;
    }

    public static bool CheckGitWorkTreeCleanAtFloder(string path) {
        return false;
    }


    public static string GetGitCacheAreaPath(string path) {
        return path + STR_NAME_GIT_PROJ_NAME + "/";
    }
}
