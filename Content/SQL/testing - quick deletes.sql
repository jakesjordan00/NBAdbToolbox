

select * from PlayByPlay p order by SeasonID desc, GameID desc, ActionNumber desc		--500
select * from StartingLineups order by SeasonID desc, GameID desc						--30
select * from PlayerBox p order by SeasonID desc, GameID desc							--30
select * from TeamBox p order by SeasonID desc, GameID desc								--2
select * from TeamBoxLineups p order by SeasonID desc, GameID desc						--2
select * from Player p order by SeasonID desc											--30
select * from Team p order by SeasonID desc												--2
select * from GameExt p order by SeasonID , GameID										--1
select * from Game p order by SeasonID desc, GameID	desc									--1
select * from Arena p order by SeasonID desc											--1
select * from Official p order by SeasonID desc											--3

delete from StartingLineups
delete from TeamBoxLineups
delete from PlayByPlay
delete from PlayerBox
delete from TeamBox
delete from GameExt
delete from Game
delete from Player
delete from Official
delete from Arena
delete from Team
delete from util.MissingData





select * from util.MissingData p
order by GameID



select * from Season

select distinct SeasonID from Team

select * from util.buildlog

delete from util.MissingData	
delete from StartingLineups
delete from TeamBoxLineups
delete from PlayByPlay
delete from PlayerBox
delete from TeamBox
delete from GameExt
delete from Game
delete from Player
delete from Official
delete from Arena
delete from Team

SELECT sum(rows) Rows
from sys.tables t inner join
		sys.partitions p on t.object_id = p.object_id
WHERE type_desc = 'USER_TABLE'



select seasonID, Games + PlayoffGames Games from Season


select SeasonID, count(GameID) Games
from Game
group by SeasonID


execute  Seasons







select * from StartingLineups		where SeasonID = 2019 order by GameID desc
--select * from TeamBoxLineups		where SeasonID = 2019 order by GameID desc
select * from PlayByPlay			where SeasonID = 2019 order by GameID desc
select * from PlayerBox				where SeasonID = 2019 order by GameID desc
select * from TeamBox				where SeasonID = 2019 order by GameID desc
select * from Game 					where SeasonID = 2019 order by GameID desc
select * from Player				where SeasonID = 2019
select * from Official 				where SeasonID = 2019
select * from Arena 				where SeasonID = 2019
select * from Team					where SeasonID = 2019

select * from util.MissingData


delete from StartingLineups		where SeasonID != 2019 --7
delete from TeamBoxLineups		where SeasonID != 2019 --10
delete from PlayByPlay			where SeasonID != 2019 --4
delete from PlayerBox			where SeasonID != 2019 --6
delete from TeamBox				where SeasonID != 2019 --9
delete from GameExt 			where SeasonID != 2019 --2
delete from Game 				where SeasonID != 2019 --2
delete from Player				where SeasonID != 2019 --5
delete from Official 			where SeasonID != 2019 --3
delete from Arena 				where SeasonID != 2019 --1
delete from Team				where SeasonID != 2019 --8




select * from Arena 			 --1
select * from StartingLineups	 --7
select * from TeamBoxLineups	 --10
select * from PlayByPlay		 --4
select * from PlayerBox			 --6
select * from TeamBox			 --9
select * from Game 				 --2
select * from Player			 --5
select * from Official 			 --3
select * from Team				 --8






select distinct p.GameID
from PlayByPlay p
--where p.TimeActual is null
order by GameID desc

select distinct p.GameID
from Game p
--where p.TimeActual is null
order by GameID desc


select *
from util.MissingData
order by GameID desc
go

select * from util.BuildLog

delete from PlayByPlay where GameID = 21900193

drop procedure SelectGamesDeletePBP


create procedure SelectGamesDeletePBP @Season int
as
select distinct GameID from Game where SeasonID = @Season and GameID >= 21900193
delete from PlayByPlay where SeasonID = @Season


delete from util.MissingData
