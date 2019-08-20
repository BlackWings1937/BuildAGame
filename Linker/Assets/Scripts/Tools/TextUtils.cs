/*
 * 用来做所有和text 相关的通用操作
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TextUtils
{
    /// <summary>
    /// 计算文字再这个text中一行排开的长度
    /// </summary>
    /// <param name="str">需要计算的字符串</param>
    /// <param name="tex">将表示字符串的text</param>
    /// <returns></returns>
    public static int CaculateTextLength(string str, Text tex) {
        int totalLength = 0;
        Font myFont = tex.font;  //chatText is my Text component
        myFont.RequestCharactersInTexture(str, tex.fontSize, tex.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();
        char[] arr = str.ToCharArray();
        foreach (char c in arr)
        {
            myFont.GetCharacterInfo(c, out characterInfo, tex.fontSize);
            totalLength += characterInfo.advance;
        }
        return totalLength;
    }

}
