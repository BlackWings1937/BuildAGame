requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");
local KeyFrameParseReader = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.KeyFrameParseReader");

local Actor = class("Actor");

Actor.EnumActorType = {
    ["E_NONE"] = 0,
    ["E_IMAGE"] = 1,
    ["E_ARMATURE"] = 2,
    ["E_SOUND"] = 3,
}

g_tConfigTable.CREATE_NEW(Actor);


--------------私有方法------------------
function Actor:ctor()

    self.myProgramerName_ = ""; -- 程序名称
    self.myArtName_ = "";       -- 动画名称
    self.myActorName_ = "";
    self.showObj_ = nil;        -- 显示对象（可能是sprite 可能是 Armature）
    self.scriptAction_ = nil;   -- 管理actor 的剧情脚本
    self.actorUpdateData_ = {};
    self.resultData = {};
    self:setMyType(Actor.EnumActorType.E_NONE);
end



function Actor:setMyType(t)
    self.myType_ = t;
end

function Actor:checkDataIsOk(data)

    for k,v in pairs(self.actorUpdateData_) do 
        if data[k] == nil then 
            return false;
        end
    end

    return true;
end

function Actor:initActorInfo(actorInfo,frameParse)
    self.actorInfo_ = actorInfo;
    self.frameReader_ = frameParse;
    self.frameReader_:InitByListOfKeyFrames(self.actorInfo_,self.scriptAction_:GetFramesRate());
end

function Actor:findFirstKeyFrameArmatureName(info)
    if info ~= nil then 
        if info.frames ~= nil then 
            local count = #info.frames;
            if count >0 then 
                local firstKeyframeInfo =  info.frames[1];
                if firstKeyframeInfo.einfo ~= nil then 
                    if firstKeyframeInfo.einfo.ename ~= nil then 
                        return firstKeyframeInfo.einfo.ename;
                    end
                end
            end
        end
    end
    return "";
end
--------------对外接口------------------
function Actor:Dispose()
end


--[[
    获取动画引擎
]]--

function Actor:GetAnimationEngine()
   return self.scriptAction_:GetAnimationEngine();
end

--[[
    获取动作对象
]]--
function Actor:GetAction() 
    return self.scriptAction_;
end


--[[
    设置显示对象zOrder
]]--

--[[
    获取角色名
]]--
function Actor:GetActorName()
    return self.actorInfo_.layername;
end

--[[
    获取动画名
]]--
function Actor:GetArtName()
    return self.myArtName_;
end

--[[
    获取角色类型
]]--
function Actor:GetMyType()
    return self.myType_;
end

--[[
    获取显示对象

    返回值
    showObj (sprite or touchArmature) 根据actor 初始化类型返回showobj 对象
]]--
function Actor:GetShowObj()
    return self.showObj_;
end

--[[
    撤销actor 为空置状态
]]--
function Actor:Dispose()
    self.myProgramerName_ = "";
    self.myArtName_ = "";
    self.showObj_ = nil;
    self:setMyType(Actor.EnumActorType.E_NONE);
end


--[[
    更新角色的虚方法
    参数:
    dt: float 更新时间差
]]--
function Actor:Update(dt)

end





return Actor;