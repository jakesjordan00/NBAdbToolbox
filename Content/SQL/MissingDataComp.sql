



select distinct DatabaseName
from missingDataComp m



select m.GameID, count(m.GameID) count
from missingDataComp m
group by m.GameID
having count(m.GameID) = 10
order by count desc