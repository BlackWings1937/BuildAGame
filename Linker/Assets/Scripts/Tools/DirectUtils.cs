using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DirectUtils
{

    /// <summary>
    /// 复制一个目录到另一目录
    /// </summary>
    /// <param name="srcPath"></param>
    /// <param name="aimPath"></param>
    public static void CopyDir(string srcPath, string aimPath) {
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
                    DirectUtils.CopyDir(element, srcdir);
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
}
