-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成
local xmlSimple = requirePack("scripts.FrameWork.FileParse.xmlSimple");

local ArmtureGroupUtil = { }

ArmtureGroupUtil.Init = function(node)

    ------------------------私有方法----------------------
    node.parseXmlScriptListToLuaScriptList = function(childList)
        local luaScriptList = { };
        for i = 1, #childList, 1 do
            local xmlNode = childList[i];
            local actType = xmlNode.name();
            if actType ~= "forbidbgnpcclick" or actType ~= "unforbidbgnpcclick" then
                local luaCmd = { };
                luaCmd.ActType = actType;
                if luaCmd.ActType == "say" then
                    luaCmd.ArmName = xmlNode["@npc"];
                    luaCmd.Audio = xmlNode["@audio"];
                elseif luaCmd.ActType == "act" then
                    luaCmd.ArmName = xmlNode["@npc"];
                    luaCmd.ActAnimIndex = xmlNode["@index"];
                    luaCmd.FreeAnimIndex = xmlNode["@xianzhi"];
                elseif luaCmd.ActType == "concurrent" then
                    luaCmd.ActList = node.parseXmlScriptListToLuaScriptList(xmlNode.children());
                elseif luaCmd.ActType == "waitclick" then
                    luaCmd.ArmName = xmlNode["@npc"];
                    luaCmd.TipName = xmlNode["@tip"];
                elseif luaCmd.ActType == "waitmic" then

                elseif luaCmd.ActType == "move" then
                    luaCmd.ArmName = xmlNode["@npc"];
                    luaCmd.ActAnimIndex = xmlNode["@index"];
                    luaCmd.AimPos = cc.p(xmlNode["@x"], xmlNode["@y"]);
                end
                table.insert(luaScriptList, luaCmd);
            end
        end
        return luaScriptList;
    end

    node.processStep = function(luaCmd, cb)-- imple
        if luaCmd.ActType == "say" then
            g_tConfigTable.Director.CurrentScene:say(luaCmd.Audio,node[luaCmd.ArmName],function()
                if cb ~= nil then 
                    cb();
                end
            end);
        elseif luaCmd.ActType == "act" then
            local arm = node[luaCmd.ArmName];
            if arm ~= nil then 
                arm:playByIndex(luaCmd.ActAnimIndex,LOOP_NO);
                arm:setLuaCallBack( function(eType, _tempArm, sEvent)
                    if eType == TouchArmLuaStatus_AnimEnd then
                        arm:playByIndex(luaCmd.FreeAnimIndex,LOOP_YES);
                        if cb~= nil then 
                            cb();
                        end
                    end
                end);
            end
        elseif luaCmd.ActType == "concurrent" then
        elseif luaCmd.ActType == "waitclick" then
        elseif luaCmd.ActType == "waitmic" then
        elseif luaCmd.ActType == "move" then
        end
    end

    node.processList = function(cbListFinish) 
        node.cbListFinish_ = cbListFinish;
        node.process();
    end

    node.processListEnd = function() 
        node.ActList_ = {};
        node.XmlScriptName_ = "";
        node.StepIndex_ = 1;
        if node.cbListFinish_ ~= nil then 
            node.cbListFinish_();
        end
    end

    node.process = function()
        if node.StepIndex_ > #node.ActList_ then 
            -- todo end
            node.processListEnd();
        else 
            local luaCmd = node.ActList_[node.StepIndex_];
            node.processStep(luaCmd,function() node.processEnd() end);
            node.StepIndex_ = node.StepIndex_ + 1;
        end
    end
    
    node.processEnd = function() 
        node.process();
    end
    ------------------------对外接口----------------------
    --[[
        节点执行一个脚本文件
    ]]
    --
    node.RunActionByXmlScript = function(xmlFilePath, xmlScriptName)
        local xmlParse = xmlSimple:loadFile(xmlFilePath);
        local childList = rootXML.root.children();
        node.ActList_ = node.parseXmlScriptListToLuaScriptList(childList);
        node.XmlScriptName_ = xmlScriptName;
        node.StepIndex_ = 1;
        -- todo : act start
        node.processList();
    end

    --[[
        停止目前正在运行的脚本
    ]]
    --
    node.StopAction = function()

    end

    --[[
        获取目前正在执行的脚本名，如果没有执行，返回 "" 空字符串
    ]]
    --
    node.RunningXMLScriptName = function()
        return node.XmlScriptName_;
    end

    --[[
        获取目前正在执行的step
    ]]
    --
    node.ProcessStep = function()

    end

    --[[
        跳过目前step 行
    ]]
    --
    node.Skip = function()

    end
end

return ArmtureGroupUtil;
-- endregion
