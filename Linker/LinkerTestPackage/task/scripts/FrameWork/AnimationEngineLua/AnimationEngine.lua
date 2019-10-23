g_tConfigTable.RootFolderPath = "scripts.";
local StringUtil = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Util.StringUtil");
requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");
local FileUtil = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Util.FileUtil");
local ScriptUtil = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ScriptUtil");
local TableUtil = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Util.TableUtil");--ArmatureUtil.Create
local UniTagsManager = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.UniTagsManager");
local ScriptAction = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ScriptAction");
local ZOrderPackage = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ZOrderPackage");
local ScriptActionExCamera = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ScriptActionExCamera");
local CustomDefineNameInfo = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.CustomActualNameConfig");
local AnimationEngine = class("AnimationEngine");
g_tConfigTable.CREATE_NEW(AnimationEngine);

AnimationEngine.EnumAnimationEvent = {
    ["Complie"] = "Complie", -- 台本播放完成事件
    ["Interupt"] = "Interupt", -- 打断完成事件
    ["InternalINterupt"] = "InternalINterupt",-- 内部打断事件
}

AnimationEngine.EnumPlayScriptMode = {
    ["ALL"] = "ALL",
    ["ZORDER_ONLY"] = "ZORDER_ONLY",
    ["REF"] = "REF",
    ["ABS"] = "ABS",
}

AnimationEngine.EnumPlayScriptZOrderMode = {
    ["CACHE_MOUDLE"] = 1,
    ["AUTO"] = 2,
}

AnimationEngine.CAMERA_REF = "ref";
AnimationEngine.CAMERA_ABS = "abs";
AnimationEngine.CAMERA_FIX = "fix";


AnimationEngine.STR_CREATE_SHOWOBJECT_PRE = "AESOP";
AnimationEngine.MAX_INDEX = 10000000; 

AnimationEngine.ZORDER_DISS = 100; -- 每个显示对象之间相差的层级

AnimationEngine.MAX_DT = 0.032;

function AnimationEngine:ctor()
    self.isInited_ = false;
    self.preWorkTime_ = 2;
end


function AnimationEngine:init() 

    -- 动画事件枚举
    self.enumEvent_ = AnimationEngine.EnumAnimationEvent;

    -- 播放脚本模式
    self.enumPlayMode_ =  AnimationEngine.EnumPlayScriptMode;

    -- 播放的zorder 排列模式
    self.enumPlayZOrderMode_ = AnimationEngine.EnumPlayScriptZOrderMode;

    -- 动画引擎创建对象名称头部
    self.animationEngineCreatePre_ = AnimationEngine.STR_CREATE_SHOWOBJECT_PRE;
    
    -- 动作对象 唯一 id 管理器
    self.uniTagManagerActions_ = UniTagsManager.new();
    
    -- 层次节点对象 唯一 id 管理器
    self.uniTagManagerZOrderObjs_ = UniTagsManager.new();
    
    -- list of scriptActions 
    self.listOfScriptActions_ = {};
    
    -- 缓存动画引擎创建的动画名
    self.listOfSONamesCache_ = {};
    
    -- 缓存场景层次字典
    self.dicOfZOrderInfo_ = {};
    
    -- 每个显示对象之间的zorder 间隔
    self.ZORDER_DISS_ = AnimationEngine.ZORDER_DISS;
    
    -- 引擎是否被初始化
    self.isInited_ = true;

   -- 注册更新函数

   self.updateHandle_ = cc.Director:getInstance():getScheduler():scheduleScriptFunc(function(dt) 
       dt = math.min(dt,AnimationEngine.MAX_DT);
       self:update(dt);
   end,0,false);   --[[]]--

end

-----------------私有方法-----------------

function AnimationEngine:getMinValueNameInTable(table)
    local minValue = AnimationEngine.MAX_INDEX;
    local minValueName = "";
    for k,v in pairs(table) do 
        if v < minValue then 
            minValueName = k;
            minValue = v;
        end
    end
    return minValueName;
end



--[[
    缓存舞台节点zOrder缓存信息

    参数：
    nodeName: string 节点名称
    fileInfo: string 层次文件全路径(绝对路径)
]]--
function AnimationEngine:setNodeZOrderCache(nodeName,fileInfo)
    local info = self.dicOfZOrderInfo_[nodeName];
    if info == nil then 
        info = {};
        info.module = {};         -- 文件中的zorder 信息
        info.runtime = {};        -- 运行时的zorder 信息
        info.countOfRunningActions = 0;
        info.runTimeZorders = {}; -- 实际舞台上各个显示对象的实时zorder 信息 markrun
        info.runtimeZordersPackage={};
        info.runTimeOccupy = {};
        info.defaultActorMoveSpeed = 200; -- 默认角色移动速度
        info.filePath = fileInfo;
        self.dicOfZOrderInfo_[nodeName] = info;
    else 
        if info.filePath == fileInfo then 
            return ;
        else 
            info.filePath = fileInfo;
            info.countOfRunningActions = 0;
            info.runTimeZorders = {}; -- 实际舞台上各个显示对象的实时zorder 信息 markrun
            info.runtimeZordersPackage={};
            info.runTimeOccupy = {};
        end
    end 
    local zOrderInfo = ScriptUtil.LoadScript(fileInfo);    
    if zOrderInfo.sceneInfo ~= nil then 
        info.sceneInfo = sceneInfo;
    else 
        print("warning zOrder module file do not have scenesize");
    end

    local elementCount = #zOrderInfo.layers;
    local showObjCount = 0;
    local tempTable = {};
    local index = 1;
    for i = elementCount,1,-1 do 
        local itemInfo = zOrderInfo.layers[i];
        if itemInfo.layername ~= "camera" then 
            if itemInfo.framestype == "element" or itemInfo.framestype == "pic" then 
                tempTable[index] = {["npcName"] = itemInfo.layername,["index"] = -1 , z =  itemInfo.frames[1].z};
                index = index + 1;
            end
        end
    end
    local countOfTempTable = #tempTable;
    TableUtil.QuickSort(tempTable,1,countOfTempTable,function(v) return v.z end);

    local runtime = {};
    local nowIndex = 1000;
    for i = 1,countOfTempTable,1 do 
        tempTable[i].z = i;
        tempTable[i].index = nowIndex * AnimationEngine.ZORDER_DISS;
        tempTable[i].count = 1;
        runtime[i] = {};
        runtime[i].npcName = tempTable[i].npcName;
        runtime[i].z = tempTable[i].z;
        runtime[i].index = tempTable[i].index;
        info.runTimeZorders[runtime[i].npcName ] = runtime[i].index ;-- markrun
        info.runtimeZordersPackage[runtime[i].npcName] = ZOrderPackage.new():Init(runtime[i].index,runtime[i].npcName,1);
        info.runtimeZordersPackage[runtime[i].npcName]:AddActor(runtime[i].npcName);
        info.runTimeOccupy [runtime[i].npcName] = false;
        nowIndex = nowIndex - 1;
    end
    info.module = tempTable;
    info.runtime = runtime;
end

--[[
    获取某一个节点的层次信息

    参数:
    nodeName:节点名称
]]--
function AnimationEngine:getNodeZOrderCacheByNodeName(nodeName)
    return self.dicOfZOrderInfo_[nodeName];
end

--[[
    清除一个节点的缓存zorder 信息

    参数:
    nodeName:节点名称
]]--
function AnimationEngine:clearNodeZorderCacheByName(nodeName)
    self.dicOfZOrderInfo_[nodeName] = nil;
end

--[[
    清除所有缓存zorder 信息
]]--
function AnimationEngine:clearAllNodeZorderCaches()
    self.dicOfZOrderInfo_ = {};
end

--[[
    添加名称到缓存名称的条目
    
    参数:
    name: string 显示对象名称
]]--
function AnimationEngine:AddNameToCache(name)
    local count = #self.listOfSONamesCache_;
    for i = 1,count,1 do 
        if name == self.listOfSONamesCache_[i] then 
            return false;
        end
    end
    table.insert(self.listOfSONamesCache_,name);
    return true;
end

--[[
    移除名称从名称缓存

    参数:
    name: string 要移除的显示对象名称
]]--
function AnimationEngine:RemoveNameFromCache(name)
    local count = #self.listOfSONamesCache_;
    for i = count,1,-1 do 
        if self.listOfSONamesCache_[i] == name then 
            table.remove(self.listOfSONamesCache_,i);
            return true;
        end
    end
    return false;
end

--[[
    清空名称缓存
]]--
function AnimationEngine:ClearNamesCache()
    self.listOfSONamesCache_ = {};
end

--[[
    停止一个舞台的所有动作
]]--
function AnimationEngine:StopAllActionsOnTheater(theater)
    local count = #self.listOfScriptActions_;
    local name = theater:getName();--
    for i = count, 1 , -1 do 
        if name == self.listOfScriptActions_[i]:GetTheaterName() then 
            local sa = self.listOfScriptActions_[i];
            sa:ProcessToEnd();
            sa:Dispose();
            sa:DestroyActionCallInternalInterupt();
            self.uniTagManagerActions_:RecycleUniId(sa:GetTag());
            table.remove(self.listOfScriptActions_,i);
        end
    end
end

--[[
    获取引擎创建的显示对象头标号
]]--
function AnimationEngine:GetAnimationEngineCreatePre()
    return self.animationEngineCreatePre_;
end

--[[
    获取动画引擎事件枚举
]]--
function AnimationEngine:GetAnimationEngineEvent()
    return self.enumEvent_;
end

--[[
    获取动画引擎播放模式枚举
]]--
function AnimationEngine:GetAnimationPlayModeEnum() 
    return self.enumPlayMode_;
end

--[[
    获取动画播放zorder 模式枚举
]]--
function AnimationEngine:GetAnimationPlayZOrderModeEnum()
    return self.enumPlayZOrderMode_;
end

--[[
    更新所有在跑的台本
]]--
function AnimationEngine:update(dt)
    local count = #self.listOfScriptActions_;
    for i = count,1 , -1 do 
        local sa = self.listOfScriptActions_[i];
        sa:Process(dt);
        if sa:GetIsComplie() then 
            table.remove(self.listOfScriptActions_,i);
            sa:Dispose();--
            sa:DestroyActionCall();
            self.uniTagManagerActions_:RecycleUniId(sa:GetTag());
        end
    end
end

--[[
    获取zorder 信息

    参数:
    node: ccnode 舞台节点
    zorderFilePath: string zorder 参考文件信息
]]--
function AnimationEngine:getZOrderInfo(node,zorderFilePath)
    if zorderFilePath ~= nil and zorderFilePath ~= "" then 
        
        local nodeName = node:getName();
        if nodeName == "" then 
            nodeName = "PREAE_"..self.uniTagManagerZOrderObjs_:GetUniId();
            node:setName(nodeName);
        end
        return self:getNodeZOrderCacheByNodeName(nodeName);
    end
    return nil;
end


function AnimationEngine:IsTheaterNodeBeZorderInit(node)
    
    if node == nil then 
        return false;
    end

    local nodeName = node:getName() ;
    if nodeName == "" then 
        return false;
    end

    local zorderModuleInfo = self:getNodeZOrderCacheByNodeName(nodeName);
    if zorderModuleInfo == nil then 
        return false;
    end

    if zorderModuleInfo.module == nil or zorderModuleInfo.runtime == nil or zorderModuleInfo.filePath == nil then 
        return false;
    end

    return true;
end

function AnimationEngine:copyTable(t)
    local bt = {};
    for k,v in pairs(t) do 
        bt[k] = v;
    end
    return bt;
end

--[[
    将角色名称转换成实际显示对象名称
]]--
function AnimationEngine:TransformEngineNameToActualName(n)
    return CustomDefineNameInfo.ToActualNameDic[n] or n;
end

--[[
    将实际显示对象名称转换成显示对象名称
]]--
function AnimationEngine:TransformActualNameToEngineName(n)
    return CustomDefineNameInfo.ToActorNameDic[n] or n;
end

--[[检查脚本类型]]--
function AnimationEngine:checkCameraType(scriptTable) 
    local layers = scriptTable.layers;
    local count = #layers;
    for i = 1,count,1 do 
        if layers[i].layername == "camera" then 
            local layer = layers[i];
            local cName = layer.frames[1].einfo.ename;
            if string.find(cName,AnimationEngine.CAMERA_FIX) then 
                return AnimationEngine.CAMERA_FIX;
            elseif string.find(cName,AnimationEngine.CAMERA_ABS) then 
                return AnimationEngine.CAMERA_ABS;
            elseif string.find(cName,AnimationEngine.CAMERA_REF) then 
                return AnimationEngine.CAMERA_REF ,cName;
            end
        end
    end
    return "";
end

function AnimationEngine:playScriptRef(filePath,node,picResPath,audioResPath,refNpcName,cb,zorderMode,isUseDefaultContentSize,isBanZorder)
    local saec = ScriptActionExCamera.new();
    saec:SetIsBanZOrder(isBanZorder);
    saec:SetIsUseTheaterContentSize(isUseDefaultContentSize);
    saec:SetPicResPath(picResPath);
    saec:SetAudioResPath(audioResPath);
    saec:SetZOrderAlignType(ScriptAction.EnumZOrderAlign.BaseOnMaxZOrder);
    saec:SetPlayMode(self:GetAnimationPlayModeEnum().REF);--播放方式异化
    saec:SetRefNpcName(refNpcName);
    saec:SetAnimationEngine(self);--

    local info = self:getNodeZOrderCacheByNodeName(node:getName());
    saec:SetRuntimeActorZorders(info.runTimeZorders);
    saec:SetRuntimeActorZOrdersPackage(info.runtimeZordersPackage);
    saec:SetActorOccupyInfo(info.runTimeOccupy);

    local result = false;
    if zorderMode == AnimationEngine.EnumPlayScriptZOrderMode.AUTO then 
        result = saec:Init(filePath,node,saec:GetRuntimeZOrderModuleByRuntimeActorZordersPackage(),cb);
    elseif zorderMode == AnimationEngine.EnumPlayScriptZOrderMode.CACHE_MOUDLE then 
        result = saec:Init(filePath,node,ScriptUtil.deepcopy(info.module),cb);
    end
    if result == false then 
        return -1;
    end

    local tag = self.uniTagManagerActions_:GetUniId();
    saec:SetTag(tag);
    saec.time_ = 0;
    table.insert(self.listOfScriptActions_,saec);
    return tag
end

function AnimationEngine:playScriptALL(filePath,node,picResPath,audioResPath,cb,zorderMode,isUseDefaultContentSize,isBanZorder)
    local saec = ScriptActionExCamera.new();
    saec:SetIsBanZOrder(isBanZorder);
    saec:SetIsUseTheaterContentSize(isUseDefaultContentSize);
    saec:SetPicResPath(picResPath);
    saec:SetAudioResPath(audioResPath);
    saec:SetZOrderAlignType(ScriptAction.EnumZOrderAlign.BaseOnMaxZOrder);
    saec:SetPlayMode(self:GetAnimationPlayModeEnum().ALL);--播放方式异化
    saec:SetAnimationEngine(self);--

    local info = self:getNodeZOrderCacheByNodeName(node:getName());
    saec:SetRuntimeActorZorders(info.runTimeZorders);
    saec:SetRuntimeActorZOrdersPackage(info.runtimeZordersPackage);
    saec:SetActorOccupyInfo(info.runTimeOccupy);

    local result = false;
    if zorderMode == AnimationEngine.EnumPlayScriptZOrderMode.AUTO then 
        result = saec:Init(filePath,node,saec:GetRuntimeZOrderModuleByRuntimeActorZordersPackage(),cb);
    elseif zorderMode == AnimationEngine.EnumPlayScriptZOrderMode.CACHE_MOUDLE then 
        result = saec:Init(filePath,node,ScriptUtil.deepcopy(info.module),cb);
    end
    if result == false then 
        return -1;
    end

    local tag = self.uniTagManagerActions_:GetUniId();
    saec:SetTag(tag);
    saec.time_ = 0;
    table.insert(self.listOfScriptActions_,saec);
    return tag
end




function AnimationEngine:playScriptAbs(filePath,node,picResPath,audioResPath,cb,zorderMode,isUseDefaultContentSize,isBanZorder)
    local saec = ScriptActionExCamera.new();
    saec:SetIsBanZOrder(isBanZorder);
    saec:SetIsUseTheaterContentSize(isUseDefaultContentSize);
    saec:SetPicResPath(picResPath);
    saec:SetAudioResPath(audioResPath);
    saec:SetZOrderAlignType(ScriptAction.EnumZOrderAlign.BaseOnMaxZOrder);
    saec:SetPlayMode(self:GetAnimationPlayModeEnum().ABS);--播放方式异化
    saec:SetAnimationEngine(self);--

    local info = self:getNodeZOrderCacheByNodeName(node:getName());
    saec:SetRuntimeActorZorders(info.runTimeZorders);
    saec:SetRuntimeActorZOrdersPackage(info.runtimeZordersPackage);
    saec:SetActorOccupyInfo(info.runTimeOccupy);

    local result = false;
    if zorderMode == AnimationEngine.EnumPlayScriptZOrderMode.AUTO then 
        result = saec:Init(filePath,node,saec:GetRuntimeZOrderModuleByRuntimeActorZordersPackage(),cb);
    elseif zorderMode == AnimationEngine.EnumPlayScriptZOrderMode.CACHE_MOUDLE then 
        result = saec:Init(filePath,node,ScriptUtil.deepcopy(info.module),cb);
    end
    if result == false then
        print("Error json error scriptaction init fail:"..filePath);
        print(debug.traceback());
        return -1;
    end

    local tag = self.uniTagManagerActions_:GetUniId();
    saec:SetTag(tag);
    saec.time_ = 0;
    table.insert(self.listOfScriptActions_,saec);
    return tag
end

function AnimationEngine:GetCameraSize(node)
    local startPos = node:convertToNodeSpace(cc.p(0,0));
    local stopPos = node:convertToNodeSpace(cc.p(VisibleRect:winSize().width,VisibleRect:winSize().height));
    return cc.size(math.abs(startPos.x-stopPos.x),math.abs(startPos.y-stopPos.y));
end
--[[
    获取显示对象之间的层级差
]]--
function AnimationEngine:GetZOrderDiss()
    return  self.ZORDER_DISS_;
end

function AnimationEngine:GetPreWorkTime()
    return self.preWorkTime_;
end

function AnimationEngine:SetPreWorkTime(v)
    self.preWorkTime_ = v;
end


-----------------对外接口------------------

--[[
    获取摄像机位置

    参数:
    n:舞台节点
]]--
function AnimationEngine:GetCameraPos(n)
    if n ~= nil then 
        local size = n:getContentSize();
        local anchorPoint = n:getAnchorPoint();
        return cc.p(anchorPoint.x/size.width,anchorPoint.y/size.height);
    end
    return cc.p(0,0);
end

--[[
    设置摄像机位置

    参数:
    n:舞台节点
    p:摄像机位置
]]--
function AnimationEngine:SetCameraPos(n,p)
    if n ~= nil and p ~= nil then
        local size = n:getContentSize(); 
        n:setAnchorPoint(cc.p(p.x/size.width,p.y/size.height));
    end
end

--[[
    设置舞台下的角色默认的移动速度

    参数:
    v: number 默认角色移动速度
    n: ccNode 舞台节点
]]--
function AnimationEngine:SetActorDefaultSpeedValue(v,n)
    local info = self:getNodeZOrderCacheByNodeName(n:getName());
    if info~= nil then 
        info.defaultActorMoveSpeed = v;
    else 
        print("error did not init the theater node in SetActorDefaultSpeedValue nodeName:"..n:getName());
    end
end

--[[
    获取默认角色移动速度

    参数:
    n：ccNode 舞台节点
]]--
function AnimationEngine:GetActorDefaultSpeedValue(n) 
    local info = self:getNodeZOrderCacheByNodeName(n:getName());
    if info ~= nil then 
        return info.defaultActorMoveSpeed
    else
        print("error did not init the theater node in GetActorDefaultSpeedValue nodeName:"..n:getName());
        return -1;
    end
end


--[[
    根据配置信息刷新场景，只刷新显示对象的ZOrder 属性

    参数:
    filePath: string bgconfig 文件路径，绝对路径
    node: ccnode 舞台节点
]]--
function AnimationEngine:InitTheater(filePath,node)
    local scriptTable = ScriptUtil.LoadScript(filePath);
    if scriptTable == nil then 
        print("Error: AnimationEngine can not parse json file:"..filePath);
        print(debug.traceback());
        return -1;
    end
    self:StopAllActionsOnTheater(node);
    self:getZOrderInfo(node,filePath);
    self:setNodeZOrderCache(node:getName(),filePath);
end


--[[
    刷新节点zorder 层级

    参数:
    filePath: string bgconfig 文件路径，绝对路径
    node: ccnode 舞台节点
]]--
function AnimationEngine:RefreshZOrderByFile(filePath,node)
    self:InitTheater(filePath,node);
    local info = self:getNodeZOrderCacheByNodeName(node:getName());
    for k,v in pairs(info.runTimeZorders) do 
        local child =  node:getChildByName(self:TransformEngineNameToActualName(self:GetAnimationEngineCreatePre().."*"..k));
        if child ~= nil then 
            child:setLocalZOrder(v);
        end
    end
end

--[[ 
    要注意的枚举值
        AnimationEngine.EnumAnimationEvent = {
            ["Complie"] = "Complie", -- 台本播放完成事件
        }
    调用例子：
         g_tConfigTable.AnimationEngine.GetInstance():GetAnimationEngineEvent().Complie
]]--

--[[
    最新的播放接口
    filePath:脚本路径名称
    node:舞台节点
    picResPath:图片资源绝对路径
    audioResPath:声音资源绝对路径
    cb:回调方法回调函数 function(eventName(事件名称),data) end  播放完成事件:"Complie" 主动打断时的事件 "Interupt" 或者如果动画中有自定义事件XX的话，那么 eventName 为 XX
    zorderMode:1 的时候是按照bgConfig中的排列顺序 2的时候为目前显示对象的排列顺序
    isUseDefaultContentSize: true为当前节点的contentSize 初始化舞台节点 false 为使用json 中定义的场景大小初始化场景
    isBanZOrder: true 的话冻结动画显示对象zorder的变更，false 的话不冻结显示对象的zorder变更
]]--
function AnimationEngine:PlayScriptIntelligent(filePath,node,picResPath,audioResPath,cb,zorderMode,isUseDefaultContentSize,isBanZOrder)
 
    print(debug.traceback());
    print("playScript--------------------------------------------------------:"..filePath);
    local scriptTable = ScriptUtil.LoadScript(filePath);
    if scriptTable == nil then 
        print("Error: AnimationEngine can not parse json file:"..filePath);
        print(debug.traceback());
        return -1;
    end
    zorderMode = zorderMode or AnimationEngine.EnumPlayScriptZOrderMode.AUTO;
    isUseDefaultContentSize = isUseDefaultContentSize or false;
    isBanZOrder = isBanZOrder or false;
    local zorderModule =  self:getZOrderInfo(node,filePath);
    if zorderModule == nil then 
        self:setNodeZOrderCache(node:getName(),filePath);
        zorderModule =  self:getZOrderInfo(node,filePath);
    end

    -- 判断是否在有动作跑的时候使用模板模式
    if zorderMode == AnimationEngine.EnumPlayScriptZOrderMode.CACHE_MOUDLE then 
        if 0 < zorderModule.countOfRunningActions then 
            print("error theater action more than zero but you try to reset zorderMode in AnimationEngine:PlayScriptIntelligent");
            return -1;
        end
    end
    local result,cName = self:checkCameraType(scriptTable);
    local tag = -1;
    if result == AnimationEngine.CAMERA_FIX then 
        -- 固定位置播放模式(直接将显示对象扯到目标位置进行播放)
        tag= self:playScriptALL(filePath,node,picResPath,audioResPath,cb,zorderMode,isUseDefaultContentSize,isBanZOrder);
    elseif result == AnimationEngine.CAMERA_ABS then 
        -- 绝对相机处理（如果显示对象没有再目标位置，则插入关键帧，移动显示对象到目标位置）
        tag = self:playScriptAbs(filePath,node,picResPath,audioResPath,cb,zorderMode,isUseDefaultContentSize,isBanZOrder);
    elseif result == AnimationEngine.CAMERA_REF then
        -- 相对相机处理（以目标显示对象的位置为坐标系进行播放） 
        local name = StringUtil.GetN_Behind(cName,3);
        tag = self:playScriptRef(filePath,node,picResPath,audioResPath,name or "",cb,zorderMode,isUseDefaultContentSize,isBanZOrder);
    else
        print("Error json miss camera type:"..filePath);
        print(debug.traceback());
    end
    if tag == -1 then 
        print("miss:"..filePath);
    end

    return tag;

end


--[[
    ---------------------------- 面向使用场景封装的接口 ------------------------------
]]--

--[[
    主场景使用接口
    filePath:脚本路径名称
    node:舞台节点
    picResPath:图片资源绝对路径
    audioResPath:声音资源绝对路径
    cb:回调方法回调函数 function(eventName(事件名称),data) end   播放完成事件:"Complie" 主动打断时的事件 "Interupt"或者如果动画中有自定义事件XX的话，那么 eventName 为 XX
]]--
function AnimationEngine:PlayMainScreen(filePath,node,picResPath,audioResPath,cb)
    self:InitTheater(filePath, node);
    return self:PlayScriptIntelligent(filePath,node,picResPath,audioResPath,cb,2,true);
end


--[[
    主界面中使用的播放bgconfig的接口
    filePath:脚本路径名称
    node:舞台节点
    picResPath:图片资源绝对路径
    audioResPath:声音资源绝对路径
    cb:回调方法回调函数 function(eventName(事件名称),data) end   播放完成事件:"Complie" 主动打断时的事件 "Interupt"
]]--
function AnimationEngine:PlayMainScreenBgConfig(filePath,node,picResPath,audioResPath,cb)
    self:InitTheater(filePath, node);
    local tag = self:PlayScriptIntelligent(filePath,node,picResPath,audioResPath,cb,1,true);
    local sa = self:GetActionByTagId(tag) ;
    if sa ~= nil then 
        sa:Process(0.016);
    end
    return tag;
end

--[[
    主界面中使用的播放action的接口
    filePath:脚本路径名称
    node:舞台节点
    picResPath:图片资源绝对路径
    audioResPath:声音资源绝对路径
    cb:回调方法回调函数 function(eventName(事件名称),data) end   播放完成事件:"Complie" 主动打断时的事件 "Interupt"
]]--
function AnimationEngine:PlayMainScreenAction(filePath,node,picResPath,audioResPath,cb)
    return self:PlayScriptIntelligent(filePath,node,picResPath,audioResPath,cb,1,true);
end




--[[
    内容包中使用的播放bgconfig的接口
    filePath:脚本路径名称
    node:舞台节点
    picResPath:图片资源绝对路径
    audioResPath:声音资源绝对路径
    cb:回调方法回调函数 function(eventName(事件名称),data) end   播放完成事件:"Complie" 主动打断时的事件 "Interupt"
]]--
function AnimationEngine:PlayPackageBgConfig(filePath,node,picResPath,audioResPath,cb)
    self:InitTheater(filePath, node);
    local tag = self:PlayScriptIntelligent(filePath,node,picResPath,audioResPath,cb,1,false);
    local sa = self:GetActionByTagId(tag) ;
    if sa ~= nil then 
        sa:Process(0.016);
    end
    return tag;
end

--[[
    内容包中使用的播放action的接口
    filePath:脚本路径名称
    node:舞台节点
    picResPath:图片资源绝对路径
    audioResPath:声音资源绝对路径
    cb:回调方法回调函数 function(eventName(事件名称),data) end   播放完成事件:"Complie" 主动打断时的事件 "Interupt"
]]--
function AnimationEngine:PlayPackageAction(filePath,node,picResPath,audioResPath,cb)
    return self:PlayScriptIntelligent(filePath,node,picResPath,audioResPath,cb,1,false);
end


--[[
    停止剧本

    说明:
    直接停止剧本的执行

    参数:
    tag： int 剧本动作对应的唯一id

    返回值:
    void 
]]--
function AnimationEngine:StopPlayStory(tag)
    local count = #self.listOfScriptActions_;
    for i = count,1,-1 do 
        local sa = self.listOfScriptActions_[i];
        if sa:GetTag() == tag then 
            sa:Dispose();
            sa:DestroyActionCallInterupt();
            self.uniTagManagerActions_:RecycleUniId(sa:GetTag());
            table.remove(self.listOfScriptActions_,i);
        end
    end
end

--[[
    将剧本跳跃到最后一帧

    参数:
    tag: int 剧本动作对应的唯一id
]]--
function AnimationEngine:ProcessActionToEndByTag(tag)
    local count = #self.listOfScriptActions_;
    for i = count,1,-1 do 
        local sa = self.listOfScriptActions_[i];
        sa:ProcessToEnd();
        break;
    end
end



--[[
    获取动作，根据tagid 
]]--
function AnimationEngine:GetActionByTagId(id)
    local count = #self.listOfScriptActions_;
    for i = count,1,-1 do 
        local sa = self.listOfScriptActions_[i];
        if sa:GetTag() == id then 
            return sa;
        end
    end
    return nil;
end

--[[
    撤销动画引擎

    说明:
    在每次不再使用动画引擎后需要撤销动画引擎1
]]--
function AnimationEngine:Dispose()
    local count = #self.listOfScriptActions_;
    for i = count,1,-1 do 
        local sa = self.listOfScriptActions_[i];
        sa:Dispose();
        sa:DestroyActionCallInterupt();
        self.uniTagManagerActions_:RecycleUniId(sa:GetTag());
        table.remove(self.listOfScriptActions_,i);
    end
    self:clearAllNodeZorderCaches();
    cc.Director:getInstance():getScheduler():unscheduleScriptEntry(self.updateHandle_);
    self.isInited_ = false;
end

--[[
    移除所有由引擎创建的节点

    参数:
    node:需要删除引擎创建的所有显示对象
]]--
function AnimationEngine:RemoveEngineCreatedObjOnNode(node)
    if node ~= nil then 
        local listOfChildren = node:getChildren();
        local count = node:getChildrenCount();--#listOfChildren;
        local index = 0;
        for i = count ,1,-1 do 
            local child = listOfChildren[i];
            if string.find( self:TransformActualNameToEngineName(child:getName()) ,AnimationEngine.STR_CREATE_SHOWOBJECT_PRE)~= nil then --
                child:removeFromParent();
                index = index + 1;
            end
        end
    end
end


--[[
    动画引擎单例
]]--
g_tConfigTable.animationEngineInstance_ = nil;
g_tConfigTable.AnimationEngine = {};
g_tConfigTable.AnimationEngine.GetInstance = function()
   
    if g_tConfigTable.animationEngineInstance_ == nil then 
        g_tConfigTable.animationEngineInstance_ = AnimationEngine.new();
    end
  
    if g_tConfigTable.animationEngineInstance_.isInited_ == false then 
        g_tConfigTable.animationEngineInstance_:init();
    end
       --[[]]--
    return g_tConfigTable.animationEngineInstance_;
    
end
