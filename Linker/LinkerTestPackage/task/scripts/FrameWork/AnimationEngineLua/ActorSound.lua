requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");
local Actor = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.Actor");
local KeyFrameParseReaderSound = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.KeyFrameParseReaderSound");

local ActorSound = class("ActorSound",function()
    return Actor.new();
end);
g_tConfigTable.CREATE_NEW(ActorSound);


function ActorSound:initActorInfo(ai)
    Actor.initActorInfo(self,ai,KeyFrameParseReaderSound.new());
    self.actorUpdateData_ = {
        ["frameid"] = -1,
        ["name"] = "null",
        ["mode"] = "null",
        ["loop"] = 0,
    }
    self.nowFrameId_ = -1;
    self.playPath_ = "";
end

function ActorSound:Update(dt)
    if self.resultData == nil then 
        self.resultData = {};
    end
    self.resultData = self.frameReader_:GetDataByTime(dt,self.actorUpdateData_,self.resultData );
    if self.resultData~= nil then 
        if self.nowFrameId_ ~= self.resultData.frameid then 
            self:updateSoundFrame(self.resultData);
            self.nowFrameId_ =  self.resultData.frameid; 
        end
    end
end

function ActorSound:getNickNamePath()
    if Utils:GetInstance():hasNickname() == false then 
        return  GET_REAL_PATH_ONLY("", PathGetRet_ONLY_BASIC).."theme/common/sounds/nickname.mp3"
    else 
        return SoundUtil:getInstance():getbabynamepath();
    end
end


function ActorSound:updateSoundFrame(data)
    if data.name == "null" or data.mode == "null" then 
        SoundUtil:getInstance():stop(self.playPath_);
    else
        if self.playPath_ ~= nil and self.playPath_  ~= "" then 
            SoundUtil:getInstance():stop(self.playPath_);
        end
        if data.name == "nickname.mp3" then 
            self.playPath_ = self:getNickNamePath();
        else 
            self.playPath_ = self.scriptAction_:GetAudioResPath()..data.name;
        end
        if data.mode == "loop" then 
            SoundUtil:getInstance():playLua(self.playPath_,self.playPath_,function() 
                self:repeatPlay();
            end);
        else 
            self.playTime_ = 0;
            self.aimPlayTime_ = data.loop;
            self:PlayTime();
        end
    end
end

function ActorSound:playCallBack()

end

function ActorSound:PlayTime()
    SoundUtil:getInstance():playLua(self.playPath_,self.playPath_,function() 
        self.playTime_  = self.playTime_ + 1;
        if self.playTime_<self.aimPlayTime_ then 
            self:PlayTime();
        end
    end);
end

function ActorSound:repeatPlay()
    if self.playPath_ ~= nil and self.playPath_  ~= "" then 
        SoundUtil:getInstance():playLua(self.playPath_,self.playPath_,function() 
            self:repeatPlay();
        end);
    end
end


------------------对外接口---------------
function ActorSound:InitByActorInfo(info)
    -- 保存角色信息
    self:initActorInfo(info);
end

function ActorSound:Dispose()
    if self.playPath_ ~= nil and self.playPath_  ~= "" then 
        SoundUtil:getInstance():stop(self.playPath_);
    end
    Actor.Dispose(self);
end

return ActorSound;
