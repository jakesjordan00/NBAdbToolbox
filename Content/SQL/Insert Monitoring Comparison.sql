
select t.SeasonID, concat('Game - ', (s.Games + s.PlayoffGames) - count(t.GameID)) [Table - Game Discrepancy], (s.Games + s.PlayoffGames) - count(t.GameID) [Game Discrepancy],
count(t.GameID) Games
from Game t inner join
		Season s on t.SeasonID = s.SeasonID
group by t.SeasonID, s.Games, s.PlayoffGames
union
select t.SeasonID, concat('GameExt - ', (s.Games + s.PlayoffGames) - count(t.GameID)) [Table - Game Discrepancy], (s.Games + s.PlayoffGames) - count(t.GameID) [Game Discrepancy],
count(t.GameID) Games
from GameExt t inner join
		Season s on t.SeasonID = s.SeasonID
group by t.SeasonID, s.Games, s.PlayoffGames
union 
select t.SeasonID, concat('Tbox - ', (s.Games + s.PlayoffGames) - count(distinct t.GameID)) [Table - Game Discrepancy], (s.Games + s.PlayoffGames) - count(distinct t.GameID) [Game Discrepancy],
count(distinct t.GameID)
from TeamBox t inner join
		Season s on t.SeasonID = s.SeasonID
group by t.SeasonID, s.Games, s.PlayoffGames
union
select t.SeasonID, concat('PBox - ', (s.Games + s.PlayoffGames) - count(distinct t.GameID)) [Table - Game Discrepancy], (s.Games + s.PlayoffGames) - count(distinct t.GameID) [Game Discrepancy],
count(distinct t.GameID)
from PlayerBox t inner join
		Season s on t.SeasonID = s.SeasonID
group by t.SeasonID, s.Games, s.PlayoffGames
union
select t.SeasonID, concat('PBP - ', (s.Games + s.PlayoffGames) - count(distinct t.GameID)) [Table - Game Discrepancy], (s.Games + s.PlayoffGames) - count(distinct t.GameID) [Game Discrepancy],
count(distinct t.GameID)
from PlayByPlay t inner join
		Season s on t.SeasonID = s.SeasonID
group by t.SeasonID, s.Games, s.PlayoffGames
union
select t.SeasonID, concat('SLineups - ', (s.Games + s.PlayoffGames) - count(distinct t.GameID)) [Table - Game Discrepancy], (s.Games + s.PlayoffGames) - count(distinct t.GameID) [Game Discrepancy],
count(distinct t.GameID)
from StartingLineups t inner join
		Season s on t.SeasonID = s.SeasonID
group by t.SeasonID, s.Games, s.PlayoffGames
union
select t.SeasonID, concat('TboxLineups - ', (s.Games + s.PlayoffGames) - count(distinct t.GameID)) [Table - Game Discrepancy], (s.Games + s.PlayoffGames) - count(distinct t.GameID) [Game Discrepancy],
count(distinct t.GameID)
from TeamBoxLineups t inner join
		Season s on t.SeasonID = s.SeasonID
group by t.SeasonID, s.Games, s.PlayoffGames



order by SeasonID desc, [Game Discrepancy] desc

