requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");
local Actor = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.Actor");
local keyFrameParseReaderEvent = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.keyFrameParseReaderEvent");

local ActorEvent = class("ActorEvent",function()
    return Actor.new();
end);
g_tConfigTable.CREATE_NEW(ActorEvent);


function ActorEvent:initActorInfo(ai)
    Actor.initActorInfo(self,ai,keyFrameParseReaderEvent.new());
    self.actorUpdateData_ = {
        ["frameid"] = -1,
        ["eventname"] = "null",
    }
    self.nowFrameId_ = -1;
    self.playPath_ = "";
    self.eventList_ = {};
end

function ActorEvent:checkEventIsInQueue(eventName)
    local count = #self.eventList_;
    for i = 1,count,1 do 
        if eventName == self.eventList_[i] then 
            return true;
        end
    end
    return false;
end

function ActorEvent:Update(dt)
    if self.resultData == nil then 
        self.resultData = {};
    end
    self.resultData = self.frameReader_:GetDataByTime(dt,self.actorUpdateData_,self.resultData );
    
    if self.resultData~= nil then 
        if self.nowFrameId_ ~= self.resultData.frameid then 
            if self.resultData.eventname~="null" then 
                if self.scriptAction_~= nil then 
                    if self:checkEventIsInQueue(self.resultData.eventname) == false then 
                        self.scriptAction_:callCustomEvent(self.resultData.eventname);
                        table.insert(self.eventList_,self.resultData.eventname);
                    end
                end
            end
        end
    end
end



------------------对外接口---------------
function ActorEvent:InitByActorInfo(info)
    -- 保存角色信息
    self:initActorInfo(info);
end

function ActorEvent:Dispose()
    Actor.Dispose(self);
end

return ActorEvent;
