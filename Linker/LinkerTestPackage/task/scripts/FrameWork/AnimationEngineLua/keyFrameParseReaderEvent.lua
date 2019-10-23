
requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");
local KeyFrameParseReader = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.KeyFrameParseReader");

local keyFrameParseReaderEvent = class("keyFrameParseReaderEvent",function() 
    return KeyFrameParseReader.new();
end)
g_tConfigTable.CREATE_NEW(keyFrameParseReaderEvent);

function keyFrameParseReaderEvent:findKeyFrameIndexByTime(time)
    local count = self.countOfKeyFrame_;
    for i = count, 1, -1  do 
        local keyFrame = self.listOfKeyFrames_[i];
        if keyFrame.ht<= time then 
            if keyFrame.eventname ~= nil then 
                return i;
            end
        end
    end
    return -1;
end


--[[
    获取某一具体时间的各个属性数据

    参数:
    time: float 具体的一个时间点
    data: table 需要知道的属性表 比如：{["position"] = cc.p(0,0),["rotation"] = 0}
    
    返回值
    data: table 返回传入data 标注的所有属性的值 比如 {["position"] = cc.p(35.7,42.6),["rotation"] = 35.6}
]]--
function keyFrameParseReaderEvent:GetDataByTime(time,data,resultData)
    local nowKeyFrameIndex = self:findKeyFrameIndexByTime(time);
    if nowKeyFrameIndex ~= -1 then 
        local nowKeyFrameInfo = nil;
        local nextKeyFrameInfo = nil;
        if nowKeyFrameIndex < self.countOfKeyFrame_ then 
            nowKeyFrameInfo = self.listOfKeyFrames_[nowKeyFrameIndex];
            nextKeyFrameInfo = self.listOfKeyFrames_[nowKeyFrameIndex + 1];
        else 
            nowKeyFrameInfo = self.listOfKeyFrames_[nowKeyFrameIndex];
            nextKeyFrameInfo = nowKeyFrameInfo;
        end
        for k,v in pairs(data) do 
            if nowKeyFrameInfo[k] ~= nil then 
                resultData[k] = nowKeyFrameInfo[k];
            else
                resultData[k] = data[k];
            end
         end
    else
            for k,v in pairs(data) do 
                if data[k] ~= nil then 
                    resultData[k] = data[k];
                end
             end
    end
    return resultData;
end


return keyFrameParseReaderEvent;