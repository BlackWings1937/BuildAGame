--local AnimationEngine = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.AnimationEngine");
requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");
local ScriptUtil = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ScriptUtil");
local ZOrderPackage = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.ZOrderPackage");
local TableUtil = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Util.TableUtil");--ArmatureUtil.Create

local FileUtil = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Util.FileUtil");
local ScriptAction = class("ScriptAction")

g_tConfigTable.CREATE_NEW(ScriptAction);

ScriptAction.EnumZOrderAlign = {
    ["None"] = 0,
    ["BaseOnMaxZOrder"] = 1,
    ["BaseOnMinZOrder"] = 2,
}

function ScriptAction:ctor()
    self.tag_ = -1;
    self.eventCallbacks_ = nil;
    self.tableOfScript_ = nil;
    self.time_ = 0;
    self.armResPath_ = ""; -- 动画资源路径
    self.picResPath_ = ""; -- 图片资源路径
    self.audioResPath_ = ""; -- 声音资源路径
    self.ZOrderAlignType_ = ScriptAction.EnumZOrderAlign.None; -- 播放具体动画时 具体动画的zOrder 对其规则
    self.zOrderInfo_ = nil; -- zorder 参考信息
    self.isComplie_ = false;
    self.animationEngine_ = nil;
    self.listOfActors_ = {};
    self.runtimeActorZorders_ = {};
    self.playMode_ = nil;
    self.actorsZorderTracks_ = {};
    self.actorsOccupyInfo_ = nil;
    self.isUserTheaterContentSize_  = false;
    self.isBanZOrder_  = false;
end

function ScriptAction:SetActorOccupyInfo(i)
    self.actorsOccupyInfo_ = i;
end

function ScriptAction:GetActorOccupyInfo()
    return self.actorsOccupyInfo_;
end

function ScriptAction:SetActorZorderTracks(v) 
    self.actorsZorderTracks_ = v;
end

function ScriptAction:GetActorZorderTracks()
    return self.actorsZorderTracks_;
end

function ScriptAction:SetRuntimeActorZOrdersPackage(v)
    self.runtimeActorZordersPackage_ = v;
end

function ScriptAction:GetRuntimeActorZOrdersPackage()
    return self.runtimeActorZordersPackage_;
end

function ScriptAction:SetRuntimeActorZordersPackageByName(name,zorder)
    local pack = self.runtimeActorZordersPackage_[name];
    if pack~= nil then 
        pack:SetZOrder(zorder);
    end
end

function ScriptAction:removePackageByName(name)
    for k,v in pairs(self.runtimeActorZordersPackage_) do 
        if k == name then 
            self.runtimeActorZordersPackage_[k] = nil;
        end
    end
end

function ScriptAction:GetRuntimeActorZOrders()-- markrun
    return self.runtimeActorZorders_;
end

function ScriptAction:SetRuntimeActorZorderByName(name,zorder)
    self.runtimeActorZorders_[name] = zorder;-- markrun
end
function ScriptAction:SetRuntimeActorZorders(v)-- markrun
    self.runtimeActorZorders_ = v;
end

function ScriptAction:GetRuntimeZOrderModuleByRuntimeActorZorders()--markrun
    local module = {};
    for k,v in pairs(self.runtimeActorZorders_) do 
        local info = {};
        info.npcName = k;
        info.z = v;
        info.name = info.npcName;
        table.insert(module,info);
    end

    local count = #module;
    self:quickSortSeq(module,1,count);

    for i = 1, count ,1 do 
        module[i].npcName = module[i].name;
        module[i].z = i;
        module[i].index = i * self:GetAnimationEngine():GetZOrderDiss();
    end
    return module;
end

function ScriptAction:GetRuntimeZOrderModuleByRuntimeActorZordersPackage()
    local module = {};
    for k,v in pairs(self.runtimeActorZordersPackage_) do 
        if v ~= nil then 
            local info = {};
            info.npcName = k;
            info.z = v:GetZOrder();
            info.count = v:Count();
            info.name = info.npcName;
            table.insert(module,info);
        end
    end
    local count = #module;
   -- self:quickSortSeq(module,1,count);
    TableUtil.QuickSort(module,1,count,function(v) return v.z end);
    for i = 1, count ,1 do 
        module[i].npcName = module[i].name;
        module[i].z = i;
        module[i].index = i * self:GetAnimationEngine():GetZOrderDiss();
    end

    return module;
end

function ScriptAction:quickSortSeq(seq,left,right)
    local temp = seq[left].z ;
    local tempName = seq[left].name;
    local p = left;
    local i = p;
    local j = right;
    while i<=j do 
        while j>=p and seq[j].z  >= temp do 
            j = j - 1;
        end
        if j>= p then 
            seq[p].z  = seq[j].z ;
            seq[p].name  = seq[j].name;
            p = j;
        end
        while  i<= p  and  seq[i].z <=temp do 
            i = i + 1;
        end
        if i<=p then 
            seq[p].z  = seq[i].z ;
            seq[p].name  = seq[i].name;
            p = i;
        end
    end
    seq[p].z  = temp;
    seq[p].name = tempName;
    if p - left > 1 then 
        self:quickSortSeq(seq,left,p-1);
    end

    if right - p > 1 then 
        self:quickSortSeq(seq,p+1,right);
    end
    return seq;
end

function ScriptAction:isContain(list,v)
    local count = #list;
    for i = count , 1, -1 do 
        if list[i] == v then 
            return true;
        end
    end
    return false;
end

function ScriptAction:isInActionControl(name)
    local count = #self.tableOfScript_.layers;
    for i = count,1,-1 do 
        local layer = self.tableOfScript_.layers[i];
        if layer.layername == name then 
            local nowIndex = 1;
            while layer.frames[nowIndex].einfo == nil do
                nowIndex = nowIndex + 1;   
            end
            return true , layer.frames[nowIndex].z;
        end
    end
    return false , -1;
end

function ScriptAction:getSeqOfTempJson()
    local count = #self.tableOfScript_.layers;
    local seq = {};
    for i = count,1,-1 do 
        local layer = self.tableOfScript_.layers[i];
        if layer.layername ~= "camera" then 
            if layer.framestype == "element" or layer.framestype == "pic" then 
                local frameCount = #layer.frames;
                local listOfUsingZOrder = {};
                for z = 1,frameCount,1 do 
                    if self:isContain(listOfUsingZOrder,layer.frames[z].z) == false and layer.frames[z].einfo~= nil then 
                        table.insert(listOfUsingZOrder,layer.frames[z].z);
                        local info = {};
                        info.name = layer.layername;
                        info.z = layer.frames[z].z;-- 加入同名
                        table.insert(seq,info);
                    end
                end
            end
        end
    end
    return self:quickSortSeq(seq,1,#seq);
end

function ScriptAction:isInSeq(seq,name)
    local count = #seq ;
    for i = 1 , count ,1 do 
        local item = seq[i];
        if item.name == name then 
            return true;
        end
    end
    return false;
end

function ScriptAction:caculateMaxPointMatch(seq,runtime)
    local matchPoint = 0;
    local maxName = seq[#seq].name;
    local maxIndexOnRuntimeIndex = 0;
    local count = #runtime;
    for i = 1,count,1 do 
        local item = runtime[i];
        if item.npcName == maxName then 
            maxIndexOnRuntimeIndex = i;
            break;
        end
    end
    if maxIndexOnRuntimeIndex ~= 0 then 
        for i = maxIndexOnRuntimeIndex,count,1 do 
            if self:isInSeq(seq,runtime[i].npcName) == false then 
                -- not cotain in seq
                matchPoint = matchPoint + 1;
            else 

            end
        end
        return matchPoint;
    end
    return  -1 ;
end

        -- SetActorZorderTracks(v) GetActorZorderTracks()
function ScriptAction:getRuntimeZOrderByName(name) 
    local runtimeTrack = self:GetActorZorderTracks();
    local count = #runtimeTrack;
    for i = 1 , count, 1 do 
        if runtimeTrack[i].npcName == name then 
            return runtimeTrack[i].index;
        end
    end
    return -1;
end

function ScriptAction:getRuntimeZOrderByNameAndOrigz(name,origz)
    local runtimeTrack = self:GetActorZorderTracks();
    local count = #runtimeTrack;
    for i = 1 , count, 1 do 
        if runtimeTrack[i].npcName == name and runtimeTrack[i].z == origz then 
            return runtimeTrack[i].index;
        end
    end
    return -1;
end


function ScriptAction:checkActorOccupyProblem(seq)
    local countOfSeq = #seq;
    for i = countOfSeq,1,-1 do 
        if self.actorsOccupyInfo_[seq[i].name] == true then 
            return true;
        end
    end
    return false;
end

function ScriptAction:updateZOrderPackageBySeq(seq)

end

function ScriptAction:initZOrderData(zorderModule) 
    local runtimeZorderInfo = zorderModule;
    local seq = self:getSeqOfTempJson();

    if self:checkActorOccupyProblem(seq) then 
        return false;
    end
    local maxMartchPoint = self:caculateMaxPointMatch(seq,runtimeZorderInfo);
    if maxMartchPoint == -1 then -- name
        print("error: do not find match point in module while play:".. self.tableOfScript_.name.."\ntry to match "..seq[#seq].name.. "fail.. i dump module next..");
    end


    local newRuntime = {};
    local countOfRuntime =  #runtimeZorderInfo

    for i = countOfRuntime,1, -1 do
        if self:isInSeq(seq,runtimeZorderInfo[i].npcName)then 
            table.remove(runtimeZorderInfo,i);
        end
   end
   
    countOfRuntime =  #runtimeZorderInfo
    for i = countOfRuntime,  countOfRuntime - maxMartchPoint + 1, -1 do
        table.insert(newRuntime,1,runtimeZorderInfo[i]);
    end

    local zorderPack = ZOrderPackage.new()
    local countOfSeq = #seq;
    local maxPackageName = seq[countOfSeq].name;
    for i =countOfSeq,1 ,-1 do 
        local info = {};
        info.npcName = seq[i].name;
        info.z = seq[i].z;
        info.count = 1;
        table.insert(newRuntime,1,info)
        self:removePackageByName(info.npcName);
        zorderPack:AddActor(info.npcName);
    end

    for i = countOfRuntime - maxMartchPoint   ,1,-1 do 
        table.insert(newRuntime,1,runtimeZorderInfo[i])
    end
    

    local tempList = {};
    local packageIndex = -1;
    local countOfNewRuntime = #newRuntime; -- 一体化没有同步 problem
    local nowIndex = 1000;
    for i = countOfNewRuntime,1 ,-1 do 
        newRuntime[i].index = nowIndex * self:GetAnimationEngine():GetZOrderDiss();
        if newRuntime[i].npcName == maxPackageName then 
            packageIndex = newRuntime[i].index;
        end
        table.insert(tempList,newRuntime[i]);
        nowIndex = nowIndex - newRuntime[i].count;

        -- 设置以前的动作包
        local packOld = self:GetRuntimeActorZOrdersPackage()[newRuntime[i].npcName];
        if packOld ~= nil then 
            packOld:SetZOrderChange(newRuntime[i].index );
        end
    end

    self:GetRuntimeActorZOrdersPackage()[self.tableOfScript_.name] = zorderPack:Init(packageIndex,self.tableOfScript_.name,countOfSeq);-- 建立打包
    self.zorderPack_ = zorderPack;
    self:SetActorZorderTracks(tempList);
    --print("------------------------------start--------------------------------------------");
    --dump( tempList );
    return true;
end

function ScriptAction:GetZOrderDiss() 
    return self.zorderPack_:GetDiss();
end



function ScriptAction:initScriptFile(filePath,module,cb)
    self.eventCallbacks_ = cb;
    self.tableOfScript_ =ScriptUtil.LoadScriptCopyMode(filePath);
    -- todo :在这里更变每个显示对象关键帧的zOrder
    -- 需要传入zorder 缓存信息
    return self:initZOrderData(module);
end

function ScriptAction:initByActors(...)

end

--[[
    用户自定义事件被调用    
]]--
function ScriptAction:callCustomEvent(eventName)
    if self.eventCallbacks_ ~= nil then 
        self.eventCallbacks_(eventName,nil);
    end
end

-------------对外接口----------------------



--[[
    设置动作播放模式
]]--
function ScriptAction:SetPlayMode(m) 
    self.playMode_ = m;
end

--[[
    设置动作相对对象
]]--
function ScriptAction:SetRefNpcName(name)
    self.refNpcName_ = name;
end

function ScriptAction:GetRefNpcName() 
    return self.refNpcName_;
end

--[[
    获取动画播放模式
]]--
function ScriptAction:GetPlayMode()
    return self.playMode_;
end

--[[
    设置声音资源路径
    
    参数:
    info:声音资源
]]--
function ScriptAction:SetAudioResPath(p)
    self.audioResPath_ = p;
end

--[[
    获取声音资源路径
]]--
function ScriptAction:GetAudioResPath()
    return self.audioResPath_;
end
--[[
    设置播放动作ZOrder对齐规则

    参数:
    ZorderAlignType: ScriptAction.EnumZOrderAlign 播放动作ZOrder对齐规则
]]--
function ScriptAction:SetZOrderAlignType(ZorderAlignType)
    self.ZOrderAlignType_ = ZorderAlignType;
end

--[[
    获取 zOrder 播放对齐规则
]]--
function ScriptAction:GetZOrderAlignType()
    return self.ZOrderAlignType_;
end

--[[
    获取动画引擎
]]--
function ScriptAction:GetAnimationEngine()
    return self.animationEngine_;
end

--[[
    设置动画引擎
    
    参数:
    engine:动画播放引擎
]]--
function ScriptAction:SetAnimationEngine(engine)
    self.animationEngine_ = engine;
end

--[[
    获取动作是否已经完成的标记
]]--
function ScriptAction:GetIsComplie()
    return self.isComplie_;
end

--[[
    设置动画资源路径

    参数:
    armResPath: string 动画资源路径
]]--
function ScriptAction:SetArmResPath(armResPath)
    self.armResPath_ = armResPath;
end

--[[
    获取动画资源路径

    返回值:
    armResPath: string 动画资源路径
]]--
function ScriptAction:GetArmResPath()
    return self.armResPath_;
end


--[[
    设置图片资源路径

    参数:
    picResPath: string 图片资源路径
]]--
function ScriptAction:SetPicResPath(picResPath)
    self.picResPath_ = picResPath;
end

--[[
    返回图片资源路径

    返回值:
    picResPath: string 图片资源路径
]]--
function ScriptAction:GetPicResPath()
    return self.picResPath_;
end

function ScriptAction:GetTheaterName()
    return self.theaterLayer_:getName();
end


--[[
    初始化脚本动作对象
    参数
    scriptPath: string 剧本文件路径（绝对路径）
    cb:void:返回空(eventName:返回事件名,data:返回数据) 台本播放中的各类事件（创建npc 销毁npc 剧本播放完毕等事件）
    ...: array of Actors 角色数组
]]--
function ScriptAction:Init(scriptPath,cb,...)                                 
    self:initScriptFile(scriptPath,cb);
    self:initByActors(...);
end

--[[
    撤销动作
]]--
function ScriptAction:Dispose()--self.zorderPack_ 
    local packs =  self:GetRuntimeActorZOrdersPackage();
    local pack =packs[self.tableOfScript_.name];
    packs[self.tableOfScript_.name] = nil;
    --elf:removePackageByName(self.tableOfScript_.name);
    if pack == nil then 
        print(self.tableOfScript_.name);
    end
    local actorNames = pack:GetActors();
    for i = 1,#actorNames,1 do 
        packs[actorNames[i]] = ZOrderPackage.new():Init(self.runtimeActorZorders_[actorNames[i]],actorNames[i],1);
    end
    
    local count = # self.listOfActors_ ;
    for i = 1,count , 1 do 
        self.listOfActors_[i]:Dispose();
    end
    local zinfo = self:GetAnimationEngine():getNodeZOrderCacheByNodeName(self.theaterLayer_:getName());
    zinfo.runTimeZorders = self.runtimeActorZorders_;
    zinfo.runtimeZordersPackage = packs;
    --print("------------------------------stop--------------------------------------------");
    --dump( zinfo.runTimeZorders );
end


--[[
    删除动作的时候调用动作完成回调
]]--
function ScriptAction:DestroyActionCall() 
    if self.eventCallbacks_ ~= nil then 
        self.eventCallbacks_(self:GetAnimationEngine():GetAnimationEngineEvent().Complie,nil);
    end
end

--[[
    删除动作是调用完成的回调，返回回调信息为打断
]]--
function ScriptAction:DestroyActionCallInterupt()
    if self.eventCallbacks_ ~= nil then 
        self.eventCallbacks_(self:GetAnimationEngine():GetAnimationEngineEvent().Interupt);
    end
end

--[[
    删除动作是调用完成的回调，返回回调信息为打断
]]--
function ScriptAction:DestroyActionCallInternalInterupt()
    if self.eventCallbacks_ ~= nil then 
        self.eventCallbacks_(self:GetAnimationEngine():GetAnimationEngineEvent().InternalINterupt);
    end
end

--[[
    更新剧本动画

    参数:
    dt:更新时间差
]]--
function ScriptAction:Process(dt)
    self.time_ = self.time_ + dt;
    if self.time_ > self.tableOfScript_.common.totalDuration then
        -- 清理每一个actor 
        self.isComplie_ = true;
    end
end

--[[
    执行动画最后一帧
]]--
function ScriptAction:ProcessToEnd()
    self.time_ = self.tableOfScript_.common.totalDuration  - 0.032;
    self:Process(0.016);
end

function ScriptAction:SetTag(tag)
    self.tag_ = tag;
end

function ScriptAction:GetTag()
    return self.tag_;
end

function ScriptAction:SetIsUseTheaterContentSize(v)
    self.isUserTheaterContentSize_ = v;
end

function ScriptAction:GetIsUseTheaterContentSize()
    return self.isUserTheaterContentSize_;
end

function ScriptAction:SetIsBanZOrder(v)
    self.isBanZOrder_ = v;
end

function ScriptAction:GetIsBanZOrder()
    return self.isBanZOrder_ ;
end


return ScriptAction;
