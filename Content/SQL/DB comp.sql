
use KawhiNBAdb
select t.SeasonID,
sum(case when GameType = 'RS' then 1 else 0 end) [Regular Season],
sum(case when GameType = 'PS' then 1 else 0 end) [Post Season],
count(t.GameID) [Total Games]
from Game t
group by t.SeasonID
order by SeasonID desc

use KawhiNBAdbCurrentHistoric
select t.SeasonID,
sum(case when GameType = 'RS' then 1 else 0 end) [Regular Season],
sum(case when GameType = 'PS' then 1 else 0 end) [Post Season],
count(t.GameID) [Total Games]
from Game t
group by t.SeasonID
order by SeasonID desc

use KawhiNBAdbHistoric
select t.SeasonID,
sum(case when GameType = 'RS' then 1 else 0 end) [Regular Season],
sum(case when GameType = 'PS' then 1 else 0 end) [Post Season],
count(t.GameID) [Total Games]
from Game t
group by t.SeasonID
order by SeasonID desc



select t.SeasonID, count(t.GameID) Game_Games
from Game t
group by t.SeasonID
order by SeasonID desc

select t.SeasonID, count(t.GameID) GameExt_Games
from GameExt t
group by t.SeasonID
order by SeasonID desc

select t.SeasonID, count(distinct t.GameID) TBox_Games
from TeamBox t
group by t.SeasonID
order by SeasonID desc

select t.SeasonID, count(distinct t.GameID) PBox_Games
from PlayerBox t
group by t.SeasonID
order by SeasonID desc

select t.SeasonID, count(distinct t.GameID) PBP_Games
from PlayByPlay t
group by t.SeasonID
order by SeasonID desc

select t.SeasonID, count(distinct t.GameID) SLineups_Games
from StartingLineups t
group by t.SeasonID
order by SeasonID desc

select t.SeasonID, count(distinct t.GameID) TBoxLineups_Games
from TeamBoxLineups t
group by t.SeasonID
order by SeasonID desc


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
