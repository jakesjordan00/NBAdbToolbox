

select * from PlayByPlay p order by SeasonID desc, GameID desc, ActionNumber desc
select * from StartingLineups order by SeasonID desc, GameID desc
select * from PlayerBox p order by SeasonID desc, GameID desc
select * from TeamBox p order by SeasonID desc, GameID desc
select * from TeamBoxLineups p order by SeasonID desc, GameID desc
select * from Player p order by SeasonID desc
select * from Team p order by SeasonID desc
select * from Game p order by SeasonID desc, GameID
select * from Arena p order by SeasonID desc
select * from Official p order by SeasonID desc

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



SELECT t.Name, p.rows Rows
from sys.tables t inner join
		sys.partitions p on t.object_id = p.object_id
WHERE type_desc = 'USER_TABLE'



select seasonID, Games + PlayoffGames Games from Season