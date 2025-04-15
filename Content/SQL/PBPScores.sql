

create trigger PBPScores
on PlayByPlay after insert as
update PlayByPlay set ScoreHome = (
select top 1 p.ScoreHome 
from PlayByPlay p inner join
		inserted i on p.SeasonID = i.SeasonID and p.GameID = i.GameID and p.ActionNumber < i.ActionNumber
where p.ScoreHome is not null order by p.ActionNumber desc),
ScoreAway = (
select top 1 p.ScoreAway 
from PlayByPlay p inner join
		inserted i on p.SeasonID = i.SeasonID and p.GameID = i.GameID and p.ActionNumber < i.ActionNumber
where p.ScoreAway is not null order by p.ActionNumber desc)
where ActionNumber = (select ActionNumber from inserted) and ScoreAway is null and ScoreHome is null