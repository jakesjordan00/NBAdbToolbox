

select * from PlayByPlay p order by SeasonID desc, GameID desc, ActionNumber desc
select * from StartingLineups
select * from PlayerBox p order by SeasonID desc, GameID desc
select * from TeamBox p order by SeasonID desc, GameID desc
select * from TeamBoxLineups p order by SeasonID desc, GameID desc
select * from Player p order by SeasonID desc
select * from Team p order by SeasonID desc
select * from Game p order by SeasonID desc, GameID
select * from Arena p order by SeasonID desc
select * from Official p order by SeasonID desc


select * from buildlog


delete from PlayByPlay
delete from PlayerBox
delete from TeamBox
delete from TeamBoxLineups
delete from Player
delete from Team
delete from Arena 
delete from Game 
delete from Official 