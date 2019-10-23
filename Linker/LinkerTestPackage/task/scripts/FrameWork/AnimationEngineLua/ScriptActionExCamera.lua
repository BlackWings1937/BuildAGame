local StringUtil = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Util.StringUtil");
local ScriptAction = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ScriptAction");
local Actor = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.Actor");
local ActorImage = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ActorImage");
local ActorSound = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ActorSound");
local ActorEvent = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ActorEvent");
local ActorArmature = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ActorArmature");
local KeyFrameParseReader = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.KeyFrameParseReader");
local FileUtil = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Util.FileUtil");
local ScriptUtil = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ScriptUtil");
local CustomConfig = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.CustomActualNameConfig");

local ScriptActionExCamera = class("ScriptActionExCamera",function() 
    return  ScriptAction.new();
end);
g_tConfigTable.CREATE_NEW(ScriptActionExCamera);

function ScriptActionExCamera:initTableScriptFrameByAbsMode()
    -- 获取默认角色移动速度
    local defaultActorSpeed = self:GetAnimationEngine():GetActorDefaultSpeedValue(self.theaterLayer_);

    local maxDistance = 0;
    local maxDistanceActorIndex = -1;

    local layers = self.tableOfScript_.layers;
    local countOfLayers = #layers;
    local tmpName = "";
    local tmpShowObj = nil;
    local isCamera = false;
    local x,y;
    local posNow;
    local aimPos;
    for i = 1,countOfLayers,1 do 
        if layers[i].framestype == "element" then 
            posNow = nil;
            tmpName = layers[i].layername;
            tmpShowObj = self:searchShowObjArmByActorName(tmpName);
            if tmpShowObj ~= nil then
                x,y = tmpShowObj:getPosition();
                posNow = cc.p(x,y);
                layers[i].tmpShowObj = tmpShowObj;
            else
                if tmpName == "camera" then 
                    posNow = self:GetCameraPos();
                end
            end
            if posNow ~= nil then 
                aimPos = cc.p(layers[i].frames[1].einfo.x, self.theaterContentSize_.height - layers[i].frames[1].einfo.y);
                local dis =  cc.pGetDistance(aimPos,posNow);
                if maxDistance < dis then 
                    maxDistance = dis;
                    maxDistanceActorIndex = i;
                end
                layers[i].beforePos = posNow;
                layers[i].distance = dis;
                if posNow.x<aimPos.x then 
                    layers[i].ori = -1;
                else
                    layers[i].ori = 1;
                end
            end
        end
    end

    if 0<maxDistance and maxDistanceActorIndex ~= -1 then 
        local maxDurtion = maxDistance/defaultActorSpeed;
        layers = self.tableOfScript_.layers;
        countOfLayers = #layers;
        tmpName = "";
        tmpShowObj = nil;
        isCamera = false;
        for i = 1,countOfLayers,1 do 
            if layers[i].framestype == "element" then 
                tmpName = layers[i].layername;
                tmpShowObj = layers[i].tmpShowObj ;
                isCamera = (tmpName == "camera");
                if tmpShowObj ~= nil or isCamera then
                    -- has show obj 
                    self:insertWaitKeyFrameShowObjOn(layers[i],maxDurtion,isCamera,defaultActorSpeed)
                else
                    -- none show obj
                    self:insertWaitKeyFrameShowObjOff(layers[i],maxDurtion);
                end
            else 
                self:insertWaitKeyFrameShowObjOff(layers[i],maxDurtion);
            end
        end
        self.tableOfScript_.common.totalDuration = self.tableOfScript_.common.totalDuration + maxDurtion;
    end
end

function ScriptActionExCamera:insertWaitKeyFrameShowObjOn(layer,maxDurtion,isCamera,defaultActorSpeed)
    local startPos = layer.beforePos;
    local dis = layer.distance;
    local stopMoveDt = 0.01;
    local keyFrameMoving = ScriptUtil.deepcopy(layer.frames[1]);
    keyFrameMoving.einfo.x = startPos.x;
    keyFrameMoving.einfo.y = self.theaterContentSize_.height - startPos.y;

    local moveTime = dis/defaultActorSpeed ;
    keyFrameMoving.dt = (moveTime- stopMoveDt)*self.tableOfScript_.common.frameRate;
    keyFrameMoving.mending = true;
    if isCamera == false then 
        keyFrameMoving.einfo.sx = layer.ori*math.abs(keyFrameMoving.einfo.sx);
        if CustomConfig.ToActorPreWorkAction[layer.layername] ~= nil then 
            keyFrameMoving.einfo.ename = CustomConfig.ToActorPreWorkAction[layer.layername].runAnimation;
            keyFrameMoving.einfo.eitem = CustomConfig.ToActorPreWorkAction[layer.layername].runIndex;
        end

    end

    local keyFrameMoveStop = ScriptUtil.deepcopy(layer.frames[1]);
    keyFrameMoveStop.dt = stopMoveDt * self.tableOfScript_.common.frameRate;
    keyFrameMoveStop.einfo.sx = keyFrameMoving.einfo.sx;
    keyFrameMoveStop.mending = false;

    local keyFrameWait = ScriptUtil.deepcopy(layer.frames[1]);
    keyFrameWait.dt = (maxDurtion - moveTime)* self.tableOfScript_.common.frameRate;
    keyFrameWait.mending = false;

    table.insert(layer.frames,1,keyFrameWait);
    table.insert(layer.frames,1,keyFrameMoveStop);
    table.insert(layer.frames,1,keyFrameMoving);
end

function ScriptActionExCamera:insertWaitKeyFrameShowObjOff(layer,maxDurtion)
    local addFrame = ScriptUtil.deepcopy(layer.frames[1]);
    addFrame.dt = maxDurtion*self.tableOfScript_.common.frameRate;
    table.insert(layer.frames,1,addFrame);
end



function ScriptActionExCamera:SetCameraActive(v)
    self.cameraActive_ = v;
end

function ScriptActionExCamera:GetCameraActive()
    return self.cameraActive_;
end


function ScriptActionExCamera:initTableScriptPosByRefMode() 
    local npcName = self:GetRefNpcName();

    -- 查找相对对象坐标
    local layers= self.tableOfScript_.layers;
    local count = #layers;
    local layer = nil;
    local matchPos = nil;
    local matchNpcShowObj = nil;

    if npcName ~= "" then 
        for i = 1,count,1 do 
            layer = layers[i];
            if layer.layername ~= "camera" then 
                if layer.framestype == "element" then 
                    local name =  StringUtil.GetN_Behind(layer.layername ,2);
                    if npcName == name then
                        matchPos = cc.p(layer.frames[1].einfo.x,layer.frames[1].einfo.y);
                        matchNpcShowObj = self:searchShowObjArmByActorName(layer.layername);
                        break; 
                    end
                end
            end
        end
    else
        print("error can not find ref obj aim Actor Name:"..npcName );
    end

    if matchNpcShowObj~= nil then 
       
        local x,y = 0,0;
        if  matchNpcShowObj ~= nil then 
            x,y = matchNpcShowObj:getPosition();
            y = self.theaterContentSize_.height - y;
        end
        local sizeOfCamera = self:GetAnimationEngine():GetCameraSize(self.theaterLayer_);
        local rectCharecter = cc.rect(0,0,self.theaterContentSize_.width,self.theaterContentSize_.height);
        local rectCamera = cc.rect(
            sizeOfCamera.width/2,
            sizeOfCamera.height/2,
            self.theaterContentSize_.width-sizeOfCamera.width
            ,self.theaterContentSize_.height-sizeOfCamera.height);
        

        for i = 1,count,1 do 
            layer = layers[i];
            if layer.layername ~= "camera" then --
                if layer.framestype == "element" or layer.framestype == "pic" then
                    local frameList = layer.frames;
                    local countFrame = #layer.frames;
                    for z = 1,countFrame,1 do 
                        if  frameList[z].einfo ~= nil then 
                            frameList[z].einfo.x = frameList[z].einfo.x - matchPos.x + x; 
                            frameList[z].einfo.y = frameList[z].einfo.y - matchPos.y + y; 
                            self:normalizePos(frameList[z].einfo,rectCharecter);
                        end
                    end
                end
            else
                local frameList = layer.frames;
                local countFrame = #layer.frames;
                for z = 1,countFrame,1 do 
                    if frameList[z].einfo~= nil then 
                        frameList[z].einfo.x = frameList[z].einfo.x - matchPos.x + x; 
                        frameList[z].einfo.y = frameList[z].einfo.y - matchPos.y + y; 
                        self:normalizePos(frameList[z].einfo,rectCamera);
                    end
                end
            end
        end
    end
end

function ScriptActionExCamera:normalizePos(pos,rect)
    pos.x = math.min(math.max(pos.x,rect.x),rect.x+ rect.width);
    pos.y = math.min(math.max(pos.y,rect.y),rect.y+ rect.height);
end

function ScriptActionExCamera:initByNode(node)

    -- 初始化表演层节点 大小

    self.theaterLayer_ = node;
    --temp
    if self:GetIsUseTheaterContentSize() then 
        self.tableOfScript_.sceneInfo = nil ;
    else
    end
    local isInitCamera = false;
    local layers = self.tableOfScript_.layers;

    local sizeNow = self.theaterLayer_:getContentSize();
    if self.tableOfScript_.sceneInfo == nil then 
        self.tableOfScript_.sceneInfo = sizeNow;
    end

    if math.abs(sizeNow.width - self.tableOfScript_.sceneInfo.width) < 5 or math.abs(sizeNow.height - self.tableOfScript_.sceneInfo.height) < 5 then 
        self.theaterContentSize_ = sizeNow;
    else 
        self.theaterContentSize_ = cc.size(
            self.tableOfScript_.sceneInfo.width,
            self.tableOfScript_.sceneInfo.height);
        self.theaterLayer_:setContentSize(self.theaterContentSize_);
    end

    -- 将时间单位从帧，转换成秒
    self.tableOfScript_.common.totalDuration  = self.tableOfScript_.common.totalDuration/self.tableOfScript_.common.frameRate;
    
    -- 初始化相机
    self.cameraInfo_ = nil;
    local count = #layers;
    for i = count,1,-1 do 
        local layer = layers[i];
        if layer.layername == "camera" then 
            self.cameraInfo_  = layer;
            break;
        end
    end
    
    --[[如果播放模式为相对模式，则以此规则初始化所有关键帧坐标点]]--
    if self:GetPlayMode() == self:GetAnimationEngine():GetAnimationPlayModeEnum().REF then 
        self:initTableScriptPosByRefMode();
    elseif self:GetPlayMode() == self:GetAnimationEngine():GetAnimationPlayModeEnum().ABS then 
        self:initTableScriptFrameByAbsMode();
    end

    if self.cameraInfo_ ~= nil then 
        self.kfprCamera_ = KeyFrameParseReader.new();
        self.kfprCamera_:InitByListOfKeyFrames(self.cameraInfo_,self.tableOfScript_.common.frameRate);
        self.cameraData_ = {
            ["x"] = 0,
            ["y"] = 0,
            ["sx"] = 0,
            ["sy"] = 0,
            ["rot"] = 0,
            ["skewX"] = 0,
            ["skewY"] = 0,
        }
    end
    -- 初始化无关角色归位层级 error
    local listOfChildren = self.theaterLayer_:getChildren();
    local count  =#listOfChildren;
    for i = count ,1,-1 do 
        local child = listOfChildren[i];
        local childName = self:GetAnimationEngine():TransformActualNameToEngineName(child:getName());
        if string.find(childName,self:GetAnimationEngine():GetAnimationEngineCreatePre()) ~= nil then 
            local list = StringUtil.Split(childName,'*');
            local actorName = list[2];
            local isInActionControl,origz = self:isInActionControl(actorName) ;
            local zOrder = -1;
            if isInActionControl then 
                zOrder = self:getRuntimeZOrderByNameAndOrigz(actorName,origz);
            else 
                zOrder = self:getRuntimeZOrderByName(actorName);-- 这里变更的时单个的actor
            end
            if  self:GetActorOccupyInfo()[actorName] == false then 
                if self:GetIsBanZOrder() == false then 
                    child:setLocalZOrder(zOrder );
                    self:SetRuntimeActorZorderByName(actorName,zOrder);
                end
            end
        end
    end

    -- 初始化角色运行时zOrder
    count = #layers;
    for i = 1,count,1 do 
        local info = layers[i];
        if info.layername ~= "camera" then 
            if info.framestype == "element" or info.framestype == "pic" then 
                local countOfFrames = #info.frames;
                for z = countOfFrames ,1 ,-1 do 
                    info.frames[z].z = self:getRuntimeZOrderByNameAndOrigz(info.layername,info.frames[z].z);
                end
            end
        end
    end

    -- 初始化相机框
    print("camera create1");
    self.cameraSp_ = self.theaterLayer_:getChildByName("camera");
    print("camera create2");
    if self.cameraSp_ == nil then
        print("camera create3");
        self.cameraSp_ = cc.Sprite:create(g_tConfigTable.sTaskpath.."bgimg/camera.png");
        print("camera create4");
        self.cameraSp_:setName("camera");
        self.theaterLayer_:addChild(self.cameraSp_,1000000);
        print("camera create5");
    end


    -- 初始化角色s
    self.listOfActors_ = {};
    count = #layers;
    local createCount = 0;
    for i = 1,count,1 do 
        local info = layers[i];
        if info.framestype == "event" then 
            local a = ActorEvent.new();
            a.scriptAction_ = self;
            a:InitByActorInfo(info);
            table.insert(self.listOfActors_,a);
        elseif info.framestype == "element" and info.layername ~= "camera" then 
            local a =  ActorArmature.new();
            a.scriptAction_ = self;
            local showObj = self:searchShowObjArmByActorName(info.layername );
            if showObj ~= nil then 
                a:InitByActorInfoAndShowObj(info,showObj,self:GetPlayMode());
            else 
                a:InitByActorInfo(info,self.theaterLayer_,self:GetPlayMode());
            end
            table.insert(self.listOfActors_,a);
            createCount = createCount + 1;
        elseif info.framestype == "pic" then
            local a = ActorImage.new();
            a.scriptAction_ = self;
            local showObj = self:searchShowObjPicByActorName(info.layername );
            if showObj ~= nil then 
                a:InitByActorInfoAndShowObj(info,showObj,self:GetPlayMode());
            else 
                a:InitByActorInfo(info,self.theaterLayer_,self:GetPlayMode());
            end
            table.insert(self.listOfActors_,a);
            createCount = createCount + 1;
        elseif info.framestype == "sound" or info.layername == "bgmusic" then 
            local a = ActorSound.new();
            a.scriptAction_ = self;
            a:InitByActorInfo(info);
            table.insert(self.listOfActors_,a);
        end
    end

end

function ScriptActionExCamera:searchShowObjArmByActorName(actorName)
    local showObj = self.theaterLayer_:getChildByName(self:GetAnimationEngine():TransformEngineNameToActualName(self:GetAnimationEngine():GetAnimationEngineCreatePre().."*".. actorName) );
    if showObj ~= nil then 
        if showObj["playByIndex"] ~= nil then 
            return showObj;
        end
    end
    return nil;
end

function ScriptActionExCamera:searchShowObjPicByActorName(actorName)
    local showObj = self.theaterLayer_:getChildByName(self:GetAnimationEngine():TransformEngineNameToActualName(self:GetAnimationEngine():GetAnimationEngineCreatePre().."*"..actorName));
    if showObj~= nil then 
        if showObj["playByIndex"] == nil then
            return showObj;
        end
    end
    return nil;
end

--mark
function ScriptActionExCamera:setCameraByData(data)--(self.theaterContentSize_.height -  (self.theaterContentSize_.height - 897.75)/self.theaterContentSize_.height
    if  self.cameraSp_ ~= nil then 
        self.cameraSp_:setPosition(data.x,(self.theaterContentSize_.height - data.y));
        self.cameraSp_:setRotation(data.rot);
        self.cameraSp_:setScaleX(data.sx);
        self.cameraSp_:setScaleY(data.sy);
        self.cameraSp_:setRotationSkewX(data.skewX);--
        self.cameraSp_:setRotationSkewY(data.skewY);
    end
    local anchorPoint = cc.p(data.x/self.theaterContentSize_.width, (self.theaterContentSize_.height - data.y)/self.theaterContentSize_.height);--cc.p(data.x/self.theaterContentSize_.width,(data.y)/self.theaterContentSize_.height);
    self.theaterLayer_:setAnchorPoint(anchorPoint);--anchorPoint
    self.theaterLayer_:setRotation(data.rot);--
    self.theaterLayer_:setScaleX(1/data.sx);
    self.theaterLayer_:setScaleY(1/data.sy);
    self.theaterLayer_:setRotationSkewX(data.skewX);--
    self.theaterLayer_:setRotationSkewY(data.skewY);--
end

function ScriptActionExCamera:updateCamera(dt)
    if self.cameraActive_ then 
        if self.cameraInfo_ ~= nil then 
            if self.resultData == nil then 
                self.resultData = {};
            end
            self.cameraData_ = self.kfprCamera_:GetDataByTime(self.time_ , self.cameraData_,self.resultData);
            self:setCameraByData(self.cameraData_);
        end
    end
    
end

function ScriptActionExCamera:updateActors(dt)
    if self.listOfActors_ ~= nil then 
        local count = #self.listOfActors_;
        for i = 1,count,1 do 
            local actor = self.listOfActors_[i];
            actor:Update(dt);
        end
    end
end

function ScriptActionExCamera:Process(dt)
    self:updateActors(self.time_);
    self:updateCamera(dt);
    ScriptAction.Process(self,dt);
end

    

--------------对外接口---------------------

--[[
    获取音频信息根据角色名
    参数：
    npcName: string 角色名称
]]--
function ScriptActionExCamera:GetAudioInfoByNpcName(npcName)
    local list = StringUtil.Split(npcName,'_');
    local actorName = list[2];
    if actorName ~= nil then 
        local count = #self.tableOfScript_.layers;
        for i = 1,count,1 do
            local item = self.tableOfScript_.layers[i] ;
            if item.layername ~= "" then 
                local list2 = StringUtil.Split(item.layername,"_");
                if list2[2] == actorName then 
                    return item;
                end
            end
        end
    end
    return nil;
end

--[[
    获取动画播放的帧率
]]--
function ScriptActionExCamera:GetFramesRate()
    return self.tableOfScript_.common.frameRate;
end

--[[
    获取舞台大小

    返回值:
    size: ccsize 舞台大小
]]--
function ScriptActionExCamera:GetTheaterContentSize()
    return self.theaterContentSize_;
end

--[[
    获取镜头位置
]]--
function ScriptActionExCamera:GetCameraPos()
    local anchorPoint = self.theaterLayer_:getAnchorPoint();
    return cc.p(anchorPoint.x*self.theaterContentSize_.width,self.theaterContentSize_.height* anchorPoint.y);
end


--[[
    初始化脚本动作对象

    参数:
    scriptPath: string 剧本路径（绝对路径）
    node: ccnode 舞台节点
    cb:void:返回空(eventName:返回事件名,data:返回数据) 台本播放中的各类事件（创建npc 销毁npc 剧本播放完毕等事件）

    返回值
    tag: int 剧本动作对应的唯一tag ，可以使用这个tag 停止动作
]]--
function ScriptActionExCamera:Init(scriptPath,node,zorderModule,cb)
    self.resultData = {};
    self.cameraActive_ = true;
    if  ScriptAction.initScriptFile(self,scriptPath,zorderModule,cb) == false then 
        return false;
    end

    self:initByNode(node);

    local info = self:GetAnimationEngine():getNodeZOrderCacheByNodeName(node:getName());
    info.countOfRunningActions = info.countOfRunningActions + 1;

    return  true;
end

function ScriptActionExCamera:Dispose()
    ScriptAction.Dispose(self);
    local info = self:GetAnimationEngine():getNodeZOrderCacheByNodeName(self.theaterLayer_:getName());
    info.countOfRunningActions = info.countOfRunningActions - 1;
end

return ScriptActionExCamera;

        --[[
        if info.framestype ~= "" and info.framestype ~= "sound" and info.layername ~= "camera" then 
            local actor = self:searchActorByActorName(info.layername);
            if actor == nil then 
                actor = Actor.new();
                self.theaterLayer_:addChild(actor)
            end
            actor:InitByActorInfo(info);
        end
        ]]--


--[[
function ScriptActionExCamera:searchActorInfoByActorName(actorName)
    local layers =  self.tableOfScript_.layers;
    local count = #layers;
    for i = 1,count,1 do 
        local info = layers[i];
        if info.layername == actorName then
            return info;
        end
    end
    return nil;
end

function ScriptActionExCamera:updateExistActorsInfo()
    if self.listOfActors_ ~= nil then 
        local count = #self.listOfActors_;
        for i = 1,count,1 do 
            local existActor = self.listOfActors_[i];
            local info = self:searchActorInfoByActorName(existActor:GetActorName());
            if info ~= nil then 
                existActor:SetActorInfo(info);
            end
        end
    end
end
]]--