-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成
local Grid = requirePack("scripts.FrameWork.AI.GridsAndAStar.Grid")
local GridManager = { }

g_tConfigTable.CREATE_NEW(GridManager);

-----------------私有方法-------------------
function GridManager:ctor()

end
function GridManager:init(rect, size)
    self.listOfGrids_ = { };
    local gridWidth;
    local gridHeight;
    gridWidth = rect.width / size.width;
    gridHeight = rect.height / size.height;
    for i = 1, size.height , 1 do
        for z = 1, size.width, 1 do
            local g = Grid.Create(
            cc.p(z, i),
            cc.pAdd(
            cc.p((z - 1) * gridWidth + gridWidth / 2, (i - 1) * gridHeight + gridHeight / 2),
            cc.p(rect.x, rect.y)
            ),
            0,
            cc.size(gridWidth, gridHeight)
            );
            table.insert(self.listOfGrids_, g);
        end
    end
    self.size_ = size;
    self.orietionPos_ = { cc.p(1, 0), cc.p(-1, 0), cc.p(0, - 1), cc.p(0, 1) };
end

function GridManager:refreshDebugPaint()
    self.debugDrawNode_:clear();
    if self.listOfGrids_ ~= nil and self.debugDrawNode_ ~= nil and self.debugIsOn_ then
        for k, v in ipairs(self.listOfGrids_) do
            local g = v;
            local posInScene = g:GetPosInScene();
            local gridSize = g:GetSize();

            self.debugDrawNode_:drawRect(
            cc.p(posInScene.x - gridSize.width / 2,
            posInScene.y - gridSize.height / 2),
            cc.p(posInScene.x + gridSize.width / 2,
            posInScene.y + gridSize.height / 2),
            cc.c4f(1, 1, 0, 1)
            );
        end
    end
    if self.listOfPath_ ~= nil then
        for k, v in ipairs(self.listOfPath_) do
            local index = v.x + (v.y-1) * self.size_.width;

            local g = self.listOfGrids_[index];
            local posInScene = g:GetPosInScene();
            local gridSize = g:GetSize();
            self.debugDrawNode_:drawRect(
            cc.p(posInScene.x - gridSize.width / 2,
            posInScene.y - gridSize.height / 2),
            cc.p(posInScene.x + gridSize.width / 2,
            posInScene.y + gridSize.height / 2),
            cc.c4f(1, 0, 1, 1)
            );
        end
    end
end

function GridManager:setPath(path)
    self.listOfPath_ = path;
end

function GridManager:findLowerValueInList(list, key)
    local element = nil;
    local maxValue = 10000;
    local index = 0;
    for k, v in ipairs(list) do
        if v[key] < maxValue then
            element = v;
            maxValue = v[key];
            index = k;
        end
    end
    return element, index;
end

function GridManager:searchPathBack(posEnd)
    local listOfPath = { };
    local nowIndexPos = posEnd;
    while nowIndexPos ~= nil do
        table.insert(listOfPath,0 ,nowIndexPos);
        nowIndexPos = nowIndexPos.parent_;
    end
    return listOfPath;
end

function GridManager:getGridByPos(pos)
    if pos.x < 1 or pos.y < 1 or pos.x > self.size_.width or pos.y > self.size_.height then
        return nil;
    end
    return self.listOfGrids_[pos.x +( pos.y-1 )* self.size_.width];
end

function GridManager:findNearCouldPassGridsList(grid)
    local posOrignal = grid:GetPos();
    local lists = { };
    for k, v in ipairs(self.orietionPos_) do
        local aimPos = cc.pAdd(posOrignal, v);
        local dealGrid = self:getGridByPos(aimPos);
        if dealGrid ~= nil  then
            if dealGrid:GetCouldPass() then 
                table.insert(lists, dealGrid);
            end
        end
    end
    return lists;
end

function GridManager:isContain(list,value)
    for k,v in ipairs(list) do
        if v == value then 
            return true;
        end 
    end
    return false;
end

-----------------对外接口-------------------
--[[
    工厂方法
    参数:
    rectInScene:网格对应场景中的矩形
    size:网格大小，网格有多上行多少列
    返回
    GridManager
]]--
function GridManager.Create(rectInScene, size)
    local g = GridManager.new();
    g:init(rectInScene, size);
    return g;
end

--[[
    设置格子穿越代价值
    参数:
    pos:格子的索引坐标
    passValue:格子的穿越代价 passValue越大越不容易穿越
]]--
function GridManager:SetGridPassValue(pos, passValue)
    local grid =self:getGridByPos(pos);
    if grid ~= nil then 
        grid:SetPassValue(passValue);
    end
end

--[[
    设置格子是否可穿越
    参数:
    pos:格子的索引坐标
    couldPass:bool 是否可以穿越
]]--
function GridManager:SetGridCouldPass(pos,couldPass)
    local grid = self:getGridByPos(pos);
    if grid ~= nil then 
        grid:SetCouldPass(couldPass);
    end
end


--[[
    计算从格子A到格子B的最短路径
    参数:
    posA:格子a
    posB:格子b
    返回
    table:路径列表，从a到b，pos 类型，在场景中的点
]]--
function GridManager:FindWayByAStarPosInScene(posA,posB)
    local listOfPosInScene = {};
    local listOfPathGrids = self:FindWayByAStar(posA,posB);
    for k,v in ipairs(listOfPosInScene) do 
        table.insert(listOfPosInScene,v:GetPosInScene()); 
    end
    return listOfPosInScene;
end

--[[
    计算从格子A到格子B的最短路径
    参数:
    posA:格子a
    posB:格子b
    返回
    table:路径列表，从a到b，Grid 类型
]]--
function GridManager:FindWayByAStar(posA, posB)
    local gridA = self.listOfGrids_[posA.x + (posA.y-1) * self.size_.width];
    local gridB = self.listOfGrids_[posB.x + (posB.y-1) * self.size_.width];
    local listOfOpen = { };
    local listOfClose = { };
    table.insert(listOfOpen, gridA);
    while #listOfOpen > 0 do
        local currentGrid, currentGridIndex = self:findLowerValueInList(listOfOpen, "f_");
        table.remove(listOfOpen, currentGridIndex);
        table.insert(listOfClose, currentGrid);
        if gridB == currentGrid then
            --[[ find ]]
            --
            local list =  self:searchPathBack(currentGrid);
            for k,v in ipairs(self.listOfGrids_) do 
                v:Dispose();
            end
            return list;
        end
        --[[
            计算一个格子的四项格子，并且将结果加入openlIST
        ]]
        --
        local listOfNearGrids = self:findNearCouldPassGridsList(currentGrid);
        for k,v in ipairs(listOfNearGrids) do 
            if self:isContain(listOfClose,v)then 
                
            elseif self:isContain(listOfOpen,v) == false then 
                v.g_ = currentGrid.g_+1;
                v.h_ = cc.pGetDistance(v:GetPos(),gridB:GetPos());
                v.f_ = v.g_ + v.h_ + v:GetPassValue();
                v.parent_ = currentGrid;
                table.insert(listOfOpen,v);
            else 
                local g = currentGrid.g_+1;
                local h = cc.pGetDistance(v:GetPos(),gridB:GetPos());
                local f = g + h + v:GetPassValue();
                if f < v.f_ then 
                    v.h_ = h;
                    v.g_ = g;
                    v.f_ = f;
                    v.parent_ = currentGrid;
                end
            end
        end
    end

    return {};
end


--[[
    获取网格
]]--
function GridManager:GetGrids()
    return self.listOfGrids_;
end

--[[
    打开关闭Debug模式
    参数:
    isOn:bool 是否打开Debug绘图
]]--
function GridManager:SetDebugIsOn(isOn)
    self.debugIsOn_ = isOn;
end


--[[
    设置Debug绘图的父节点
    参数:
    parent:node Debug绘图的父节点
]]--
function GridManager:SetDebugPaintNodeParent(parent)
    if self.debugDrawNode_ ~= null then
        self.debugDrawNode_:removeFromParent();
        self.debugDrawNode_ = null;
    end
    local sp = cc.Sprite:create(g_tConfigTable.sTaskpath .. "bgimg/GameLose.png");
    self.debugDrawNode_ = cc.DrawNode:create();
    local size = cc.Director:getInstance():getWinSize();
    parent:addChild(self.debugDrawNode_);
end


return GridManager;
-- endregion
--[[
list index 通过二维转1维度 考虑 二维y值的取值空间来计算 转一维的过程
]]--