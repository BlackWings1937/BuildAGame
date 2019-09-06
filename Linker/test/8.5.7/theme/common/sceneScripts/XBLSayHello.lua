
-- -------------------- 每个玩法必须拥有的声明方法 ---------------------------------
local WaitToPoint = requirePack("sceneScripts.WaitToPoint");
local tXBLEventBase = requirePack("sceneScripts.XBLEventBase");
local XBLSayHello = class("XBLSayHello", tXBLEventBase.new());
 
-- ---------------------------------------------------------------------------------------------
-- 自定义事件名称
local HomeEventName = requirePack("sceneScripts.HomeEvent");

-- 已经创建初步初始化完成
function XBLSayHello:ctor()
    print("XBLSayHello xiewentao 12138");
	-- 说话和动作系列的处理
	self.sayOrActCon = nil;
	self.pHomeWorld = Home7World:getCurInstance();
	self.pXbl = self.pHomeWorld:getXBL();
	self.cameraScale = self.pHomeWorld:getCemeraScale();
    self.dragonShowObject =  Home7World:getCurInstance():getHomeNpcShowByName("XBL") ;
    local x,y = self.dragonShowObject:getPosition(); 
    self.centerPos = cc.p(x,y);

    -- 初始化路径
    local pathBase = GET_REAL_PATH_ONLY("", PathGetRet_ONLY_BASIC);
    self.imgPath_ =cc.FileUtils:getInstance():fullPathForFilename(pathBase.."theme/common/sayHelloGuide/HDImage/") ;--cc.FileUtils:getInstance():fullPathForFilename(THEME_FILE("sayHelloGuide/HDImage/"));-- todo 判断高低清
    self.scriptPath_ = cc.FileUtils:getInstance():fullPathForFilename(pathBase.."theme/common/sayHelloGuide/story/");-- cc.FileUtils:getInstance():fullPathForFilename(THEME_FILE("sayHelloGuide/story/"));
    self.audioPath_ =pathBase.."theme/common/sounds/";--cc.FileUtils:getInstance():fullPathForFilename(THEME_FILE("sounds/"));


    self.actionTagOfAnimationEngine_ = -1;

     Home7World:getCurInstance():setOnlyScrollEnable(false);
 
end
-- 是否能被不在镜头中的小伴龙打断
function XBLSayHello:isCanNotInScreenXBLIntrrupt()
    return false;
end

-- 是否必须中断XBL的动作
function XBLSayHello:isMustStopXbl()
	return true;
end

function XBLSayHello:cemeraMove(isFast)
    local fTmpX = 0;
    local fTmpY = 0;
    fTmpX,fTmpY = self.pHomeWorld:getSceneMiddlePos(fTmpX, fTmpY, true);
    local pXbl = self.pHomeWorld:getHomeNpcShowByName(HomeEventName.XBL);
    if isFast == true then
        local tMove1 = cc.Place:create(cc.p(pXbl:getPositionX(), fTmpY))
        self.pHomeWorld:runCemeraByAction(tMove1)
    else
        local tMove1 = cc.MoveTo:create(0.02, cc.p(pXbl:getPositionX(), fTmpY))
        self.pHomeWorld:runCemeraByAction(tMove1)
    end
end

-- 普通打招呼
function XBLSayHello:playHello()
    print("XBLSayHello:playHello 61 61 61");
	self:removeSayOrActControl();
    local cameraPos = Home7World:getCurInstance():getCemeraPosition(true)
--	local  cemeraX = cameraPos.x
    math.randomseed(tostring(os.time()):reverse():sub(1, 6)) 
    local nHelloType = 1
    -- 增加冲浪打招呼
    if ThemeUtil:getInstance().getCurThemeVersion ~= nil and ThemeUtil:getInstance():getCurThemeVersion() >= 12 then
        nHelloType = math.random(4)
    else
        nHelloType = math.random(3)
    end  
    local cbOfSayHello = function(eventName)
        if eventName == "Complie" then 
           self:removeSayOrActControl();
           self:playNormalHelloEnd();
        elseif eventName == "Interupt"  then --正确
        end
    end 
    local jsonName = "SayHello_football.json";
    if nHelloType == 1 then --踢足球Home7World:getCurInstance() xblactionTouchHeadC_27 
        self:cemeraMove();
        jsonName = "SayHello_football.json"
    elseif nHelloType == 2 then
        self:cemeraMove();
        local xbl = Home7World:getCurInstance():getChildByName("XBL");
        xbl:setPositionX(xbl:getPositionX()+450);
        jsonName = "SayHello_run.json"
    elseif nHelloType == 3 then
        self:cemeraMove(true);
        jsonName = "SayHello_car.json"
    elseif nHelloType == 4 then
        self:cemeraMove();
        jsonName = "SayHello_surf.json"
        local xbl = Home7World:getCurInstance():getChildByName("XBL");
        xbl:setVisible(false);
        xbl:runAction(cc.Sequence:create(cc.DelayTime:create(0.016), cc.CallFunc:create(function() xbl:setVisible(true); end)));
    end
    self:playByJsonName(jsonName,cbOfSayHello);
    local xbl = Home7World:getCurInstance():getChildByName("XBL");
    local x,y = xbl:getPosition();
    print("test3.png mark");
    local sp = cc.Sprite:create("test3.png");
    if(sp == nil) then  print("sp == nil") else 
        print("sp ~= nil")
    end
	sp:setPosition(cc.p(0,0));
    xbl:addChild(sp,100000000000);
end

function XBLSayHello:playByJsonName(jsonName,cb)
    g_tConfigTable.AnimationEngine:GetInstance():StopPlayStory(self.actionTagOfAnimationEngine_);
    self.actionTagOfAnimationEngine_ = g_tConfigTable.AnimationEngine:GetInstance():PlayMainScreen(
        self.scriptPath_..jsonName,
        Home7World:getCurInstance(),
        self.imgPath_,
        self.audioPath_,
        cb);
    local sa = g_tConfigTable.AnimationEngine.GetInstance():GetActionByTagId( self.actionTagOfAnimationEngine_);
    if sa ~= nil then 
        Home7World:getCurInstance():runAction(cc.Sequence:create(cc.DelayTime:create(0.016),cc.CallFunc:create(function()
            sa:SetCameraActive(false);
        end)));
    else 
        cb();
    end
end
 
-- 首次登陆 / 验证了用户名字 的逻辑。 
function XBLSayHello:playFirstHello()
    self.pXbl:setPositionX(1220);
    -- 创建一次对话
    self:playByJsonName("SayHello_newone.json",function(eventName)
         if eventName == "Complie" then 
            self:removeSayOrActControl();
            self:playNormalHelloEnd(); 
         elseif eventName == "Interupt"  then --正确
         end
    end);
end

-- 删除说话控制
function XBLSayHello:removeSayOrActControl()
	if self.sayOrActCon ~= nil then
		self.sayOrActCon:clearInfo();
		self.sayOrActCon = nil;
	end
   if self.waitToPoint ~= nil then
        self.waitToPoint:clear();
        self.waitToPoint = nil;
    end

    if self.waitToPointXbl ~= nil then
        self.waitToPointXbl:clear();
        self.waitToPointXbl = nil;
    end
end

-- 删除说话控制
function XBLSayHello:playNormalHelloEnd()
	Home7World:getCurInstance():setOnlyScrollEnable(true);
	self:doScuessEnd();
end

-- 打断事件开始，继承过来的
function XBLSayHello:interruptStart(isForce)
    g_tConfigTable.AnimationEngine:GetInstance():ProcessActionToEndByTag(self.actionTagOfAnimationEngine_);
    g_tConfigTable.AnimationEngine:GetInstance():StopPlayStory(self.actionTagOfAnimationEngine_);
	
	self:removeSayOrActControl();
	Home7World:getCurInstance():setOnlyScrollEnable(true);
	-- 创建一次对话
	self.sayOrActCon = requirePack("sceneScripts.sayact").new();
	self.sayOrActCon:clearInfo();
	self.sayOrActCon:addOneAct("XBL", THEME_FIX_ANIM("XBL6ZH"), 0, LOOP_YES);

	self.sayOrActCon:startAction( function()
		----self:resetCamera();  --2019.8.8 打断的时候，信箱会出现 镜头 突然晃动的情况 。 
		self:removeSayOrActControl();
		self:doInterruptEnd();
	end );
end

function XBLSayHello:resetCamera()
    local x, y = Home7World:getCurInstance():getHomeNpcShowByName("XBL"):getPosition();
    local fX = 0;
    local fY = 0;
    fX,fY = Home7World:getCurInstance():getSceneMiddlePos(fX, fY, true);
    local tMove1 = cc.MoveTo:create(0.3, cc.p(x,fY));
    local tScale1 = cc.ScaleTo:create(0.3, self.cameraScale);
    local spa = cc.Spawn:create(tMove1, tScale1);
    Home7World:getCurInstance():runCemeraByAction(spa);
end


------------------------------刷牙部分---------------------JulyTALK09
-- 进行刷牙事件SayHello_morning JulyTALK09
function XBLSayHello:playShuaYa()
    self:removeSayOrActControl();
    self:playByJsonName("SayHello_morning.json",function(eventName)
        if eventName == "Complie" then 
            Home7World:getCurInstance():setOnlyScrollEnable(true);
            self:doScuessEnd();
        end
    end);
end

-- 准备行动
function XBLSayHello:readyGo()
    local pHomeWorld = Home7World:getCurInstance()
    local pXbl = pHomeWorld:getXBL()
    pXbl:setScaleX(math.abs(pXbl:getScaleX()))

    self:removeSayOrActControl();
    self.sayOrActCon = requirePack("sceneScripts.sayact").new();
    self.sayOrActCon:clearInfo();
    self.sayOrActCon:addOneAct("XBL", THEME_FIX_ANIM("XBL6_OP02"), 1, LOOP_ORIG);
    self.sayOrActCon:addOneSay("XBL", "newxian502a", false, "你起得真早！")
    self.sayOrActCon:addOneSay("XBL", "newxian502b", false, "你想玩什么？")
    self.sayOrActCon:startAction( function() 
        self.sayOrActCon = requirePack("sceneScripts.sayact").new();
        self.sayOrActCon:clearInfo();
        self.sayOrActCon:addOneAct("XBL", THEME_FIX_ANIM("XBL6ZH"), 0, LOOP_YES);
        self.sayOrActCon:startAction( function()
            Home7World:getCurInstance():setOnlyScrollEnable(true);
            -- 风车模块结束
            self:doScuessEnd();
        end);
    end);
end
 
--actionType 对应的是 參考 XBLMiniTalk
function XBLSayHello:addOneActForItem(  stringAct  ) 
     if  stringAct ~= nil and  stringAct ~= 0 and  stringAct ~= ""  then
         local arr = string.split(stringAct,";")
         if arr == nil or #arr < 2 then
            writeToFile(" 282  XBLMiniTalk:addOneActForItem  error  ="..stringAct)
            return
         end
         self.sayOrActCon:addOneAct("XBL", THEME_FIX_ANIM(arr[1]), tonumber(arr[2]), LOOP_YES, true); --动作有衔接回来。
     end
end

function XBLSayHello:addOneSayForItem(  stringSay ) 
     if  stringSay ~= nil and  stringSay ~= 0 and  stringSay ~= ""  then
         self.sayOrActCon:addOneSay("XBL", stringSay, false, "");
     end
end

function XBLSayHello:playActionByLibraryNewEngine(actionItem)
    if actionItem~= nil then 
        if actionItem.newEngineScript ~= nil then 
            self:playByJsonName(actionItem.newEngineScript..".json",function(eventName)
                if eventName == "Complie" then 
                    Home7World:getCurInstance():setOnlyScrollEnable(true);
                    self:doScuessEnd();
                end
            end);
            return;
        end
    end
end
 
--播放配置的動作  參考 XBLMiniTalk:playAction( actionItem )   兼容 
function XBLSayHello:playActionByLibrary( actionItem ) 
    self:removeSayOrActControl();
    math.randomseed(tostring(os.time()):reverse():sub(1, 6)) 
    -- 创建一次对话
    self.sayOrActCon = requirePack("sceneScripts.sayact").new();
    self.sayOrActCon:clearInfo();
    local pHomeWorld = Home7World:getCurInstance()
    local pXbl = pHomeWorld:getXBL()
    pXbl:setZOrder(30)
    pXbl:stopAllActions()
    if  actionItem.actionType  == 1  then  --通用的最基本的方法之类的
        self:addOneActForItem(actionItem.oneAct1)
        self:addOneSayForItem(actionItem.oneSay1) 
        self:addOneActForItem(actionItem.oneAct2)
        self:addOneSayForItem(actionItem.oneSay2) 
        self:addOneActForItem(actionItem.oneAct3)
        self:addOneSayForItem(actionItem.oneSay3) 
        self:addOneActForItem(actionItem.oneAct4)
        self:addOneSayForItem(actionItem.oneSay4)  
    elseif actionItem.actionType  == 2  then --复杂的预留的方法
        if  self[actionItem.oneAct1] ~= nil then
            self[actionItem.oneAct1](self);--index
            return
        end    
    end 

    self.sayOrActCon:startAction(function() 
        self:removeSayOrActControl();
        self:doScuessEnd();
    end);  
end
 
function XBLSayHello:removeAction( )
     self:removeSayOrActControl();
end

return XBLSayHello;