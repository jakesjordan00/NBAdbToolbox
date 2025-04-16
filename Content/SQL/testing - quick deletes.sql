

select * from PlayByPlay p order by SeasonID desc, GameID desc, ActionNumber desc		--500
select * from StartingLineups order by SeasonID desc, GameID desc						--30
select * from PlayerBox p order by SeasonID desc, GameID desc							--30
select * from TeamBox p order by SeasonID desc, GameID desc								--2
select * from TeamBoxLineups p order by SeasonID desc, GameID desc						--2
select * from Player p order by SeasonID desc											--30
select * from Team p order by SeasonID desc												--2
select * from Game p order by SeasonID desc, GameID										--1
select * from Arena p order by SeasonID desc											--1
select * from Official p order by SeasonID desc											--3

select * from Season

select distinct SeasonID from Team

select * from buildlog

delete from StartingLineups
delete from TeamBoxLineups
delete from PlayByPlay
delete from PlayerBox
delete from TeamBox
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