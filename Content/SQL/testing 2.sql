
select distinct t.SeasonID, t.GameID
from Game t
order by GameID desc

select distinct t.SeasonID, t.GameID
from PlayByPlay t
order by seasonID, GameID desc

select distinct t.SeasonID, t.GameID
from GameExt t
order by seasonID, GameID desc

select distinct t.SeasonID, t.GameID
from TeamBox t
order by GameID desc

select distinct t.SeasonID, t.GameID
from PlayerBox t
order by GameID desc

select distinct t.SeasonID, t.GameID
from PlayByPlay t
order by GameID desc

select distinct t.SeasonID, t.GameID
from StartingLineups t
order by GameID desc



select distinct t.SeasonID, t.GameID
from TeamBoxLineups t
order by GameID desc


select *
from util.MissingData

select t.SeasonID, count(t.TeamID) Teams
from Team t
group by t.SeasonID

select t.SeasonID, count(t.ArenaID) Arenas
from Arena t
group by t.SeasonID

select t.SeasonID, count(t.OfficialID) Officials
from Official t
group by t.SeasonID

select t.SeasonID, count(t.PlayerID) Players
from Player t 
group by t.SeasonID

select t.SeasonID, count(t.GameID) Games
from Game t
group by t.SeasonID

select t.SeasonID, count(t.GameID) Games
from GameExt t
group by t.SeasonID

select t.SeasonID, count(distinct t.GameID) Games
from TeamBox t
group by t.SeasonID

select t.SeasonID, count(distinct t.GameID) Games
from PlayerBox t
group by t.SeasonID

select t.SeasonID, count(distinct t.GameID) Games
from PlayByPlay t
group by t.SeasonID

select t.SeasonID, count(distinct t.GameID) Games
from StartingLineups t
group by t.SeasonID

select t.SeasonID, count(distinct t.GameID) Games
from TeamBoxLineups t
group by t.SeasonID


select *
from util.MissingData

select *
from BuildLog

select *, Games + PlayoffGames Total
from Season




select *
from Game g 
order by g.Date




