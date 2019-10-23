requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");
local KeyFrameParseReader = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.KeyFrameParseReader");
local Actor = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.Actor");
local ActorShowObj = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ActorShowObj");

local ActorImage = class("ActorImage",function()
    return ActorShowObj.new();
end);
g_tConfigTable.CREATE_NEW(ActorImage);



function ActorImage:initActorInfo(ai)
    ActorShowObj.initActorInfo(self,ai,KeyFrameParseReader.new());

    if self.playMode_ == self:GetAnimationEngine():GetAnimationPlayModeEnum().ALL then 
        self:initActorAllMode();
    elseif  self.playMode_ == self:GetAnimationEngine():GetAnimationPlayModeEnum().ZORDER_ONLY then
        self:initActorZorderMode();
    else 
        self:initActorAllMode();
    end

    self.picHorizontalDiss_ = 0;
    self.picVerticalDiss_ = 0;
    self.showingPicName_ = "";
end


function ActorImage:initActorAllMode() 
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
    }
end

function ActorImage:initActorZorderMode() 
    self.actorUpdateData_ = {
        ["z"] = 0,
        ["ename"] ="",
    }
end

function ActorImage:Update(dt)

    if self.resultData == nil then 
        self.resultData = {};
    end
    self.resultData = self.frameReader_:GetDataByTime(dt,self.actorUpdateData_,self.resultData );

    if self.resultData~= nil then 
        if  self:checkDataIsOk(self.resultData) then 
            if self.showObj_ ~= nil then 
                if self.playMode_ ~= self:GetAnimationEngine():GetAnimationPlayModeEnum().ZORDER_ONLY then 
                    self.showObj_:setPosition(cc.p( self.resultData.x , self:GetAction():GetTheaterContentSize().height-self.resultData.y));
                    self.showObj_:setScaleX(self.resultData.sx);
                    self.showObj_:setScaleY(self.resultData.sy);
                    self.showObj_:setRotationSkewX(self.resultData.skewX);
                    self.showObj_:setRotationSkewY(self.resultData.skewY);
                    self.showObj_:setRotation(self.resultData.rot);
                end

                self:SetShowObjZOrder(self.resultData.z + self:GetAction():GetZOrderDiss());
            else
                if self.resultData.ename ~= "" and self.resultData.ename  ~= nil then 
                    if self.theaterLayer_ ~= nil then 
                        self.showObj_ = cc.Sprite:create(self:GetAction():GetPicResPath()..self.resultData.ename);
                        self.showObj_:setName(self:GetAnimationEngine():TransformEngineNameToActualName( self:GetAnimationEngine():GetAnimationEngineCreatePre().."*".. self.actorInfo_.layername));
                        self:reInitBySaveStatue();
                        self.theaterLayer_:addChild(self.showObj_);
                    else
                        self.showObj_ = nil ;
                    end
                end
            end
            self:updatePicName(self.resultData);
        end
    else
        if self.showObj_ ~= nil then 
            self:saveShowObjNowStatue();
            self.showObj_:removeFromParent();
            self.showObj_ = nil;
        end
        self.playingArmatureName_ = "";
    end


    
end

function ActorImage:updatePicName(data)
    if self.showObj_ == nil then 
        return ;
    end
    if data.ename ~= self.playingArmatureName_ then 
        self.playingArmatureName_ = data.ename;
        local pCache = cc.Director:getInstance():getTextureCache();
        local tex = pCache:addImage(self:GetAction():GetPicResPath()..data.ename);
        self.showObj_:setTexture(tex);
        local size = self.showObj_:getTexture():getContentSize();
        if math.abs(data.rot- 90) <5 then 
            self.picHorizontalDiss_ = data.sy* size.height/2;
            self.picVerticalDiss_ =    data.sx* size.width/2;
        else
            self.picHorizontalDiss_ = data.sx* size.width/2;
            self.picVerticalDiss_ =   data.sy* size.height/2;
        end
        self.showObj_:setTextureRect(cc.rect(0, 0, size.width, size.height ));
    end
end

------------------对外接口---------------
--[[
    初始化Actor 并创建actor 的显示对象

    参数:
    info: table 初始化Actor 的信息
    theaterLayer: ccnode Actor 显示对象的母节点
]]--
function ActorImage:InitByActorInfo(info,theaterLayer,playMode)
    -- 初始化播放模式
    self.playMode_ = playMode or self:GetAnimationEngine():GetAnimationPlayModeEnum().ALL;
    
    -- 保存角色信息
    self:initActorInfo(info);

    -- 创建角色显示对象
    local firstKeyFrameArmatureName = self:findFirstKeyFrameArmatureName(self.actorInfo_);
    if firstKeyFrameArmatureName ~= "" then 
        -- todo check armature name exist in res 
        self.showObj_ =  cc.Sprite:create(self:GetAction():GetPicResPath()..firstKeyFrameArmatureName);
        self.showObj_:setName(self:GetAnimationEngine():TransformEngineNameToActualName(self:GetAnimationEngine():GetAnimationEngineCreatePre().."*"..self.actorInfo_.layername) );
        self.showObj_:setAnchorPoint(cc.p(0,1));
        self:Update(0.01);
        theaterLayer:addChild(self.showObj_);--,info.z
    else 
        -- todo error firstKeyFrameArmatureName == ""
    end

end

--[[
    初始化Actor

    参数:
    info: table 初始化actor 的信息
    showObj: ccsprite Actor的显示对象
]]--
function ActorImage:InitByActorInfoAndShowObj(info,showObj,playMode)
    -- 初始化播放模式
    self.playMode_ = playMode or self:GetAnimationEngine():GetAnimationPlayModeEnum().ALL;

    self:initActorInfo(info);
    self.showObj_  = showObj;
    self.theaterLayer_  = self.showObj_:getParent();
end


return ActorImage;