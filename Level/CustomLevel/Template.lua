--测试模式-子模式

infotable =nil

function FightStart(ignore)
	infotable={}
end

function Update(time,deltaTime)
	-- body
end

function JudgeEnd(ignore)
	return false
end

function KillScore(ignore)
	return 1000
end

function TimeScore(ignore)
	return 2222
end

function ModeScore(ignore)
	return 3600
end

function ReleaseData(ignore)
	infotable=nil
end
