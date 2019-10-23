requirePack("scripts.FrameWork.Global.GlobalFunctions");
local FileUtil = requirePack("scripts.FrameWork.Util.FileUtil");
local AudioRecordManager = class("Toggle",function()
    return cc.Node:create(); 
end);
g_tConfigTable.CREATE_NEW(AudioRecordManager);
AudioRecordManager.TAG_ACTION_CHECK_AUDIO = 1001;
function AudioRecordManager:ctor()
    self.isCouldRecord_ = true;
    self.projName_ = "";
    self.listOfAudioPart_ = {};
    self.audioPartIndex_ = 1;
end

-- 录音结束回调
function AudioRecordManager:RecordEnd(iEndType)

end

function AudioRecordManager:copyToProj(path)
    local aimpath = g_tConfigTable.sTaskpath.."Audio"..self.projName_.."/"..self.audioPartIndex_..".wav";
    FileUtil.CopyTo(path,aimpath);
end

-------------对外接口---------------
function AudioRecordManager:SetMovieName(projName)
    self.projName_ = projName;
end
function AudioRecordManager:GetIsCouldRecord()
    return self.isCouldRecord_;
end
function AudioRecordManager:StartRecord()
    local path = g_tConfigTable.sTaskpath.."Audio"..self.projName_.."/";
    if FileUtil.ExistsDir(path) then 
        FileUtil.RemoveDir(path);
    end
    FileUtil.CreateDir(path);
    self.listOfAudioPart_ = {};
    self.audioPartIndex_  = 1;
end

function AudioRecordManager:PauseRecord()
    SoundUtil:getInstance():soundListenDetectStop();
    self:stopActionByTag(AudioRecordManager.TAG_ACTION_CHECK_AUDIO);
    self.isCouldRecord_ = false;
    local cbOfCopyFinish = function() 
        self.isCouldRecord_ = true;
    end
    -- 轮询复制
    local action = cc.RepeatForever:create(cc.Sequence:create(
        cc.DelayTime:create(0.016),
        cc.CallFunc:create(function()
            local effectPath = ArmatureDataDeal:sharedDataDeal():getRealOnlyPathByRelPath("", -2) .. "menuinfo/lastmicaudio.wav";
            if FileUtil.Exists(effectPath) then
                cbOfCopyFinish(); 
                self:copyToProj(effectPath);
                --FileUtil.Delete(effectPath);
                self:stopActionByTag(AudioRecordManager.TAG_ACTION_CHECK_AUDIO);
                self.audioPartIndex_ = self.audioPartIndex_ + 1;
                -- todo:write audio config to listOfAudioPart_
      
            end
        end)
        ,nil));
    action:setTag(AudioRecordManager.TAG_ACTION_CHECK_AUDIO);
    self:runAction(action);
end

function AudioRecordManager:ResumeRecord()
    local effectPath = ArmatureDataDeal:sharedDataDeal():getRealOnlyPathByRelPath("", -2) .. "menuinfo/lastmicaudio.wav";
    if FileUtil.Exists(effectPath) then 
        FileUtil.Delete(effectPath);
    end
    SoundUtil:getInstance():soundListenDetectLua(
        Utils:GetInstance().sourceType,
        Utils:GetInstance().sourceId,
        90,
        function(iEndType)
            self:RecordEnd(iEndType)
        end
    )
end

function AudioRecordManager:StopRecord()
    self:PauseRecord();
end

return AudioRecordManager;
