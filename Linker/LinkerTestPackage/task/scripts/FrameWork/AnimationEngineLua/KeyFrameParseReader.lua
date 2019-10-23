
requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");

local KeyFrameParseReader = class("KeyFrameParseReader")
g_tConfigTable.CREATE_NEW(KeyFrameParseReader);

---------------私有方法-------------
function KeyFrameParseReader:findKeyFrameIndexByTime(time)
    local count = self.countOfKeyFrame_;
    for i = 1,count ,1  do 
        local keyFrame = self.listOfKeyFrames_[i];
        if keyFrame.ht<= time and time< keyFrame.rt then 
            return i;
        end
    end
    return -1;
end

function KeyFrameParseReader:lerp(a,n,dt)
    return (a - n) * dt + n;
end

---------------对外接口-------------

--[[
    初始化关键帧读取器

    参数:
    listOfKeyFrames: array of keyFrame 关键帧数组
]]--
function KeyFrameParseReader:InitByListOfKeyFrames(listOfKeyFrames,frameRate)
    self.listOfKeyFrames_ = listOfKeyFrames.frames;
    self.countOfKeyFrame_ = #self.listOfKeyFrames_;
    if listOfKeyFrames.isFormatDT == nil then 

        local count = self.countOfKeyFrame_;
        local rearTime = 0;
        for i = 1,count, 1 do 
            local frame = self.listOfKeyFrames_[i];
            frame.dt = frame.dt/frameRate;
            frame.ht = rearTime;
            frame.rt = rearTime + frame.dt;
            rearTime = frame.rt;
        end
        listOfKeyFrames.isFormatDT = true;
    end
end

--[[
    获取某一具体时间的各个属性数据

    参数:
    time: float 具体的一个时间点
    data: table 需要知道的属性表 比如：{["position"] = cc.p(0,0),["rotation"] = 0}
    
    返回值
    data: table 返回传入data 标注的所有属性的值 比如 {["position"] = cc.p(35.7,42.6),["rotation"] = 35.6}
]]--
function KeyFrameParseReader:GetDataByTime(time,data,resultData)
    
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
        if nowKeyFrameInfo.einfo ~= nil then 
            for k,v in pairs(data) do 
                if nowKeyFrameInfo.einfo[k] ~= nil  then 
                    if nowKeyFrameInfo.mending then 
                        if type(nowKeyFrameInfo.einfo[k]) ~= "string" then 
                            resultData[k] = self:lerp(
                                nextKeyFrameInfo.einfo[k],
                                nowKeyFrameInfo.einfo[k],
                                (time - nowKeyFrameInfo.ht ) / nowKeyFrameInfo.dt);
                        else 
                            resultData[k] = nowKeyFrameInfo.einfo[k];
                        end
                    else 
                        resultData[k] = nowKeyFrameInfo.einfo[k];
                    end
                else 
                    if nowKeyFrameInfo[k] ~= nil then 
                        resultData[k] = nowKeyFrameInfo[k];
                    end
                end
             end
        else 
            return nil;
        end
        
    end
    return resultData;
end

return KeyFrameParseReader;