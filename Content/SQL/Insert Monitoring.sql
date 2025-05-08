
select t.SeasonID,
sum(case when GameType = 'RS' then 1 else 0 end) [Regular Season],
sum(case when GameType = 'PS' then 1 else 0 end) [Post Season],	
count(t.GameID) [Total Games]
from Game t
group by t.SeasonID
order by SeasonID desc



select t.SeasonID, concat('Game - ', count(t.GameID)) [Table - Games], count(t.GameID) Games
from Game t
group by t.SeasonID
union
select t.SeasonID, concat('GameExt - ', count(t.GameID)), count(t.GameID)
from GameExt t
group by t.SeasonID
union
select t.SeasonID, concat('Tbox - ', count(distinct t.GameID)), count(distinct t.GameID)
from TeamBox t
group by t.SeasonID
union
select t.SeasonID, concat('PBox - ', count(distinct t.GameID)), count(distinct t.GameID)
from PlayerBox t
group by t.SeasonID
union
select t.SeasonID, concat('PBP - ', count(distinct t.GameID)), count(distinct t.GameID)
from PlayByPlay t
group by t.SeasonID
union
select t.SeasonID, concat('SLineups - ', count(distinct t.GameID)), count(distinct t.GameID)
from StartingLineups t
group by t.SeasonID
union
select t.SeasonID, concat('TboxLineups - ', count(distinct t.GameID)), count(distinct t.GameID)
from TeamBoxLineups t
group by t.SeasonID
union
select t.SeasonID, concat('Season - ', Games + PlayoffGames), Games + PlayoffGames
from Season t
order by SeasonID desc







select * from util.BuildLog


select Max(cast(right(GameID, 4) as int))/10
from Game 
where SeasonID = 2016
