requirePack("scripts.FrameWork.Global.GlobalFunctions");
local MessageManager = requirePack("scripts.Messages.MessageManager");
local LinkerManager = class("LinkerManager")
local PackageManager = requirePack("baseScripts.LinkerBase.PackageManager");
g_tConfigTable.CREATE_NEW(LinkerManager);
LinkerManager.instance_ = nil;
function LinkerManager:ctor()
    -- 设置单例
    LinkerManager.instance_ = self;

    -- 根节点
    self.rootNode_ = nil; 
    self.talkToCSharp_ = nil;
    self.messageHandle_ = nil;
    self.packageManager_ = nil;
    self.result_ = nil;
end

function LinkerManager:recv(str)
    local message = json.decode(str);
    dump(message);
    if self.messageHandle_ ~= nil then 
        self.messageHandle_:HandleMessage(message);
    end
end

-- -----对外接口 ----- 

function LinkerManager:Start()
    self.talkToCSharp_ = TalkToCSharp:GetInstance();
    self.messageHandle_ = MessageManager.new();
    self.messageHandle_:SetLinkerManager(self);-- mei有设置linkerManager
    self.packageManager_ = PackageManager.new();


    local result = self.talkToCSharp_:ConnectToCSharp();
    if result then 
        print("win32 linker to linker");
        self.talkToCSharp_:Send("hello linker");
    else 
        print("win32 not linker to linker");
    end
    if self.rootNode_~= nil then 
        self.rootNode_:runAction(cc.RepeatForever:create(cc.Sequence:create(cc.DelayTime:create(0.016),cc.CallFunc:create(function() 
            if self.talkToCSharp_ ~= nil then 
                self.talkToCSharp_:ProcessData();
            end
        end))));
    end
end

function LinkerManager:SetRootNode(n)
    self.rootNode_ = n;
end

-- ----- linker 命令 -----
function LinkerManager:ReloadRes()
    -- todo delete all res
    ArmatureDataDeal:sharedDataDeal():removeAllRes();
    self.packageManager_:Dispose();

    -- load res
    local pathOfXML =g_tConfigTable.sTaskpath .. "/LinkerData/DragonBoneDatas/";
    local pathOfPng = "";
    if true then -- 高低请
        pathOfPng = g_tConfigTable.sTaskpath .. "/LinkerData/DragonBoneDatas/Hd/";
    else 
        pathOfPng = g_tConfigTable.sTaskpath .. "/LinkerData/DragonBoneDatas/Ld/";
    end
    ArmatureDataDeal:sharedDataDeal():loadArmatureData(pathOfXML);
    ArmatureDataDeal:sharedDataDeal():loadArmatureData(pathOfPng);

    local pathOfPackageInfo = g_tConfigTable.sTaskpath .. "/LinkerData/formatProjData.json";
    self.result_ = self.packageManager_:InitByFile(pathOfPackageInfo);
    self.packageManager_:SetLinkerProjPath(g_tConfigTable.sTaskpath .. "LinkerData");
    self.packageManager_:SetRootNode(self.rootNode_ );
    self.packageManager_:SetLinkerManager(self);
    
    if self.result_ then 
        print("Reload data Complie");
        self:SMReloadComplie();
    else 
        print("init packageManager fail");
        print("Reload data Fail");
        print(debug.traceback(  ));
    end
end

function LinkerManager:StartSceneBySceneID(ID)
    if self.result_ then 
        print("ID:"..ID);
        self.packageManager_:StartScene(ID,function() end);
    else 
        print("init packageManager fail");
    end
end

function LinkerManager:StopSceneByID(ID)
    if self.result_ and self.packageManager_ ~= nil then 
        print("ID:"..ID);

        self.packageManager_:StopScene(ID);
        print("StopSceneByID complie:"..ID);
    else 
        print("self.packageManager_ ~= nil");
    end
end

function LinkerManager:PlayPlotBySceneIDAndNpcNameAndStartIndex(id,n,i)
 -- todo delete all res
 ArmatureDataDeal:sharedDataDeal():removeAllRes();
 self.packageManager_:Dispose();

 -- load res
 local pathOfXML =g_tConfigTable.sTaskpath .. "/LinkerData/DragonBoneDatas/";
 local pathOfPng = "";
 if true then -- 高低请
     pathOfPng = g_tConfigTable.sTaskpath .. "/LinkerData/DragonBoneDatas/Hd/";
 else 
     pathOfPng = g_tConfigTable.sTaskpath .. "/LinkerData/DragonBoneDatas/Ld/";
 end
 ArmatureDataDeal:sharedDataDeal():loadArmatureData(pathOfXML);
 ArmatureDataDeal:sharedDataDeal():loadArmatureData(pathOfPng);

 local pathOfPackageInfo = g_tConfigTable.sTaskpath .. "/LinkerData/formatProjData.json";
 self.result_ = self.packageManager_:InitByFile(pathOfPackageInfo);
 self.packageManager_:SetLinkerProjPath(g_tConfigTable.sTaskpath .. "LinkerData");
 self.packageManager_:SetRootNode(self.rootNode_ );
 self.packageManager_:SetLinkerManager(self);

    if self.result_ then 
        print("ID:"..id);
        self.packageManager_:StopRunningScene();
        print("PlayPlotBySceneIDAndNpcNameAndStartIndex:i:"..i);
        self.packageManager_:StartSceneBySceneIDAndNpcNameAndStartIndex(id,n,i,function() end);
    else 
        print("init packageManager fail");
    end
end



function LinkerManager:TestPostDownloadRes()
    self:SMLoadResStatueDownloading();
    g_tConfigTable.DownloadComplie = function(str) 
        self:SMLoadResStatueDownloaded();
    end
    self.talkToCSharp_:TestPost();
    --[[
    self.rootNode_:runAction(cc.Sequence:create(cc.DelayTime:create(5),cc.CallFunc:create(
        function() 
        end
    )));
    ]]--
end


-- ----- 发送消息 -----
function LinkerManager:SMLoadResStatueDownloading()
    local m = {};
    m.EventName = MessageManager.STR_MN_LOAD_RES_STATUE_UPDATE;
    m.IsLoading = true;
    local str = json.encode(m);
    self.talkToCSharp_:Send(str);
end
function LinkerManager:SMLoadResStatueDownloaded()
    local m = {};
    m.EventName = MessageManager.STR_MN_LOAD_RES_STATUE_UPDATE;
    m.IsLoading = false;
    local str = json.encode(m);
    self.talkToCSharp_:Send(str);
end

function LinkerManager:SMReloadComplie()
    local m = {};
    m.Result = true;
    m.EventName = MessageManager.STR_MN_RELOAD_RES_COMPLIE;
    local str = json.encode(m);
    self.talkToCSharp_:Send(str);
end
function LinkerManager:SMReloadFail()
    local m = {};
    m.Result = false;
    m.EventName = MessageManager.STR_MN_RELOAD_RES_COMPLIE;
    local str = json.encode(m);
    self.talkToCSharp_:Send(str);
end


-- ----- packagemanger ----- // SceneID
function LinkerManager:UpdateSceneStatue(sceneID,statue)
    local m = {};
    m.EventName  = MessageManager.STR_MN_PLAY_STATUE_CHANGE;
    m.IsPlaying = statue;
    m.SceneID = sceneID;
    local str = json.encode(m);
    self.talkToCSharp_:Send(str);
end



-- ----- 接收消息方法 -----
g_tConfigTable.Recv = function(str) 
    if LinkerManager.instance_ ~=nil then 
        xpcall(function()  
            print("recv:"..str);
            LinkerManager.instance_:recv(str);
        end, function(e)
            dump(e);
            print(debug.traceback()) 
        end)

    end
end


return LinkerManager;