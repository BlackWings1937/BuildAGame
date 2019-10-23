
requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");
local KeyFrameParseReader = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.KeyFrameParseReader");
local KeyFrameParseReaderSound = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.KeyFrameParseReaderSound");
local Actor = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.Actor");
local ActorShowObj = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ActorShowObj");
local CustomDefineNameInfo = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.CustomActualNameConfig");
local ActorArmature = class("ActorArmature",function()
    return ActorShowObj.new();
end);
g_tConfigTable.CREATE_NEW(ActorArmature);

--[[
function ActorArmature:ctor()
    Actor.ctor();
end
]]--


function ActorArmature:initActorInfo(ai)

    -- 初始化角色动作信息
    ActorShowObj.initActorInfo(self,ai,KeyFrameParseReader.new());

    if self.playMode_ == self:GetAnimationEngine():GetAnimationPlayModeEnum().ALL then 
        self:initActorAllMode();
    elseif  self.playMode_ == self:GetAnimationEngine():GetAnimationPlayModeEnum().ZORDER_ONLY then
        self:initActorZorderMode();
    else 
        self:initActorAllMode();

    end


    self.playingArmatureName_ = "";
    self.playingArmatureIndex_ = "";

    -- 初始化角色声音信息
    local aai = self:GetAction():GetAudioInfoByNpcName(ai.layername);-- 从scriptAction 获取
    if aai~= nil then 
        self.audioFrameReader_ = KeyFrameParseReaderSound.new();
        self.audioFrameReader_:InitByListOfKeyFrames(aai,self:GetAction():GetFramesRate());
        self.audioUpdateData_ = {
            ["frameid"] = -1,
            ["name"] = "null",
        };
        self.nowFrameId_ = -1;
    else
        self.audioFrameReader_ = nil;
    end
end

function ActorArmature:initActorAllMode() 
    self.actorUpdateData_ = {
        ["x"] = 0,
        ["y"] = 0,
        ["sx"] = 0,
        ["sy"] = 0,
        ["rot"] = 0,
        ["skewX"] = 0,
        ["skewY"] = 0,
        ["z"] = 0,
        ["ename"] ="",
        ["eitem"] = "",
    }
end

function ActorArmature:initActorZorderMode()
    self.actorUpdateData_ = {
        ["z"] = 0,
        ["ename"] ="",
        ["eitem"] = "",
    }
end

function ActorArmature:updateArmatureName(armName,index,dt)
    if self.showObj_== nil then -- 防止showObj为空的情况
        return ;
    end
    if self.resultData.ename == "" or self.resultData.ename == nil then 
        return ;
    end
    if armName ~= self.playingArmatureName_ or index ~= self.playingArmatureIndex_ then 
        self.playingArmatureName_ = armName;
        self.playingArmatureIndex_ = index;
        self.showObj_:changeArmature(self.playingArmatureName_, self.playingArmatureIndex_);
        self.showObj_:playByMovName(self.playingArmatureIndex_ , LOOP_ORIG);
        self:updateSpeak(dt,true);
    end
end


function ActorArmature:Update(dt)
    -- 更新角色动作
    if self.resultData == nil then 
        self.resultData = {};
    end
    self.resultData = self.frameReader_:GetDataByTime(dt,self.actorUpdateData_ ,self.resultData);
    if self.resultData ~= nil  then 
        if self:checkDataIsOk(self.resultData) then 
            if self.showObj_ ~= nil then 
                if self.playMode_ ~= self:GetAnimationEngine():GetAnimationPlayModeEnum().ZORDER_ONLY then 
                    self.showObj_:setPosition(cc.p(self.resultData.x,self:GetAction():GetTheaterContentSize().height- self.resultData.y));
                    self.showObj_:setScaleX(self.resultData.sx);
                    self.showObj_:setScaleY(self.resultData.sy);
                    self.showObj_:setRotationSkewX(self.resultData.skewX);
                    self.showObj_:setRotationSkewY(self.resultData.skewY);
                    self.showObj_:setRotation(self.resultData.rot);
                end
                self:SetShowObjZOrder(self.resultData.z + self:GetAction():GetZOrderDiss());
            else
                if self.resultData.ename ~= "" and self.resultData.ename ~= nil then 
                    if self.theaterLayer_ ~= nil then 
                        self.showObj_ =  TouchArmature:create(self.resultData.ename, TOUCHARMATURE_NORMAL, "");
                        self.showObj_:setName( self:GetAnimationEngine():TransformEngineNameToActualName(self:GetAnimationEngine():GetAnimationEngineCreatePre().."*".. self.actorInfo_.layername) );
                        self:reInitBySaveStatue();
                        self.theaterLayer_:addChild(self.showObj_);
                    else 
                        self.showObj_ = nil;
                    end
                end
            end
            self:updateArmatureName(self.resultData.ename,self.resultData.eitem,dt);
        end
    else
        if  self.showObj_ ~= nil then 
            local name = self.showObj_:getName();
            if self:checkNameInFix(name) == false then 
                if self.showObj_ ~= nil then 
                    self:saveShowObjNowStatue();
                    self.showObj_:removeFromParent();
                    self.showObj_ = nil;
                end
                self.playingArmatureName_  = "";
                self.playingArmatureIndex_ = "";
            else
                self.playingArmatureName_ = "debug_blank";
                self.playingArmatureIndex_ = "0";
                self.showObj_:changeArmatureByIndx(self.playingArmatureName_, 0);
                self.showObj_:playByIndex(0,LOOP_YES);
            end
        end
    end
    self:updateSpeak(dt);
end

function ActorArmature:checkNameInFix(name)
    local count = #CustomDefineNameInfo.ListOfFixNpcs;
    for i = 1,count,1 do 
        if name == CustomDefineNameInfo.ListOfFixNpcs[i] then 
            return true;
        end
    end
    return false;
end

function ActorArmature:updateSpeak(dt,isBreak)
    if self.audioFrameReader_  == nil then 
        return ;
    end
     -- 更新角色嘴型
    if self.showObj_ ~= nil then 
        if self.audioResultData == nil then 
            self.audioResultData = {};
        end
        self.audioResultData = self.audioFrameReader_:GetDataByTime(dt,self.audioUpdateData_ ,self.audioResultData);
        if self.audioResultData ~= nil then 
            if self.nowFrameId_ ~= self.audioResultData.frameid or isBreak then 
                if  self.audioResultData.name == "null" then 
                    self.showObj_:changeChildArmaturesToName("sp_mouse", "idle_mouse");
                else 
                    self.showObj_:changeChildArmaturesToName("sp_mouse", "speak_mouse");
                end
                self.nowFrameId_ =  self.audioResultData.frameid; 
            end
        end
    end
end

------------------对外接口---------------
--[[
    初始化Actor 并创建actor 的显示对象

    参数:
    info: table 初始化Actor 的信息
    theaterLayer: ccnode Actor 显示对象的母节点
]]--
function ActorArmature:InitByActorInfo(info,theaterLayer,playMode)
    -- 初始化播放模式
    self.playMode_ = playMode or self:GetAnimationEngine():GetAnimationPlayModeEnum().ALL;
    -- 保存角色信息
    self:initActorInfo(info);
    self.theaterLayer_ = theaterLayer;
    -- 创建角色显示对象
    local firstKeyFrameArmatureName = self:findFirstKeyFrameArmatureName(self.actorInfo_);
    if firstKeyFrameArmatureName ~= "" then 
        -- todo check armature name exist in res 
        self.showObj_ =  TouchArmature:create(firstKeyFrameArmatureName, TOUCHARMATURE_NORMAL, "");
        self.showObj_:setName(self:GetAnimationEngine():TransformEngineNameToActualName(self:GetAnimationEngine():GetAnimationEngineCreatePre().."*"..self.actorInfo_.layername) );
        theaterLayer:addChild(self.showObj_);--,info.z
    else 
        -- todo error firstKeyFrameArmatureName == ""
    end

end

--[[
    初始化Actor

    参数:
    info: table 初始化actor 的信息
    showObj: touchArmature Actor的显示对象
]]--
function ActorArmature:InitByActorInfoAndShowObj(info,showObj,playMode)
    -- 初始化播放模式
    self.playMode_ = playMode or self:GetAnimationEngine():GetAnimationPlayModeEnum().ALL;

    self:initActorInfo(info);
    self.showObj_  = showObj;
    self.theaterLayer_  = self.showObj_:getParent();
end


return ActorArmature;