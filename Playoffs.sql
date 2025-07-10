
create view PlayoffSeries
as
select distinct g.SeasonID, g.SeriesID,
case when Label != 'NBA Finals' then hi.Conference else null end Conference,
case when cast(left(right(SeriesID, 2), 1) as int) = 1 then '1'
     when cast(left(right(SeriesID, 2), 1) as int) = 2 then '2'
     when cast(left(right(SeriesID, 2), 1) as int) = 3 then '3'
     when cast(left(right(SeriesID, 2), 1) as int) = 4 then '4'
else 0 end Round, 
hi.TeamID HighTeamID, hi.FullName HighSeed, th.Seed HSeed, 
lo.TeamID LowTeamID, lo.FullName LowSeed, tl.Seed LSeed,
concat(replace(replace(replace(e.Label, ' -', ''), 'East ', ''), 'West ', ''), ' - ', hi.Tricode, ' (', th.Seed, ') vs ', lo.Tricode, ' (', tl.Seed, ')') Description,
(select top 1 WinnerID from Game g2 where g.SeriesID = g2.SeriesID and g.SeasonID = g2.SeasonID order by GameID desc) WinnerID

from game g inner join
		GameExt e on g.GameID = e.GameID and g.SeasonID = e.SeasonID and e.LabelDetail like '%1' inner join
		Team hi on g.HomeID = hi.TeamID and g.SeasonID = hi.SeasonID inner join
		Team lo on g.AwayID = lo.TeamID and g.SeasonID = lo.SeasonID inner join
		TeamBox th on g.GameID = th.GameID and hi.TeamID = th.TeamID and g.SeasonID = th.SeasonID inner join
		TeamBox tl on g.GameID = tl.GameID and lo.TeamID = tl.TeamID and g.SeasonID = tl.SeasonID
where g.GameType = 'PS'