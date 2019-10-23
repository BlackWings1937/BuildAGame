-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成
local Grid = { };

g_tConfigTable.CREATE_NEW(Grid);

-----------私有方法-------------------
function Grid:ctor()

end
Grid.EnumOrietion = {
    ["NONE"] = - 1,
    ["UP"] = 0,
    ["DOWN"] = 1,
    ["LEFT"] = 2,
    ["RIGHT"] = 3,
};
function Grid:init(pos, posInScene, passValue, size)
    self.pos_ = pos;
    self.posInScene_ = posInScene;
    self.passValue_ = passValue;
    self.size_ = size;
    self:Dispose();
end

-----------对外接口-------------------
--[[
    工厂方法
    参数:
    pos:网格的索引
    posInScene:网格对应场景中的位置
    passValue:通过代价
    返回
    Grid
]]--
function Grid.Create(pos, posInScene, passValue, size)
    local g = Grid.new();
    g:init(pos, posInScene, passValue, size);
    return g;
end


function Grid:Dispose()
    self.f_ = 0;
    self.g_ = 0;
    self.h_ = 0;
    self.pv_ = 0;
    self.cp_ = true;
    self.parent_ = nil;
end

function Grid:GetPos()
    return self.pos_;
end

function Grid:GetPosInScene()
    return self.posInScene_;
end

function Grid:GetPassValue()
    return self.passValue_;
end

function Grid:GetSize()
    return self.size_;
end

function Grid:SetPassValue(pv)
    self.pv_ = pv
end

function Grid:GetPassValue()
    return self.pv_;
end

function Grid:SetCouldPass(cp) 
    self.cp_ = cp;
end

function Grid:GetCouldPass()
    return self.cp_;
end

return Grid;
-- endregion
