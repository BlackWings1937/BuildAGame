using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ZipUtils
{
    /// <summary>
    /// 创建压缩包
    /// </summary>
    /// <param name="sourceFilePath">压缩文件夹路径</param>
    /// <param name="destinationZipFilePath">压缩包放哪</param>
    public static void CreateZip(string sourceFilePath, string destinationZipFilePath)
    {
        if (sourceFilePath[sourceFilePath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            sourceFilePath += System.IO.Path.DirectorySeparatorChar;
        ZipOutputStream zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
        zipStream.SetLevel(0);  // 压缩级别 0-9
        CreateZipFiles(sourceFilePath, zipStream, sourceFilePath.Length);
        zipStream.Finish();
        zipStream.Close();
    }

    /// 递归压缩文件
    static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream, int subIndex)
    {
        Crc32 crc = new Crc32();
        string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
        foreach (string file in filesArray)
        {
            if (Directory.Exists(file))                     //如果当前是文件夹，递归
            {
                CreateZipFiles(file, zipStream, subIndex);
            }
            else                                            //如果是文件，开始压缩
            {
                FileStream fileStream = File.OpenRead(file);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                string tempFile = file.Substring(subIndex);
                ZipEntry entry = new ZipEntry(tempFile);
                entry.DateTime = DateTime.Now;
                entry.Size = fileStream.Length;
                fileStream.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                zipStream.PutNextEntry(entry);
                zipStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
