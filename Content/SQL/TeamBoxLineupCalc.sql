
create procedure TeamBoxLineupCalc @SeasonID int, @GameID int
as
select 
concat('insert into TeamBoxLineups values(', p.SeasonID, ', ', p.GameID, ', ', p.TeamID, ', ', p.MatchupID, ', ''', 
case when p.Starter = 1 then 'Starters' else 'Bench' end, ''', ''', 'minutesplaceholder'', '
     , sum(p.Points), ', ', sum(p.FG2M), ', ', sum(p.FG2A)
     , ', '
     , cast(sum(p.[FG2%]) as decimal (18, 2))
     , ', '
     , sum(p.FG3M) 
     , ', '
     , sum(p.FG3A) 
     , ', '
     , cast(sum(p.[FG3%]) as decimal (18, 2))
     , ', '
     , sum(p.FGM) 
     , ', '
     , sum(p.FGA) 
     , ', ' 
     , cast(sum(p.[FG%]) as decimal (18, 2))
     , ', '
     , sum(p.FTM) 
     , ', ' 
     , sum(p.FTA) 
     , ', ' 
     , cast(sum(p.[FT%]) as decimal (18, 2))
     , ', '
     , sum(p.ReboundsDefensive)    
     , ', '
     , sum(p.ReboundsOffensive)    
     , ', '
     , sum(p.ReboundsTotal)
     , ', '
     , sum(p.Assists)      
     , ', '
     , cast(sum(p.AssistsTurnoverRatio) as decimal (18, 2)) 
     , ', '
     , sum(p.Steals)
     , ', '
     , sum(p.Turnovers)    
     , ', '
     , sum(p.Blocks)
     , ', '
     , sum(p.FoulsPersonal) 
     , ')') InsertCmd     
	 , cast(cast(sum(p.MinutesCalculated) as decimal (18, 2)) as varchar(10)) MinutesCalculated   
from PlayerBox p
where Status != 'INACTIVE' and p.SeasonID = @SeasonID and GameID = @GameID
group by  p.SeasonID, p.GameID, p.TeamID, p.MatchupID, p.Starter
order by GameID, p.TeamID

