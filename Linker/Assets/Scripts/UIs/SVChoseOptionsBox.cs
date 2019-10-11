using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SVChoseOptionsBox : ChoseOptionsBox
{

    /// <summary>
    /// 选择Scene类型选项面板
    /// </summary>
    /// <param name="data">sceneNode data</param>
    /// <param name="rt">面板停靠的显示对象</param>
    /// <param name="sv">sceneView</param>
    public void ShowChoseOptionSceneType(SceneNodeData data,RectTransform rt,SceneView sv) {
        var dic = new Dictionary<string, TapButtonCallBack>();
        dic.Add("剧情类型", () => {
            if (data != null)
            {
                data.MySceneInfoData.MyState = SceneInfoData.State.E_PLot;
                sv.UpdateData();
            }
        });
        dic.Add("玩法类型", () => {
            if (data != null)
            {
                data.MySceneInfoData.MyState = SceneInfoData.State.E_Playment;
                sv.UpdateData();
            }
        });
        this.ShowChoseOptionBoxByNameAnidDicAndNearObj(
            "选择场景类型",
            dic,
            rt
            );
    }

    /// <summary>
    /// 选择初始化InitBgConfig界面
    /// </summary>
    /// <param name="data"></param>
    /// <param name="rt"></param>
    /// <param name="sv"></param>
    public void ShowInitBgConfgChoseBoxByEditBtn(
        SceneNodeData data, 
        RectTransform rt,
        SceneView sv
        ) {
        if (rt != null)
        {
            var dic = new Dictionary<string, TapButtonCallBack>();
            var listOfData = sv.GetController<SceneController>().GetBgConfigsList();
            var count = listOfData.Count;
            for (int i = 0; i < count; ++i)
            {
                var str = listOfData[i];
                dic.Add(str, () => {
                    if (data != null)
                    {
                        data.MySceneInfoData.PLotData.BgConfigName = str;
                        sv.GetController<SceneController>().ParseBgConfigToData(data.MySceneInfoData.PLotData.BgConfigName, data);
                        sv.GetController<SceneController>().UpdateData();
                    }
                });
            }
            this.ShowChoseOptionBoxByNameAnidDicAndNearObj("选择用来初始化的BgConfig", dic, rt);
        }
    }


    /// <summary>
    /// 用户点击NpcItem
    /// </summary>
    /// <param name="data"></param>
    /// <param name="rt"></param>
    /// <param name="sv"></param>
    /// <param name="npcName"></param>
    public void ShowOptionsByUserClickNpcItem(
        SceneNodeData data,
        RectTransform rt,
        SceneView sv,
        NpcOptions ndatas
        ) {
        if (rt != null)
        {
            var dic = new Dictionary<string, TapButtonCallBack>();
            dic.Add("添加子节点", () => {
                sv.OnAddChildNpcOptionNodeByNpcOptions(ndatas,rt);
            });
            dic.Add("移除所有子节点", () => {
                sv.OnRemoveAllChildByNpcOptions(ndatas);
            });
            //dic.Add("粘贴",()=> { });
            this.ShowChoseOptionBoxByNameAnidDicAndNearObj(
                "选择",
                dic,
                rt);
        }

    }

    /// <summary>
    /// 用户点击npcOption
    /// </summary>
    /// <param name="data"></param>
    /// <param name="rt"></param>
    /// <param name="sv"></param>
    /// <param name="ndata"></param>
    public void ShowOptionsByUserClickOptionItem(
                SceneNodeData data,
        RectTransform rt,
        SceneView sv,
        NpcOption ndata
        ) {
        if (rt != null && sv!= null)
        {
            var dic = new Dictionary<string, TapButtonCallBack>();
            dic.Add("添加子节点", () => {
                var ndatas = sv.GetOptionsByNpcName(ndata.Npc);
                if (ndatas!= null) {
                    sv.OnAddChildNpcOptionNodeByNpcOptions(ndatas, rt);
                }
            });
            dic.Add("复制", () => { });
            dic.Add("删除", () => {

            });
            dic.Add("移除所有子节点", () => {
                var ndatas = sv.GetOptionsByNpcName(ndata.Npc);
                sv.OnRemoveAllChildByNpcOptions(ndatas);
            });
            //dic.Add("粘贴",()=> { });
            this.ShowChoseOptionBoxByNameAnidDicAndNearObj(
                "选择",
                dic,
                rt);
        }
    }

    /// <summary>
    /// 显示所有可以选择的lua脚本
    /// </summary>
    /// <param name="data"></param>
    /// <param name="rt"></param>
    /// <param name="sv"></param>
    public void ShowLuaScriptChoseBoxByEditBtn(
        SceneNodeData data,
        RectTransform rt,
        SceneView sv
        )
    {
        if (rt != null)
        {
            var dic = new Dictionary<string, TapButtonCallBack>();
            var listOfData = sv.GetController<SceneController>().GetLuaScriptsResList();
            var count = listOfData.Count;
            for (int i = 0; i < count; ++i)
            {
                var str = listOfData[i];
                dic.Add(str, () => {
                    if (data != null)
                    {
                        data.MySceneInfoData.PlayMentData.LuaScriptName = str;
                        sv.GetController<SceneController>().UpdateData();
                    }
                });
            }
            this.ShowChoseOptionBoxByNameAnidDicAndNearObj("选择luaScript", dic, rt);
        }
    }

    /// <summary>
    /// 显示所有可以选择的动画配置
    /// </summary>
    /// <param name="data"></param>
    /// <param name="rt"></param>
    /// <param name="sv"></param>
    public void ShowAnimationConfigChoseBoxByEditBtn(
        SceneNodeData data,
        RectTransform rt,
        SceneView sv
        )
    {
        if (rt != null)
        {
            var dic = new Dictionary<string, TapButtonCallBack>();
            var listOfData = sv.GetController<SceneController>().GetAnimationConfigsList();
            var count = listOfData.Count;
            for (int i = 0; i < count; ++i)
            {
                var str = listOfData[i];
                dic.Add(str, () => {
                    if (data != null)
                    {
                        data.MySceneInfoData.PlayMentData.AnimationConfigName = str;
                        sv.GetController<SceneController>().UpdateData();
                    }
                });
            }
            this.ShowChoseOptionBoxByNameAnidDicAndNearObj("选择AnimationConfig", dic, rt);
        }
    }

    /// <summary>
    /// 显示所有可以选择的产品配置
    /// </summary>
    /// <param name="data"></param>
    /// <param name="rt"></param>
    /// <param name="sv"></param>
    public void ShowProductConfigBoxByEditBtn(
        SceneNodeData data,
        RectTransform rt,
        SceneView sv
        )
    {
        if (rt != null)
        {
            var dic = new Dictionary<string, TapButtonCallBack>();
            var listOfData = sv.GetController<SceneController>().GetProductConfigsList();
            var count = listOfData.Count;
            for (int i = 0; i < count; ++i)
            {
                var str = listOfData[i];
                dic.Add(str, () => {
                    if (data != null)
                    {
                        data.MySceneInfoData.PlayMentData.ProductConfigName = str;
                        sv.GetController<SceneController>().UpdateData();
                    }
                });
            }
            this.ShowChoseOptionBoxByNameAnidDicAndNearObj("选择ProductConfig", dic, rt);
        }
    }

    /// <summary>
    /// 添加npc 操作节点类型
    /// </summary>
    /// <param name="data"></param>
    /// <param name="rt"></param>
    /// <param name="sv"></param>
    /// <param name="ndatas"></param>
    public void ShowOptionsAddChildNpcOptionNode(
        SceneNodeData data,
        RectTransform rt,
        SceneView sv,
        NpcOptions ndatas
        ) {
        if (rt != null) {
            var dic = new Dictionary<string, TapButtonCallBack>();
            dic.Add("添加播放json",()=> {
                sv.OnAddChildNpcOptionNodeJsonByNpcOptions(ndatas,rt);
            });
            dic.Add("添加触发监听",()=> { });
            dic.Add("添加出口",()=> { });
            this.ShowChoseOptionBoxByNameAnidDicAndNearObj(
                "选择要添加的操作类型", dic, rt);

        }
    }


    /// <summary>
    /// 添加npc 操作节点类型
    /// </summary>
    /// <param name="data"></param>
    /// <param name="rt"></param>
    /// <param name="sv"></param>
    /// <param name="ndatas"></param>
    public void ShowOptionsAddChildJsonNpcOptionNode(
        SceneNodeData data,
        RectTransform rt,
        SceneView sv,
        NpcOptions ndatas
        )
    {
        if (rt != null)
        {
            var dic = new Dictionary<string, TapButtonCallBack>();
            var listOfActionJsons = sv.GetActionsList(data.MySceneInfoData.PLotData.BgConfigName);
            var count = listOfActionJsons.Count;
            for (int i = 0;i<count;++i) {
                var strJsonName = listOfActionJsons[i];
                dic.Add(strJsonName,()=> {
                    sv.AddJsonNpcOptionOnNpcOption(ndatas,strJsonName);
                });
            }
            this.ShowChoseOptionBoxByNameAnidDicAndNearObj(
                "选择要添加的json", dic, rt);

        }
    }

}
