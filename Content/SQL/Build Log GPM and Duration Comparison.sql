


select cast(b.DatetimeStarted as date) Date,
	   b.DatetimeStarted,
	   b.SeasonID, 
	   b.source,
	   replace(b.InsertTime, '00:', '') InsertTime,
	   ((Min * 60) + Sec + cast(cast(MS as decimal(18,3))/1000 as decimal(18,3))) TotalSec,
	   s.Games + s.PlayoffGames Games,
	   cast((s.Games + s.PlayoffGames)/cast(((Min * 60) + Sec + cast(cast(MS as decimal(18,3))/1000 as decimal(18,3)))/60 as decimal(18,3)) as decimal(18,3)) GPM
from util.BuildLog b inner join
		Season s on b.SeasonID = s.SeasonID
order by b.DatetimeStarted desc

