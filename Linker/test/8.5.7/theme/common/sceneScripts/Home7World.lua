
-- -------------------- 每个玩法必须拥有的声明方法 ---------------------------------
local CustomEventTypes = requirePack("baseScripts.dataScripts.CustomEventType", false);
local mLuaFileExt1 = ".lua";
local mLuaFileExt2 = ".luac";
local InterruptTypes = requirePack("baseScripts.dataScripts.InterruptType", false);
local PetDataUtil = requirePack("sceneScripts.PetDataUtil");
local Schedule = cc.Director:getInstance():getScheduler();
-- 自定义事件名称
local HomeEventName = requirePack("sceneScripts.HomeEvent");

requirePack("baseScripts.homeUI.FrameWork.AnimationEngineLua.AnimationEngine");

local HomeWorldStatus = {
	-- 各自的脚本
	SCRIPT = 0,
	-- 二级菜单
	ERJIVIEW = 1,
};

local ErjiStatus = {
	-- 各自的脚本
	OPENED = 0,
	-- 二级菜单
	CLOSE = 1,
	-- 转变中
	CHANGE = 2,
	-- 打断关闭中
	INTRCLOSING = 3,
	-- 进入内容包关闭中
	INTOSOURCECLOSING = 4,
};

local Home7WorldLua = class("Home7WorldLua", function()
	local pNode = Home7World:create();  -- Home7World(C++)  <-CameraLayer <-Layer
	return pNode;
end);
-- 已经创建初步初始化完成
function Home7WorldLua:ctor()


	--cc.FileUtils:getInstance():addSearchPath("E:/TestSearchPath/tp1/");
	--cc.FileUtils:getInstance():addSearchPath("E:/TestSearchPath/tp2/");
	g_tConfigTable.AnimationEngine.TestFunc();
	print("Home7WorldLua xiewentao 12138");
	local listOfSearchPath = cc.FileUtils:getInstance():getSearchPaths();
	for i=1,#listOfSearchPath,1 do 
	    print("mark 42 1"..i);
        print("sdCardPath"..listOfSearchPath[i]);
	end
	--[[
	local sp = cc.Sprite:create("t.png");
	sp:setPosition(cc.p(150,150));
	self:addChild(sp,100000000000);
]]--
	-- 注册节点lua中的生命周期方法
	-- self:enableNodeEvents();-- 2019.7.23 Ting 删除。谁要开放 找我
    -- 初始化动作引擎
    g_tConfigTable.AnimationEngine.GetInstance();
	self:setLuaHandle(function(sType, pInfo, pInfo2) --点击的回调 会来到这里 
		-- 创建NPC
		if sType == "createNpc" then
			if pInfo == nil or pInfo.nNpcType == nil  then --预防野指针 。
			   return  nil 
			end
			local isLuaCreate = true;
			local sFilePath = THEME_FILE("sceneScripts/"..pInfo.sScript..mLuaFileExt2);
			if cc.FileUtils:getInstance():isFileExist(sFilePath) == false then
				local sFilePath = THEME_FILE("sceneScripts/"..pInfo.sScript..mLuaFileExt1);
				if cc.FileUtils:getInstance():isFileExist(sFilePath) == false then
					isLuaCreate = false;
				end
			end
			-- 需要lua创建的
			if isLuaCreate == true  then
				local npcLua = requirePack("sceneScripts."..pInfo.sScript);
				return npcLua.new(pInfo);
			end
			return Home7Npc:createNpc(pInfo);
		elseif sType == "onEnter" then
			self:onEnter();
			local pXblTmp = self:getXBL();
			if pXblTmp  then  -- 2019.7.23 夏令营回来会是null 的
				local fMidX = 0.0;
				local fMidY = 0.0;
				fMidX,fMidY = self:getSceneMiddlePos(fMidX, fMidY, true);
				self:runCemeraByAction(cc.MoveTo:create(0, cc.p(pXblTmp:getPositionX(), fMidY)));
            end
		elseif sType == "onExit" then
			self:onExit();
		-- 打断逻辑，UI主界面的一些强制打断逻辑
		elseif sType == "interrupt" then --此处打断的时候有众多坑 
			return self:doInterruptByType(pInfo, true);
		-- 打开二级结束
		elseif sType == "openErjiEnd" then
			self:openErjiEnd();
		-- 关闭二级结束
		elseif sType == "CloseErjiEnd" then
			self:CloseErjiEnd();
		-- 发呆的动作
		elseif sType == "readyact" then
			--print( " 83 83 83 这是一个极其恶心 的 无用的逻辑 " )
			self:playXBLReady();
		-- 学堂的开关
		elseif sType == "onXTAct" then
		elseif sType == "commonAction" then
		elseif sType == "resetXbltoMainView" then  --有时候从主界面回来 会出现 错。然后 此处被频繁调用。
			if self.resetXbltoMainView ~= nil then 
			   self:resetXbltoMainView(pInfo);
			end
		elseif  sType ==  "TouchBegan"  then
             self.screenMoveIng = false 
		elseif  sType =="TouchMoved" then
             self.screenMoveIng = true 
		elseif  sType == "TouchEnd" then
             if self.screenMoveIng then
                self:checkRunToCenter()
             end
             self.screenMoveIng = false
		end
	end);
	self.screenMoveIng = false 
	-- 二级菜单
	self.eMainStatus = HomeWorldStatus.SCRIPT;
	self.eErjiStatus = ErjiStatus.CLOSE;

	-- 是否不可打断
	self.isCanNotInterrupt = false; 
	-- 初始化打断类型
	self.nInterruptType = InterruptTypes.MAINPLAYER_INTERRUPT_UNKNOW;
	-- UI界面
	self.pErjiUI = HomeUILayer:getCurInstance();

	-- 是否被其他人占有打断
	self.isCurInterrupt = false;
	-- 打断结束的回调
	self.funcInterruptEnd = nil;
	-- 打断NPC的个数
	self.funcInterruptCnt = 0;
	-- 当前需要打断的NPC列表
	self.tNeedInterruptList = {};
	-- 打断UI的回调
	self.funcInterruptUi = nil;
	-- 从二级里面回来
	self.fPushCemeraHeight = 0.0;

	-- 休息或者睡觉
	self.sIntoRestType = nil;
	self.scheduleEnty  = nil;

	--忽略ignoreResetXbltoMainView的效果  true 表示 忽略 。 false 表示 不能回略 
	self.ignoreResetXbltoMainView = false 

	-- 这里要用 Home7World:getCurInstance() ，不然有些机子会挂掉。
	-- 捕抓休息事件
	Home7World:getCurInstance():addReceiveWorldEventHandleLua("", HomeEventName.EVENT_HOME_REST, function(pEvent)
		if self.sIntoRestType ~= HomeEventName.EVENT_HOME_SLEEP then
		   self.sIntoRestType = HomeEventName.EVENT_HOME_REST;
		end
		-- 去掉旧的计时器
		if self.scheduleEnty ~= nil then
			Schedule:unscheduleScriptEntry(self.scheduleEnty);
			self.scheduleEnty = nil;
		end
		-- 跳过1帧，避免出错
		self.scheduleEnty = Schedule:scheduleScriptFunc(function()
			 if self.intoRestJudge then 
				self:intoRestJudge() 
			 else 
				if self.scheduleEnty ~= nil then
				   Schedule:unscheduleScriptEntry(self.scheduleEnty);
				   self.scheduleEnty = nil;
				end
			 end
		end, 1.0, false)
	end, self)

	-- 捕抓睡觉事件
	Home7World:getCurInstance():addReceiveWorldEventHandleLua("", HomeEventName.EVENT_HOME_SLEEP, function(pEvent)
		self.sIntoRestType = HomeEventName.EVENT_HOME_SLEEP;
		-- 去掉旧的计时器
		if self.scheduleEnty ~= nil then
			Schedule:unscheduleScriptEntry(self.scheduleEnty);
			self.scheduleEnty = nil;
		end
		-- 跳过1帧，避免出错
		self.scheduleEnty = Schedule:scheduleScriptFunc(function()
			 if self.intoRestJudge then 
				self:intoRestJudge();
			 else 
				if self.scheduleEnty ~= nil then
				   Schedule:unscheduleScriptEntry(self.scheduleEnty);
				   self.scheduleEnty = nil;
				end
			 end
		end, 1.0, false)
	end, self)

	self:initTestTool()
end

--测试环境专用工具
function Home7WorldLua:initTestTool()
	if not IS_TEST then
		return
	end
	self.testSchedule = Schedule:scheduleScriptFunc(function()
		Schedule:unscheduleScriptEntry(self.testSchedule)
		self.testSchedule = nil
  		local testContainer = cc.Node:create()
		testContainer:setPosition(cc.p(10, 140))
		testContainer:setVisible(false)
		self:getParent():addChild(testContainer)
		self:getParent().testContainer=testContainer
		local bgSprite = ccui.Scale9Sprite:create(THEME_IMG("new2ji/msg_bg.png"),cc.rect(40,20,10,10), cc.rect(0,0,10,10))
		local serverTimeEditBox = ccui.EditBox:create(cc.size(200,24), bgSprite)
	    serverTimeEditBox:setFontSize(24)
	    serverTimeEditBox:setPlaceHolder("1炸弹瓜 2放屁花 3隐形珊瑚")
	    serverTimeEditBox:setAnchorPoint(cc.p(0,0.5))
	    serverTimeEditBox:setPosition(cc.p(0, 60))
	    testContainer:addChild(serverTimeEditBox)
	    local function onOkBtnClick(sender)
	  		local txt = serverTimeEditBox:getText()
	        --修改获取服务器时间的全局函数
			-- UInfoUtil:getInstance().getServerTime = function()
			-- 	local txt = serverTimeEditBox:getText()
			-- 	local t = string.split(txt,"-")
			-- 	local time = os.time({["year"]=tonumber(t[1]), ["month"]=tonumber(t[2]), ["day"]=tonumber(t[3])})
			-- 	return time
			-- end
			--获得新的收集物
			CustomEventDispatcher:getInstance():msgBroadcastLua(CustomEventTypes.CE_COLLECT_SHOW_NEW_GOOD, txt, true)
		end
	    local okBtn = ccui.Button:create()
	    okBtn:setTouchEnabled(true)
	    okBtn:setPressedActionEnabled(true)
	    okBtn:setAnchorPoint(cc.p(0,0.5))
	    okBtn:setPosition(cc.p(0, 20))
	    okBtn:addClickEventListener(onOkBtnClick)
	    okBtn:setTitleText("OK")
	    okBtn:setTitleFontSize(24)
	    okBtn:setTitleColor(cc.c3b(255,0,0))
	    testContainer:addChild(okBtn)

		local function onTestBtnClick(sender)
			testContainer:setVisible(not testContainer:isVisible())
		end
		local testBtn = ccui.Button:create()
	    testBtn:setTouchEnabled(true)
	    testBtn:setPressedActionEnabled(true)
	    testBtn:setAnchorPoint(cc.p(0,0.5))
	    testBtn:setPosition(cc.p(10, 120))
	    testBtn:addClickEventListener(onTestBtnClick)
	    testBtn:setTitleText("TEST工具")
	    testBtn:setTitleFontSize(26)
	    testBtn:setTitleColor(cc.c3b(255,0,0))
	    self:getParent():addChild(testBtn)
	end, 0.1, false);
end


local function Log(...)
	--CC_GameLog("Home7WorldLua----", ...)
end
--功能：释放神奇扫描仪中创建的东西
function Home7WorldLua:releaseNewGoodsEffect()
	--1.清除内容
	Home7World:getCurInstance():stopActionByTag(777)
	AsyncLoadRes:shareMgr():cancelOneDirAsyncLoad("sqsmy_anim_mov", true) --清除缓存动画
	cc.Director:getInstance():getEventDispatcher():removeEventListener(self.touch_listenner)--释放监听器
	self.pSpeak:release()
	--2.小伴龙恢复闲置
	local pXbl = self:getHomeNpcByName(HomeEventName.XBL)
    pXbl:setBeHoldByNpc("") 
    if pXbl ~= nil then
    	pXbl:playXianZhi() 
    end
end
--功能：神奇扫描仪中调用到的方法
function Home7WorldLua:showNewGoodsEffect(dataInfo,resource)
	--dataInfo = {}
	--dataInfo = {{goodid = 1000},{goodid = 1001}}
	if #dataInfo <= 0 or PetDataUtil.mianmianNode:isVisible() == false then
		--2.小伴龙恢复闲置
		local pXbl = self:getHomeNpcByName(HomeEventName.XBL)
	    pXbl:setBeHoldByNpc("") 
	    if pXbl ~= nil then
	    	pXbl:playXianZhi() 
	    end
		return
	end
	--1.加载资源
	AsyncLoadRes:shareMgr():addOneDirAsyncLoadLua(resource.."animation/hold/", "", "sqsmy_anim_mov", function(key)
		--2.1数据处理
		local ResKeyMap = {[1000] = "sqsmy_sxmm_ji_mov", [1001] = "sqsmy_sxmm_niu_mov", [1002] = "sqsmy_sxmm_she_mov", [1003] = "sqsmy_sxmm_shu_mov", [1004] = "sqsmy_sxmm_tu_mov" }
		local scene = cc.Director:getInstance():getRunningScene()
		local winsize = cc.Director:getInstance():getWinSize()
		local size = scene:getContentSize()
		Log("now scene size:",size.width, size.height)
		--2.2播放动画
		if #dataInfo > 0 then
			--3.1屏蔽触摸事件层
			self:createBlockLayer()
			--3.2播放
			local index = 1
			local delay_time = 2 / #dataInfo--以2s为基本停顿时长，均分给所有的果实避免多个果实时等待太久
			Log("delay_time",delay_time)
			--此处为复制过来的代码（参见HomeUIItemLua:creatRightMenuLayer），从那里x复制的坐标设定方式
			local excuteY = 14 
			local pos = cc.p( cc.Director:getInstance():getWinSize().width - 100 - 5 , cc.Director:getInstance():getWinSize().height-300 -excuteY )
			if  Utils:GetInstance():getIsIphoneX() == true then
				excuteY = 44
				pos = cc.p(cc.Director:getInstance():getWinSize().width-100,cc.Director:getInstance():getWinSize().height-300 -excuteY  )
			end
			pos.x = pos.x + 50--
			pos.y = pos.y + 149
			--复制结束
			local function delayCall()
				local armRes = ResKeyMap[tonumber(dataInfo[index].goodid)]
				Log("armRes:",armRes)
				local mianmianfruit = TouchArmature:create(armRes, TOUCHARMATURE_NORMAL)
				self.mianmianfruit = mianmianfruit
				scene:addChild(self.mianmianfruit,100000)
				mianmianfruit:setPosition(cc.p(winsize.width/2,winsize.height/2))
				mianmianfruit:playByIndex(0,LOOP_NO)--阶段1
				mianmianfruit:setLuaCallBack(function ( eType, pTouchArm, sEvent )
					if eType == TouchArmLuaStatus_AnimEnd then--阶段2
						mianmianfruit:playByIndex(1,LOOP_YES)
						mianmianfruit:runAction(cc.Sequence:create(
							cc.DelayTime:create(delay_time),
							cc.MoveTo:create(1.0,pos),
							cc.CallFunc:create(function()
								mianmianfruit:setLuaCallBack(function ( eType, pTouchArm, sEvent )
									mianmianfruit:playByIndex(2,LOOP_NO)
									mianmianfruit:setLuaCallBack(function ( eType, pTouchArm, sEvent )
										if eType == TouchArmLuaStatus_AnimEnd then--结束统计
											index = index + 1
											if index <=  #dataInfo then
												local actionFunc = cc.CallFunc:create(self.delayCall)
												actionFunc:setTag(7777)
												Home7World:getCurInstance():runAction(actionFunc) 
											else
												--4.清除
												self.pSpeak = Speak:create()
												self.pSpeak:retain()
												self.pSpeak:sayCfgLua(self:getXBL(), resource.."audio/sqsmy040/", "", function(pArm)
													self:releaseNewGoodsEffect()
												end)
											end -- end if
										end -- end if 结束统计
									end)
								end)
							end)))--一边移动一边播其他动作
					end -- end if 阶段2
				end)
			end    
			self.delayCall = delayCall
			local actionFunc = cc.CallFunc:create(delayCall)
			actionFunc:setTag(7777)
			self:runAction(actionFunc)
		end
	end)
end
--功能：创建一个监听器来屏蔽掉主场景的触摸操作行为
function Home7WorldLua:createBlockLayer()
	--1.创建一个事件监听器类型为 OneByOne 的单点触摸
	self.touch_listenner = cc.EventListenerTouchOneByOne:create()
	self.touch_listenner:setSwallowTouches(true)
	--【onTouchBegan】
	self.touch_listenner:registerScriptHandler(function(touch,event)
		return true--确保全局屏蔽
		end, cc.Handler.EVENT_TOUCH_BEGAN )
	--【onTouchMoved】
	--self.touch_listenner:registerScriptHandler(self.onTouchBegan, cc.Handler.EVENT_TOUCH_BEGAN )
	--【onTouchEnded】
	-- self.touch_listenner:registerScriptHandler(function(touch, event)
	-- 	CC_GameLog("【onTouchEnded】-------------------")
	-- 	end, cc.Handler.EVENT_TOUCH_ENDED )
	--2添加监听器
	local eventDispatcher = cc.Director:getInstance():getEventDispatcher()
	eventDispatcher:addEventListenerWithFixedPriority(self.touch_listenner, -0xffff)--提高监听器的相应层级

	-- local function testFunc()
	-- end
	-- performWithDelay(self,testFunc, 0.5)
end

-- 创建主场景之后的处理
-- nFromModule :  从什么模块出来
-- nToModule: 要去往哪个模块
function Home7WorldLua:createAction(nFromModule, nToModule)
	--print("new into Home7WorldLua::".. nFromModule .."       :".. nToModule);
end

-- 主场景的入场的处理
function Home7WorldLua:onEnter()
	-- UI界面
	self.pErjiUI = HomeUILayer:getCurInstance();

	self.uid = UInfoUtil:getInstance():getCurUidStr() 
end
-- 主场景的离场的处理
function Home7WorldLua:onExit()
	-- 撤销动画引擎
	g_tConfigTable.AnimationEngine.GetInstance():Dispose();

	-- 去掉旧的计时器
	if self.scheduleEnty ~= nil then
		Schedule:unscheduleScriptEntry(self.scheduleEnty);
		self.scheduleEnty = nil;
	end
	local pXbl = self:getHomeNpcByName(HomeEventName.XBL);
	if pXbl ~= nil and pXbl.removeCurAction ~= nil then
	   pXbl:removeCurAction()
	end
end


-- 主场景的离场的处理
function Home7WorldLua:intoRestJudge()
	--刪除二級购买或者H5父母验证
	local scene = cc.Director:getInstance():getRunningScene()
	scene:removeChildByTag(20190509)

	local isNeedEndSchedule = false;
	-- 休息
	if self.sIntoRestType == HomeEventName.EVENT_HOME_REST then
		isNeedEndSchedule = self:doInterruptByType(InterruptTypes.MAINPLAYER_INTERRUPT_MENU_XIUXI, true);
	-- 睡觉
	elseif self.sIntoRestType == HomeEventName.EVENT_HOME_SLEEP then
		isNeedEndSchedule = self:doInterruptByType(InterruptTypes.MAINPLAYER_INTERRUPT_MENU_SHUIJIAO, true);
	else
		isNeedEndSchedule = true;
	end
	-- 是否要删除计时器
	if isNeedEndSchedule == true then
		-- 去掉旧的计时器
		if self.scheduleEnty ~= nil then
			Schedule:unscheduleScriptEntry(self.scheduleEnty);
			self.scheduleEnty = nil;
		end
	end
end

-- 强制打断
function Home7WorldLua:startStopCurAllActionLua(isAddXbl, funcBack,  isUiSelfCon)
	if self.isCurInterrupt == true or funcBack == nil then
		return false;
	end
	callStaticManagerFunction("endPlayToyBox") 
	-- 获取要打断的NPC列表
	local sNpcList = self:getNeedInterruptNpcListLua(isAddXbl);
	self.tNeedInterruptList = string.split(sNpcList, "|");
	if self.tNeedInterruptList == nil then
	   self.tNeedInterruptList = {};
	end
	--print("260 Home7WorldLua:startStopCurAllActionLua  检查要打断的NPC列表 ,:", sNpcList);
	local nCnt = #self.tNeedInterruptList;
	if nCnt <= 0 or sNpcList == "" then
		--print("263 Home7WorldLua:startStopCurAllActionLua  打断NPC ");
		if funcBack ~= nil then
		--	print("265 Home7WorldLua:startStopCurAllActionLua 打断NPC 回调 ");
			funcBack();
		end
		return true;
	end
	self.funcInterruptCnt = nCnt;
	self.funcInterruptEnd = funcBack;
	--print("272 Home7WorldLua:startStopCurAllActionLua  逻辑判断 ",isAddXbl, funcBack,  isUiSelfCon,self.eErjiStatus)
	-- 打断处理
	if (isUiSelfCon == nil or isUiSelfCon == false) and self.eErjiStatus ~= ErjiStatus.CLOSE and self.funcInterruptUi == nil then
	--	print(" 275  Home7WorldLua:startStopCurAllActionLua  逻辑判断, 创建  self.funcInterruptUi ");
		self.funcInterruptUi = function()
			self.funcInterruptUi = nil;
			self:judgeAllInterruptIsAllEnd();
		end
		-- 二级是打开的时候强制关闭
		if self.eErjiStatus == ErjiStatus.OPENED then
			--print(" 275  Home7WorldLua:startStopCurAllActionLua  逻辑判断, 二级打开的时候 强制 关闭 ");
			self:doInterruptByType(InterruptTypes.MAINPLAYER_INTERRUPT_MENU_NPCXML);
		end
	else
		--print("=========================Home7WorldLua pps 0")
		self.funcInterruptUi = nil;
	end

	-- 开始打断NPC
	self:startInterruptNpcLua(sNpcList,  true, function(sNpcName)
		self.funcInterruptCnt = self.funcInterruptCnt-1;
		--print("Home7WorldLua:startStopCurAllActionLua:22222:::", self.funcInterruptCnt, sNpcName);
		-- 如果所有NPC都打断结束，则提示打断结束
		if self.funcInterruptCnt <= 0 then
			self.funcInterruptCnt = 0;
			--print("Home7WorldLua:startStopCurAllActionLua:333333");
			self:judgeAllInterruptIsAllEnd();
		end
	end);
	

	return true;
end

-- 所有打断是否已经全部结束
function Home7WorldLua:judgeAllInterruptIsAllEnd()
	if self.funcInterruptUi ~= nil then
		return;
	end
	if self.funcInterruptEnd ~= nil then
		local funcTmp = self.funcInterruptEnd;
		self.funcInterruptEnd = nil;
		funcTmp();
	end
end

-- UI主界面的打断逻辑
function Home7WorldLua:doInterruptByType(nType, isNeedInterruptOther) 
	if self.isCanNotInterrupt == true then
	   return false;
	end
	
	-- 所有按钮不可点
	self:setAllTouchEnableLua(false, true);  
	self.nInterruptType = nType;
	if nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_SETING 
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_GUSHI
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_GEWU
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_XUETANG
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_BAOXIANG
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_THEME
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_ERJICLOSE
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_ERJIPLAYER
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_NPCXML
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_XIUXI 
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_SHUIJIAO
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_TOYBOXOPEN 
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_ALBUM
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_COLLECT
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_CLASSICAL
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_THEME_LUA
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_AD
		or nType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_XIALINGYING then
		-- 根据打断情况进行处理
		if isNeedInterruptOther == true then 
			self:startStopCurAllActionLua(true, function()
				self:stopXBLByUIEnd();
			end, true);
		else
			self:stopXBLByUIEnd();
		end
	else
		 self:setAllTouchEnableLua(true, true);
	end

	return true 
end

function Home7WorldLua:setAllTouchEnableLua(isTouch, isChangeSetting)
	-- 所有按钮不可点
	self:setAllTouchEnable(isTouch, isChangeSetting);
	self.isCanNotInterrupt = not isTouch;
	if isTouch == false then
		self:stopCemeraAction();
	end
end

-- 强制打断小伴龙
function Home7WorldLua:stopXBLByUIEnd()
	
	if self.eMainStatus == HomeWorldStatus.SCRIPT then
		-- 关闭状态才可以打开
		if self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_SETING 
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_GUSHI
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_GEWU
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_XUETANG
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_BAOXIANG
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_THEME
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_TOYBOXOPEN 
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_ALBUM 
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_COLLECT 
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_CLASSICAL
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_THEME_LUA
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_AD
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_XIALINGYING
		then
			
			self.funcInterruptUi = nil;
			-- 根据不同平台做不同的处理
			local targetPlatform = cc.Application:getInstance():getTargetPlatform()
			-- 点击的是设置
			if self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_SETING then
			   self:setAllTouchEnableLua(true, true); --坑
			else
				self.eErjiStatus = ErjiStatus.CHANGE;
			end
		    local nType = HomeUILayer:getCurInstance():getNormalUIItem():getInterruptTypeTOMenuType(self.nInterruptType)
		    HomeUILayer:getCurInstance():getNormalUIItem():openMenuMsg(nType)
			-- 主题打开时，小伴龙也居中
		    if self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_SETING then --主题
				self.nInterruptType = InterruptTypes.MAINPLAYER_INTERRUPT_UNKNOW;
			end
		-- 被其他打断的情况
		elseif self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_NPCXML then
			-- 二级关闭的时候才会处理
			if self.eErjiStatus == ErjiStatus.CLOSE then
				if self.funcInterruptUi ~= nil then
					self.funcInterruptUi();
				end
			end-- 全部可以点击
			self:setAllTouchEnableLua(true, true);
		-- 睡觉打断
		elseif self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_SHUIJIAO then  
			local pXbl = self:getHomeNpcByName(HomeEventName.XBL);
			if pXbl ~= nil and pXbl.setXblCannotDoOther ~= nil  then
			   pXbl:setXblCannotDoOther();
			end
			self:setAllTouchEnableLua(false, true);
			self:setUIvisible(false, true);
			self:gotoSourceStart(MOUDULE_SHUIJIAO,"4",STORY4V_TYPE_UNKNOW);
			self:gotoSourceSay();
		-- 休息打断
		elseif self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_XIUXI then
			local pXbl = self:getHomeNpcByName(HomeEventName.XBL);
			if pXbl ~= nil and pXbl.setXblCannotDoOther ~= nil  then
				pXbl:setXblCannotDoOther();
			end
			self:setAllTouchEnableLua(false, true);
			self:setUIvisible(false, true);
			self:gotoSourceStart(MOUDULE_XIUXI,"",STORY4V_TYPE_UNKNOW);
			self:gotoSourceSay();
			 --镜头移动到小伴龙身上 。
			local fX = 0;
			local fY = 0;  
			fX,fY =  Home7World:getCurInstance():getSceneMiddlePos(fX, fY, true);
			local xbl = Home7World:getCurInstance():getXBL(); 
			--xbl:stopAllActions() --影响打卡 。所以不要了
			--xbl:setScale(0.65)--默认大小是0.655
			local xblBegX = xbl:getPositionX();
			local tMove1 = cc.MoveTo:create(0.23, cc.p(xblBegX,fY)) --时间参数有毒不要改
			local tScale1 = cc.ScaleTo:create(0.23, 1.0) --时间参数有毒不要改
			local spa1 = cc.Spawn:create(tMove1, tScale1)
			Home7World:getCurInstance():runCemeraByAction(spa1)
		-- 主动关闭
		else
			-- 二级还开着
			if HomeUILayer:getCurInstance():isErjiMenuShow() == true then
				-- 主动关闭
				if self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_ERJICLOSE then
					self.funcInterruptUi = nil;
					-- 打开状态才能关闭二级界面
					if self.eErjiStatus == ErjiStatus.OPENED then
						self.eErjiStatus = ErjiStatus.CHANGE;
						self.pErjiUI:closeSubmenuList();--通知关闭全部的二级 
					end
				-- 被动关闭
				else
					-- 打开状态才能关闭二级界面
					if self.eErjiStatus == ErjiStatus.OPENED then
						if self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_ERJIPLAYER then
							self.funcInterruptUi = nil;
							self.eErjiStatus = ErjiStatus.INTOSOURCECLOSING;
						else
							if self.nInterruptType ~= InterruptTypes.MAINPLAYER_INTERRUPT_MENU_NPCXML then
								self.funcInterruptUi = nil;
							end
							self.eErjiStatus = ErjiStatus.INTRCLOSING;
						end
						self.pErjiUI:closeSubmenuList(); --通知关闭全部的二级 
					end
				end
			end
		end
	elseif self.eMainStatus == HomeWorldStatus.ERJIVIEW then
		-- 主动关闭
		if self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_ERJICLOSE then
			self.funcInterruptUi = nil;
			-- 打开状态才能关闭二级界面
			if self.eErjiStatus == ErjiStatus.OPENED then
				self.eErjiStatus = ErjiStatus.CHANGE;
				self.pErjiUI:closeSubmenuList(); --通知关闭全部的二级 
			end
		-- 被动关闭
		else
			-- 打开状态才能关闭二级界面
			if self.eErjiStatus == ErjiStatus.OPENED then
				if self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_ERJIPLAYER then
					self.funcInterruptUi = nil;
					self.eErjiStatus = ErjiStatus.INTOSOURCECLOSING;
				else
					if self.nInterruptType ~= InterruptTypes.MAINPLAYER_INTERRUPT_MENU_NPCXML then
						self.funcInterruptUi = nil;
					end
					self.eErjiStatus = ErjiStatus.INTRCLOSING;
				end
				self.pErjiUI:closeSubmenuList(); --通知关闭全部的二级 
			end
		end
	end
end

-- 打开二级结束
function Home7WorldLua:openErjiEnd()
	self.eErjiStatus = ErjiStatus.OPENED;
	self.eMainStatus = HomeWorldStatus.ERJIVIEW;
	-- 检测可以休息
	self:checkEventListenEnable(true);
	-- 全部可以点击
	self:setAllTouchEnableLua(true, true);
	self:setOnlyScrollEnable(false);
   
	if self.funcInterruptUi ~= nil then
	   self:doInterruptByType(InterruptTypes.MAINPLAYER_INTERRUPT_MENU_NPCXML);
	end
    --此处有一个 巨坑的内容。每次打开了UI 都会来到这里
end

-- 关闭二级结束
function Home7WorldLua:CloseErjiEnd()
	self.eMainStatus = HomeWorldStatus.SCRIPT;
	-- 全部可以点击
	self:setAllTouchEnableLua(true, true);
	self:setOnlyScrollEnable(true);
	-- 进入内容包的打断
	if self.eErjiStatus == ErjiStatus.INTOSOURCECLOSING then
		-- 重置
		self.eErjiStatus = ErjiStatus.CLOSE;

		-- 要去其他内容，则不让小伴龙干其他事情了
		local pXbl = self:getHomeNpcByName(HomeEventName.XBL);
		if pXbl ~= nil and pXbl.setXblCannotDoOther ~= nil  then
		   pXbl:setXblCannotDoOther();
		end
		-- 开始进入资源内容
		self:setAllTouchEnableLua(false, true);
		self:setUIvisible(false, true);
		self:gotoSourceSay();
	-- 其他打断关闭
	elseif self.eErjiStatus == ErjiStatus.INTRCLOSING then
		self.eErjiStatus = ErjiStatus.CLOSE;
		-- 进入打断之后的处理
		self:stopXBLByUIEnd();
		-- 重置打断记录
		self.nInterruptType = InterruptTypes.MAINPLAYER_INTERRUPT_UNKNOW;
	-- 正常关闭
	else
		-- 重置打断记录
		self.nInterruptType = InterruptTypes.MAINPLAYER_INTERRUPT_UNKNOW;
		self.eErjiStatus = ErjiStatus.CLOSE;
		self:playXblXianzhi();
	end
end
 
-- 打断的事件是否是底部二级打断
function Home7WorldLua:isBottomErjiInterrupt()
	-- 关闭状态才可以打开
	if  self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_GUSHI
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_GEWU
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_XUETANG
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_BAOXIANG
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_THEME
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_TOYBOXOPEN  
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_ALBUM  
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_COLLECT  
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_CLASSICAL
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_THEME_LUA
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_SETING 
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_AD then
		return true;
	end
	return false;
end

-- 打断的事件是否是底部二级打断
function Home7WorldLua:isCanNotStopToXianZhi()
	-- 关闭状态才可以打开
	if self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_SETING 
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_GUSHI
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_GEWU
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_XUETANG
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_BAOXIANG
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_THEME
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_TOYBOXOPEN  
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_ALBUM  
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_COLLECT  
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_CLASSICAL
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_THEME_LUA
		or self.nInterruptType == InterruptTypes.MAINPLAYER_INTERRUPT_MENU_AD then
		return true;
	end
	return false;
end
 --true 表示 忽略 。false 表示不能忽略
function Home7WorldLua:setIgnoreResetXbltoMainView ( needReflash )
    self.ignoreResetXbltoMainView = needReflash 
end

-- 返回播放闲置  重置到 主界面 
function Home7WorldLua:resetXbltoMainView(nValue)
    if self.ignoreResetXbltoMainView  then
       return
    end
	local pXbl = self:getHomeNpcByName(HomeEventName.XBL);
	if self.eMainStatus == HomeWorldStatus.SCRIPT and pXbl:getBeHoldNpc() == "" then
	   self:playXblXianzhi();
	end
	local temp = UInfoUtil:getInstance():getCurUidStr() 
	if self.uid ~=  temp then  --如果UID 被改变就发送广播出去 。
		CustomEventDispatcher:getInstance():msgBroadcastLua(CustomEventTypes.CE_USERIINFO_CHANGE_UID)
	else
		CustomEventDispatcher:getInstance():msgBroadcastLua(CustomEventTypes.CE_USERIINFO_NOT_CHANGE_UID) --从用户中心回来
	end
	 --ios 应该有用 。 android的不知道 后遗症
	CustomEventDispatcher:getInstance():msgBroadcast(CE_YIJI_TIPS_UPDATE)
	self.uid =  temp
end

-- 返回播放闲置
function Home7WorldLua:playXblXianzhi()
	self:setOnlyScrollEnable(true);
	self.eErjiStatus = ErjiStatus.CLOSE;
	self.eMainStatus = HomeWorldStatus.SCRIPT;
	local pXbl = self:getHomeNpcByName(HomeEventName.XBL);
	if pXbl ~= nil and pXbl.playXianZhi ~= nil  then
	   pXbl:playXianZhi() 
	end
end

-- 关闭二级结束
function Home7WorldLua:playXBLReady()
	local pXbl = self:getHomeNpcShowByName(HomeEventName.XBL);
	if pXbl ~= nil then
	   pXbl:changeArmature(THEME_FIX_ANIM("XBL6ZH"));
	   pXbl:playByIndex(0,LOOP_YES);
	end
end

-- 设置是否玩具箱要打断
function Home7WorldLua:setToyInterrupt(isToyInterrupt)
	self.isToyInterrupt = isToyInterrupt;
end 

function Home7WorldLua:getIsToyInterrupt()
	if self.isToyInterrupt == nil then
		return false;
	end

	return self.isToyInterrupt;
end

--重置鏡頭的Y坐標。 默認時間是 0.5 秒 
function Home7WorldLua:resetCameraYPosition( resetTime_temp )
    local pXblTmp = Home7World:getCurInstance():getXBL();
    if pXblTmp == nil  then
       return 
    end
    local resetTime = resetTime_temp
    if resetTime == nil then
       resetTime = 0.5
    end
    local fMidX = 0.0;
    local fMidY = 0.0;
    fMidX,fMidY =  Home7World:getCurInstance():getSceneMiddlePos(fMidX, fMidY, true);
    Home7World:getCurInstance():runCemeraByAction(cc.MoveBy:create(resetTime, cc.p( 0 , fMidY - Home7World:getCurInstance():getCameraNode():getPositionY())));

end

--設置鏡頭的坐標 到  targetPoxitionX .同時 Y坐標也歸爲
function Home7WorldLua:setCameraXPosition( tregetPositionX , resetTime_temp )  
      if tregetPositionX  == nil then
         return 
      end 
      local resetTime = resetTime_temp
      if resetTime == nil then
         resetTime = 0.5
      end
	  local fX = 0.0;
      local fY = 0.0;
      fX,fY = Home7World:getCurInstance():getSceneMiddlePos(fX, fY , true )
      local tMove2 = cc.MoveTo:create(resetTime, cc.p(tregetPositionX ,fY));
      local tScale1 = cc.ScaleTo:create(resetTime, 1);
      -- local delayRun =  cc.Sequence:create(cc.DelayTime:create(resetTime), 
      --     cc.CallFunc:create(function() 
      --     print( " 838  結束  結束  " )
      -- end))
      local spawn = cc.Spawn:create(tScale1, tMove2);
      Home7World:getCurInstance():runCemeraByAction(spawn);
end


function Home7WorldLua:checkRunToCenter()  
         if  self.screenMoveIng then      	
		     local pXbl = self:getHomeNpcByName(HomeEventName.XBL)
		     if pXbl ~= nil then
		    	pXbl:doCameraMoveToXblByTimeCheck() 
		     end
         end
end


function Home7WorldLua:getNInterruptType ()
	return self.nInterruptType
end


return Home7WorldLua;