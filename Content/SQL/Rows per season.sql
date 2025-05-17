with RowCounts as(
select p.SeasonID, concat('Game - ', count(p.SeasonID)) TableRows, count(p.SeasonID) Rows
from Game p
group by SeasonID
union
select p.SeasonID, concat('GameExt - ', count(p.SeasonID)) TableRows, count(p.SeasonID) Rows
from GameExt p
group by SeasonID
union
select p.SeasonID, concat('TBox - ', count(p.SeasonID)) TableRows, count(p.SeasonID) Rows
from TeamBox p
group by SeasonID
union
select p.SeasonID, concat('PBox - ', count(p.SeasonID)) TableRows, count(p.SeasonID) Rows
from PlayerBox p
group by SeasonID
union
select p.SeasonID, concat('PBP - ', count(p.SeasonID)) TableRows, count(p.SeasonID) Rows
from PlayByPlay p
group by SeasonID
union
select p.SeasonID, concat('SLineups - ', count(p.SeasonID)) TableRows, count(p.SeasonID) Rows
from StartingLineups p
group by SeasonID
union
select p.SeasonID, concat('TboxLineups - ', count(p.SeasonID)) TableRows, count(p.SeasonID) Rows
from TeamBoxLineups p
group by SeasonID
union
select p.SeasonID, concat('Season - ', Games + PlayoffGames) Rows, Games + PlayoffGames Rows
from Season p
)
select r.SeasonID, sum(rows) Rows
from RowCounts r
group by r.SeasonID



