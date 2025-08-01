


create table Season(
SeasonID			int,
ChampionID			int,
Games				int,
PlayoffGames		int,
HistoricLoaded		int,
CurrentLoaded		int,
Primary Key(SeasonID))

create table Team(SeasonID			int,
TeamID				int,
City				varchar(255),
Name				varchar(255),
Tricode				varchar(255),
Wins				int,
Losses				int,
FullName			varchar(255),
Conference          varchar(255),
Division            varchar(255),
Primary Key(SeasonID, TeamID),
Foreign Key (SeasonID) references Season(SeasonID))

create table Arena(
SeasonID			int,
ArenaID				int,
TeamID				int,
City				varchar(255),
Country				varchar(255),
Name				varchar(255),
PostalCode			varchar(255),
State				varchar(255),
StreetAddress		varchar(255),
Timezone			varchar(255),
Primary Key (SeasonID, ArenaID),
Foreign Key (SeasonID) references Season(SeasonID),
Foreign Key (SeasonID, TeamID) references Team(SeasonID, TeamID))

create table Official(
SeasonID			int,
OfficialID			int,
Name				varchar(255),
Number				varchar(3)
Primary Key(SeasonID, OfficialID),
Foreign Key (SeasonID) references Season(SeasonID))


create table Player(
SeasonID			int,
PlayerID			int,
Name				varchar(255),
Number				varchar(3),
Position			varchar(100),
Primary Key(SeasonID, PlayerID),
Foreign Key (SeasonID) references Season(SeasonID))

create table Game(
SeasonID			int,
GameID				int,
Date				date,
GameType			varchar(10),
HomeID				int,
HScore				int,
AwayID				int,
AScore				int,
WinnerID			int,
WScore				int,
LoserID				int,
LScore				int,
SeriesID			varchar(20),
Datetime			datetime,
Primary Key(SeasonID, GameID),
Foreign Key (SeasonID) references Season(SeasonID),
Foreign Key (SeasonID, HomeID) references Team(SeasonID, TeamID),
Foreign Key (SeasonID, AwayID) references Team(SeasonID, TeamID),
Foreign Key (SeasonID, WinnerID) references Team(SeasonID, TeamID),
Foreign Key (SeasonID, LoserID) references Team(SeasonID, TeamID))

create table GameExt(
SeasonID			int,
GameID				int,
ArenaID             int,
Attendance			int,
Sellout				int,
Label				varchar(100),
LabelDetail			varchar(100),
OfficialID			int,
Official2ID			int,
Official3ID			int,
OfficialAlternateID int,
Status				varchar(50),
Primary Key(SeasonID, GameID),
Foreign Key (SeasonID) references Season(SeasonID),
Foreign Key (SeasonID, GameID) references Game(SeasonID, GameID),
Foreign Key (SeasonID, ArenaID) references Arena(SeasonID, ArenaID),
Foreign Key (SeasonID, OfficialID) references Official(SeasonID, OfficialID),
Foreign Key (SeasonID, Official2ID) references Official(SeasonID, OfficialID),
Foreign Key (SeasonID, Official3ID) references Official(SeasonID, OfficialID)
)

create table TeamBox(
SeasonID					int,
GameID						int,
TeamID						int,
MatchupID					int,
--Points
Points int,
PointsAgainst int,
--Field Goals			
FG2M int,
FG2A int,	
[FG2%] float,
FG3M int,
FG3A int,
[FG3%] float,
FGM int,
FGA int,
[FG%] float,
FieldGoalsEffectiveAdjusted float,
FTM int,
FTA int,
[FT%]				float,
SecondChancePointsMade int,
SecondChancePointsAttempted int,
SecondChancePointsPercentage float,
TrueShootingAttempts float,
TrueShootingPercentage float,
PointsFromTurnovers int,
PointsSecondChance int,
PointsInThePaint int,
PointsInThePaintMade int,
PointsInThePaintAttempted int,
PointsInThePaintPercentage float,
PointsFastBreak int,
FastBreakPointsMade int,
FastBreakPointsAttempted int,
FastBreakPointsPercentage float,
BenchPoints int,
--Rebounds
ReboundsDefensive int,
ReboundsOffensive int,
ReboundsPersonal int,
ReboundsTeam int,
ReboundsTeamDefensive int,
ReboundsTeamOffensive int,
ReboundsTotal int,
Assists int,
AssistsTurnoverRatio float,
BiggestLead int,
BiggestLeadScore varchar(30),
BiggestScoringRun int,
BiggestScoringRunScore varchar(30),
TimeLeading varchar(30),				--replace(replace(replace(timeLeading, 'PT', ''), 'M', ':'), 'S', '')
TimesTied int,
LeadChanges int,
Steals int,
--Turnovers
Turnovers int,
TurnoversTeam int,
TurnoversTotal int,
Blocks int,
BlocksReceived int,
FoulsDrawn int,
FoulsOffensive int,
FoulsPersonal int,
FoulsTeam int,
FoulsTeamTechnical int,
FoulsTechnical int,
Wins int,
Losses int,
Win	int,
Seed int,
Primary Key (SeasonID, GameID, TeamID, MatchupID),
Foreign Key (SeasonID) references Season(SeasonID),
Foreign Key (SeasonID, GameID) references Game(SeasonID, GameID),
Foreign Key (SeasonID, TeamID) references Team(SeasonID, TeamID),
Foreign Key (SeasonID, MatchupID) references Team(SeasonID, TeamID)
)

create table PlayerBox(
SeasonID					int,
GameID						int,
TeamID						int,
MatchupID					int,
PlayerID					int,
Status						varchar(20),
Starter						int,
Position					varchar(2),
Minutes						varchar(30),
MinutesCalculated			float,
Points						int,
Assists						int,
ReboundsTotal				int,
FG2M						int,
FG2A						int,
[FG2%]						float,
FG3M						int,
FG3A						int,
[FG3%]						float,
FGM							int,
FGA							int,
[FG%]						float,
FTM							int,
FTA							int,
[FT%]						float,
ReboundsDefensive			int,
ReboundsOffensive			int,
Blocks						int,
BlocksReceived				int,
Steals						int,
Turnovers					int,
AssistsTurnoverRatio		float,
Plus						float,
Minus						float,
PlusMinusPoints				float,
PointsFastBreak				int,
PointsInThePaint			int,
PointsSecondChance			int,
FoulsOffensive				int,
FoulsDrawn					int,
FoulsPersonal				int,
FoulsTechnical				int,
StatusReason				varchar(100),
StatusDescription			varchar(200),
Primary Key(SeasonID, GameID, TeamID, MatchupID, PlayerID),
Foreign Key (SeasonID) references Season(SeasonID),
Foreign Key (SeasonID, GameID) references Game(SeasonID, GameID),
Foreign Key (SeasonID, TeamID) references Team(SeasonID, TeamID),
Foreign Key (SeasonID, PlayerID) references Player(SeasonID, PlayerID),
Foreign Key (SeasonID, GameID, TeamID, MatchupID) references TeamBox(SeasonID, GameID, TeamID, MatchupID))

create table PlayByPlay(
SeasonID			int,
GameID				int,
ActionID			int,
ActionNumber		int,
Qtr					int,
Clock				varchar(20),
TimeActual			datetime,
ScoreHome			int,
ScoreAway			int,
Possession			int,
TeamID				int,
Tricode				varchar(3),
PlayerID			int,
Description			varchar(999),
SubType				varchar(999),
IsFieldGoal			int,
ShotResult			varchar(999),
ShotValue			int,
ActionType			varchar(999),
ShotDistance		float,
Xlegacy				float,
Ylegacy				float,
X					float,
Y					float,
Location			varchar(35),
Area				varchar(50),
AreaDetail			varchar(50),
Side				varchar(30),
ShotType			varchar(4),
PtsGenerated		int,
Descriptor			varchar(30),
Qual1				varchar(30),
Qual2				varchar(30),
Qual3				varchar(30),
ShotActionNbr		int,
PlayerIDAst			int,
PlayerIDBlk			int,
PlayerIDStl			int,
PlayerIDFoulDrawn	int,
PlayerIDJumpW		int,
PlayerIDJumpL		int,
OfficialID			int,
QtrType				varchar(20),
Primary Key(SeasonID, GameID, ActionNumber, ActionID),
Foreign Key (SeasonID) references Season(SeasonID),
Foreign Key (SeasonID, GameID) references Game(SeasonID, GameID))

create table TeamBoxLineups(
SeasonID					int,
GameID						int,
TeamID						int,
MatchupID					int,
Unit						varchar(30),
Minutes						varchar(30),
Points						int,
FG2M						int,
FG2A						int,
[FG2%]						float,
FG3M						int,
FG3A						int,
[FG3%]						float,
FGM							int,
FGA							int,
[FG%]						float,
FTM							int,
FTA							int,
[FT%]						float,
ReboundsDefensive			int,
ReboundsOffensive			int,
ReboundsTotal				int,
Assists						int,
AssistsTurnoverRatio		float,
Steals						int,
Turnovers					int,
Blocks						int,
FoulsPersonal				int,
Primary Key (SeasonID, GameID, TeamID, MatchupID, Unit),
Foreign Key (SeasonID) references Season(SeasonID),
Foreign Key (SeasonID, GameID) references Game(SeasonID, GameID),
Foreign Key (SeasonID, TeamID) references Team(SeasonID, TeamID),
Foreign Key (SeasonID, MatchupID) references Team(SeasonID, TeamID),
Foreign Key (SeasonID, GameID, TeamID, MatchupID) references TeamBox(SeasonID, GameID, TeamID, MatchupID))

create table StartingLineups(
SeasonID		int,
GameID			int,
TeamID			int,
MatchupID		int,
PlayerID		int,
Unit			varchar(30),
Position		varchar(10),
Primary Key(SeasonID, GameID, TeamID, MatchupID, PlayerID),
Foreign Key (SeasonID) references Season(SeasonID),
Foreign Key (SeasonID, GameID) references Game(SeasonID, GameID),
Foreign Key (SeasonID, TeamID) references Team(SeasonID, TeamID),
Foreign Key (SeasonID, PlayerID) references Player(SeasonID, PlayerID),
Foreign Key (SeasonID, GameID, TeamID, MatchupID, PlayerID) references PlayerBox(SeasonID, GameID, TeamID, MatchupID, PlayerID))



create table PlayerMovement(
Date date,
Type varchar(255),
Description varchar(355),
TeamID int,
PlayerID int,
AddTeamID int,
GroupSort varchar(255))

/*
--The below section will be used to create a new string for procedure creation
~create procedure SeasonInsert
as
insert into Season values(1996, 1610612741, 1189, 72, 0, 0)
insert into Season values(1997, 1610612741, 1189, 71, 0, 0)
insert into Season values(1998, 1610612759, 725, 66, 0, 0)
insert into Season values(1999, 1610612747, 1189, 75, 0, 0)
insert into Season values(2000, 1610612747, 1189, 71, 0, 0)
insert into Season values(2001, 1610612747, 1189, 71, 0, 0)
insert into Season values(2002, 1610612759, 1189, 88, 0, 0)
insert into Season values(2003, 1610612765, 1189, 82, 0, 0)
insert into Season values(2004, 1610612759, 1230, 84, 0, 0)
insert into Season values(2005, 1610612748, 1230, 89, 0, 0)
insert into Season values(2006, 1610612759, 1230, 79, 0, 0)
insert into Season values(2007, 1610612738, 1230, 86, 0, 0)
insert into Season values(2008, 1610612747, 1230, 85, 0, 0)
insert into Season values(2009, 1610612747, 1230, 82, 0, 0)
insert into Season values(2010, 1610612742, 1230, 81, 0, 0)
insert into Season values(2011, 1610612748, 990, 84, 0, 0)
insert into Season values(2012, 1610612748, 1229, 85, 0, 0)
insert into Season values(2013, 1610612759, 1230, 89, 0, 0)
insert into Season values(2014, 1610612744, 1230, 81, 0, 0)
insert into Season values(2015, 1610612739, 1230, 86, 0, 0)
insert into Season values(2016, 1610612744, 1230, 79, 0, 0)
insert into Season values(2017, 1610612744, 1230, 81, 0, 0)
insert into Season values(2018, 1610612761, 1230, 82, 0, 0)
insert into Season values(2019, 1610612747, 1059, 83, 0, 0)
insert into Season values(2020, 1610612749, 1080, 91, 0, 0)
insert into Season values(2021, 1610612744, 1230, 93, 0, 0)
insert into Season values(2022, 1610612743, 1230, 90, 0, 0)
insert into Season values(2023, 1610612738, 1230, 89, 0, 0)
insert into Season values(2024, 1610612760, 1230, 91, 0, 0)
~~~

create schema util
~~~

create table util.MissingData(
SeasonID		int,
GameID			int,
Source			varchar(15),
MissingData		varchar(100),
Note			varchar(999),
Primary Key(SeasonID, GameID, Source, MissingData),
Foreign Key (SeasonID) references Season(SeasonID))
~~~

create table util.BuildLog(
BuildID int,
RunID INT IDENTITY(1,1),
SeasonID int,
Source varchar(25),
Hr  int,
Min int,
Sec int,
Ms  int,
FullTime varchar(255),
HrR  int,
MinR int,
SecR int,
MsR  int,
ReadTime varchar(255),
HrI  int,
MinI int,
SecI int,
MsI  int,
InsertTime varchar(255),
DatetimeStarted datetime,
DatetimeComplete datetime,
Primary Key (BuildID, RunID),
Foreign Key (SeasonID) references Season(SeasonID))
~~~

create procedure Tables
as
select t.Name, p.rows Rows
from sys.tables t inner join
		sys.partitions p on t.object_id = p.object_id
where type_desc = 'USER_TABLE'
~~~

create procedure Seasons
as
with Seasons as(
select s.SeasonID, s.Games + s.PlayoffGames Games, case when s.HistoricLoaded = 1 or s.CurrentLoaded = 1 then 1 else 0 end Loaded
	 , (select COUNT(distinct TeamID) from Team t where s.SeasonID = t.SeasonID) Team
	 , (select COUNT(distinct ArenaID) from Arena a where s.SeasonID = a.SeasonID) Arena
	 , (select COUNT(distinct PlayerID) from Player p where s.SeasonID = p.SeasonID) Player
	 , (select COUNT(distinct OfficialID) from Official o where s.SeasonID = o.SeasonID) Official
     , COUNT(distinct g.GameID) Game
	 , (select COUNT(distinct pb.GameID) from PlayerBox pb where s.SeasonID = pb.SeasonID) PlayerBox
	 , (select COUNT(distinct Tb.GameID) from TeamBox Tb where s.SeasonID = Tb.SeasonID) TeamBox
	 , (select COUNT(distinct pbp.GameID) from PlayByPlay pbp where s.SeasonID = pbp.SeasonID) PlayByPlay
	 , (select COUNT(distinct sl.GameID) from StartingLineups sl where s.SeasonID = sl.SeasonID) StartingLineups
	 , (select COUNT(distinct tbl.GameID) from TeamBoxLineups tbl where s.SeasonID = tbl.SeasonID) TeamBoxLineups
	 , HistoricLoaded, CurrentLoaded
	 , PBoxRows
	 , TboxRows
	 , PbpRows
	 , StartingLineupsRows
	 , TboxLineupRows

from Season s left join
		Game g on s.SeasonID = g.SeasonID left join(
select SeasonID, COUNT(PlayerID) as PBoxRows  
from PlayerBox group by SeasonID) 
pb on s.SeasonID = pb.SeasonID left join(
select SeasonID, COUNT(TeamID) as TboxRows  
from TeamBox group by SeasonID) 
tb on s.SeasonID = tb.SeasonID left join(
select SeasonID, COUNT(ActionID) as PbpRows  
from PlayByPlay group by SeasonID) 
pbp on s.SeasonID = pbp.SeasonID left join(
select SeasonID, COUNT(Unit) as StartingLineupsRows  
from StartingLineups group by SeasonID) 
sl on s.SeasonID = sl.SeasonID left join(
select SeasonID, COUNT(Unit) as TboxLineupRows  
from TeamBoxLineups group by SeasonID) 
tbl on s.SeasonID = tbl.SeasonID
group by s.SeasonID, s.Games, s.PlayoffGames, HistoricLoaded, CurrentLoaded
	 , PBoxRows
	 , TboxRows
	 , PbpRows
	 , StartingLineupsRows
	 , TboxLineupRows)
select SeasonID, Games, Loaded, 
	   Team, Arena, Player, Official, Game, PlayerBox, TeamBox, PlayByPlay, StartingLineups, TeamBoxLineups, HistoricLoaded, CurrentLoaded
	 , case when PBoxRows is null then 0 else PBoxRows end PBoxRows
	 , case when TboxRows is null then 0 else TboxRows end TboxRows
	 , case when PbpRows is null then 0 else PbpRows end PbpRows
	 , case when StartingLineupsRows is null then 0 else StartingLineupsRows end StartingLineupsRows
	 , case when TboxLineupRows is null then 0 else TboxLineupRows end TboxLineupRows
	 , case when Games != Game then 'Missing Game(s)'
       else 
        case 
            when Games = PlayerBox and Games = TeamBox and Games = PlayByPlay and Games = StartingLineups and Games = TeamBoxLineups
                then 'Operational'
			when (Games = PlayerBox and Games = TeamBox and Games = StartingLineups and (Games = TeamBoxLineups or Games = TeamBoxLineups + 1)
			and Games = PlayByPlay + 1 and SeasonID in(2006, 2007, 2003, 1998)) or
			(Games = PlayerBox and Games = TeamBox and Games = StartingLineups and Games = TeamBoxLineups + 2
			and Games = PlayByPlay + 2 and SeasonID = 1996) 
				then 'Operational*'
            else 'Missing Data from:' + 
              STUFF((case when Games != PlayerBox then ', PlayerBox' else '' end) +
                    (case when Games != TeamBox then ', TeamBox' else '' end) +
                    (case when Games != PlayByPlay then ', PlayByPlay' else '' end) +
                    (case when Games != StartingLineups then ', StartingLineups' else '' end) +
                    (case when Games != TeamBoxLineups then ', TeamBoxLineups' else '' end)
                    , 1, 2, '')
        end
  end Status
from Seasons s
Order by SeasonID desc
~~~



create procedure BuildLogCheck
as
Select case when max(BuildID) is null then 1 else max(BuildID) + 1 end BuildID
from util.BuildLog
~~~


create procedure BuildLogInsert
@BuildID int,
@Season int,
@Hr  int,
@Min int,
@Sec int,
@Ms  int,
@FullTime varchar(255),
@HrR  int,
@MinR int,
@SecR int,
@MsR  int,
@ReadTime varchar(255),
@HrI  int,
@MinI int,
@SecI int,
@MsI  int,
@InsertTime varchar(255),
@DatetimeStarted datetime,
@DatetimeComplete datetime,
@Historic int,
@Current int,
@Source varchar(30)
as
insert into util.BuildLog(
BuildID, 
SeasonID, 
Hr, 
Min, 
Sec, 
Ms, 
FullTime, 
HrR,
MinR,
SecR,
MsR,
ReadTime,
HrI,
MinI,
SecI,
MsI,
InsertTime, 
DatetimeStarted, 
DatetimeComplete,
Source) 
values(
@BuildID, 
@Season, 
@Hr, 
@Min, 
@Sec, 
@Ms, 
@FullTime, 
@HrR,
@MinR,
@SecR,
@MsR,
@ReadTime,
@HrI,
@MinI,
@SecI,
@MsI,
@InsertTime, 
@DatetimeStarted, 
@DatetimeComplete,
@Source);
update Season set HistoricLoaded = 1 where SeasonID = @Season and @Historic = 1;
update Season set CurrentLoaded  = 1 where SeasonID = @Season and @Current = 1;
exec sp_msforeachtable 'alter table ? with check check constraint all';
~~~


create procedure TeamBoxLineupCalc @SeasonID int, @GameID int
as
select 
concat('insert into TeamBoxLineups values(', p.SeasonID, ', ', p.GameID, ', ', p.TeamID, ', ', p.MatchupID, ', ''', 
case when p.Starter = 1 then 'Starters' else 'Bench' end, ''', ''', 'minutesplaceholder'', '
     , sum(p.Points)
     , ', '
     , sum(p.FG2M)
     , ', '
     , sum(p.FG2A)
     , ', fg2%, ' 
     , sum(p.FG3M) 
     , ', '
     , sum(p.FG3A) 
     , ', fg3%, '
     , sum(p.FGM) 
     , ', '
     , sum(p.FGA) 
     , ', fg%, ' 
     , sum(p.FTM) 
     , ', ' 
     , sum(p.FTA) 
     , ', ft%, ' 
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
	 , cast(cast(sum(p.MinutesCalculated) as decimal (18, 2))as varchar(10)) MinutesCalculated   
     , sum(p.FG2M) FG2M
     , sum(p.FG2A) FG2A
     , sum(p.FG3M) FG3M
     , sum(p.FG3A) FG3A
     , sum(p.FGM) FGM
     , sum(p.FGA) FGA
     , sum(p.FTM) FTM
     , sum(p.FTA) FTA
from PlayerBox p
where Status != 'INACTIVE' and p.SeasonID = @SeasonID and GameID = @GameID
group by  p.SeasonID, p.GameID, p.TeamID, p.MatchupID, p.Starter
order by GameID, p.TeamID
~~~

create procedure PlayByPlayCleanup
as
update PlayByPlay set TeamID = PlayerID, Tricode = (select Tricode from Team t where PlayerID = t.TeamID and t.SeasonID = PlayByPlay.SeasonID), PlayerID = null 
where PlayerID in((select distinct TeamID from Team t where t.SeasonID = PlayByPlay.SeasonID))
update PlayByPlay set OfficialID = PlayerID, PlayerID = null
where PlayerID in((select distinct OfficialID from Official o where o.SeasonID = PlayByPlay.SeasonID))
~~~


create procedure DeleteSeasonData @season int
AS
BEGIN
    set nocount on;  
    delete from StartingLineups with (tablock) where SeasonID = @season;
    delete from TeamBoxLineups with (tablock) where SeasonID = @season;
    delete from PlayerBox with (tablock) where SeasonID = @season;
    delete from TeamBox with (tablock) where SeasonID = @season;
    delete from GameExt with (tablock) where SeasonID = @season;
    delete from Game with (tablock) where SeasonID = @season;
    delete from Player with (tablock) where SeasonID = @season;
    delete from Official with (tablock) where SeasonID = @season;
    delete from Arena with (tablock) where SeasonID = @season;
    delete from Team with (tablock) where SeasonID = @season;
    delete from util.MissingData with (tablock) where SeasonID = @season;
    update Season set CurrentLoaded = 0, HistoricLoaded = 0 where SeasonID = @season
END
~~~




create procedure AlterTablesDeletePBP @season int
as
begin
   set nocount on;   
   exec sp_MSforeachtable 'alter table ? nocheck constraint all';   
   delete top(210000) from PlayByPlay with (tablock) where SeasonID = @season;
   delete top(210000) from PlayByPlay with (tablock) where SeasonID = @season;
   delete top(210000) from PlayByPlay with (tablock) where SeasonID = @season;
   delete top(210000) from PlayByPlay with (tablock) where SeasonID = @season;
end
~~~

create procedure TableKeysOff @SeasonID int
as
begin
    exec sp_MSforeachtable 'alter table ? nocheck constraint all';
    select 
        s.SeasonID,
        (select count(p.SeasonID) from PlayerBox p where p.seasonID = s.SeasonID) +
        (select count(p.SeasonID) from PlayByPlay p where p.seasonID = s.SeasonID) as PBPandBox
    from 
        Season s
    where s.SeasonID = @SeasonID;
end
~~~
create procedure TableKeysOn
as
begin
   set nocount on;   
   execute sp_MSforeachtable 'alter table ? check constraint all';
end
~~~

create procedure UpdateSeries1996
as
update Game set SeriesID = 
case 
when GameID in(
	49600006,
	49600014,
	49600023
) then '4960010'
when GameID in(
	49600002,
	49600012,
	49600019,
	49600026,
	49600036
) then '4960011'
when GameID in(
	49600001,
	49600010,
	49600017
) then '4960012'
when GameID in(
	49600005,
	49600015,
	49600038,
	49600020,
	49600030
) then '4960013'
when GameID in(
	49600004,
	49600011,
	49600018
) then '4960014'
when GameID in(
	49600007,
	49600016,
	49600022,
	49600029,
	49600034
) then '4960015'
when GameID in(
	49600003,
	49600009,
	49600021
) then '4960016'
when GameID in(
	49600008,
	49600013,
	49600024,
	49600032
) then '4960017'
--Semis
when GameID in(
	49600043,
	49600047,
	49600058,
	49600051,
	49600053
) then '4960020'
when GameID in(
	49600045,
	49600049,
	49600054,
	49600056,
	49600060,
	49600064,
	49600068
) then '4960021'
when GameID in(
	49600057,
	49600041,
	49600044,
	49600048,
	49600052
) then '4960022'
when GameID in(
	49600042,
	49600046,
	49600050,
	49600055,
	49600059,
	49600063,
	49600066
) then '4960023'
--Conf Finals
when GameID in(
	49600070,
	49600072,
	49600074,
	49600076,
	49600078
) then '4960030'
when GameID in(
	49600069,
	49600071,
	49600073,
	49600075,
	49600077,
	49600079
) then '4960031'
--Finals
when GameID in(
	49600083,
	49600084,
	49600088,
	49600085,
	49600086,
	49600087
) then '4960040'
else SeriesID end
where GameType = 'PS' and SeasonID = 1996
~~~

create procedure UpdateSeries1997
as
update Game set SeriesID = 
case 
--East First
when GameID in(
	49700006,
	49700015,
	49700023
) then '4970010'
when GameID in(
	49700005,
	49700013,
	49700036,
	49700019,
	49700026
) then '4970011'
when GameID in(
	49700002,
	49700009,
	49700017,
	49700027
) then '4970012'
when GameID in(
	49700001,
	49700011,
	49700020,
	49700031
) then '4970013'
--West First
when GameID in(
	49700003,
	49700012,
	49700039,
	49700025,
	49700032
) then '4970014'
when GameID in(
	49700007,
	49700016,
	49700021,
	49700028,
	49700033
) then '4970015'

when GameID in(
	49700008,
	49700014,
	49700022,
	49700029	
) then '4970016'
when GameID in(
	49700004,
	49700010,
	49700018,
	49700024
) then '4970017'

--Semis

when GameID in(
	49700041,
	49700045,
	49700049,
	49700055,
	49700057
) then '4970020'
when GameID in(
	49700043,
	49700047,
	49700051,
	49700053,
	49700060
) then '4970021'
when GameID in(
	49700044,
	49700048,
	49700052,
	49700056,
	49700058	
) then '4970022'
when GameID in(
	49700042,
	49700046,
	49700050,
	49700054,
	49700059	
) then '4970023'

--Conf Finals

when GameID in(
	49700070,
	49700072,
	49700074,
	49700076,
	49700078,
	49700080,
	49700082	
) then '4970030'
when GameID in(
	49700069,
	49700071,
	49700073,
	49700075	
) then '4970031'

--Finals
when GameID in(
	49700085,
	49700086,
	49700087,
	49700083,
	49700084,
	49700088
) then '4970040'
else SeriesID end
where GameType = 'PS' and SeasonID = 1997
~~~

create procedure UpdateSeries1998
as
update Game set SeriesID = 
case 
--East First
when GameID in(
	49800001,
	49800009,
	49800033,
	49800018,
	49800025
) then '4980010'
when GameID in(
	49800008,
	49800015,
	49800023
) then '4980011'
when GameID in(
	49800005,
	49800013,
	49800021,
	49800030
) then '4980012'
when GameID in(
	49800004,
	49800010,
	49800035,
	49800017,
	49800026
) then '4980013'
--West First
when GameID in(
	49800006,
	49800014,
	49800022,
	49800031
) then '4980014'
when GameID in(
	49800002,
	49800012,
	49800019
) then '4980015'

when GameID in(
	49800003,
	49800011,
	49800037,
	49800020,
	49800028
) then '4980016'
when GameID in(
	49800007,
	49800016,
	49800024,
	49800032
) then '4980017'

--Semis

when GameID in(
	49800041,
	49800045,
	49800049,
	49800052
) then '4980020'
when GameID in(
	49800043,
	49800047,
	49800053,
	49800056
) then '4980021'
when GameID in(
	49800042,
	49800046,
	49800051,
	49800054	
) then '4980022'
when GameID in(
	49800044,
	49800048,
	49800058,
	49800050,
	49800055,
	49800062
) then '4980023'

--Conf Finals

when GameID in(
	49800069,
	49800070,
	49800073,
	49800071,
	49800072,
	49800074
) then '4980030'
when GameID in(
	49800076,
	49800077,
	49800078,
	49800079
) then '4980031'

--Finals
when GameID in(
	49800085,
	49800086,
	49800087,
	49800083,
	49800084
) then '4980040'
else SeriesID end
where GameType = 'PS' and SeasonID = 1998
~~~

create procedure UpdateSeries1999
as
update Game set SeriesID = 
case 
--East First
when GameID in(
	49900008,
	49900015,
	49900021,
	49900025,
	49900033
) then '4990010'
when GameID in(
	49900001,
	49900011,
	49900018
) then '4990011'
when GameID in(
	49900005,
	49900013,
	49900022
) then '4990012'
when GameID in(
	49900004,
	49900009,
	49900017,
	49900026
) then '4990013'
--West First
when GameID in(
	49900007,
	49900016,
	49900024,
	49900028,
	49900039
) then '4990014'
when GameID in(
	49900003,
	49900010,
	49900020,
	49900032,
	49900040
) then '4990015'

when GameID in(
	49900006,
	49900014,
	49900023,
	49900029
) then '4990016'
when GameID in(
	49900002,
	49900012,
	49900019,
	49900030
) then '4990017'

--Semis

when GameID in(
	49900048,
	49900049,
	49900050,
	49900051,
	49900052,
	49900053
) then '4990020'
when GameID in(
	49900041,
	49900042,
	49900043,
	49900044,
	49900045,
	49900046,
	49900047
) then '4990021'
when GameID in(
	49900055,
	49900056,
	49900057,
	49900058,
	49900059
) then '4990022'
when GameID in(
	49900062,
	49900063,
	49900064,
	49900065,
	49900066
) then '4990023'

--Conf Finals

when GameID in(
	49900076,
	49900077,
	49900078,
	49900079,
	49900080,
	49900081
) then '4990030'
when GameID in(
	49900069,
	49900070,
	49900071,
	49900072,
	49900073,
	49900074,
	49900075
) then '4990031'

--Finals
when GameID in(
	49900085,
	49900086,
	49900087,
	49900083,
	49900084,
	49900088
) then '4990040'
else SeriesID end
where GameType = 'PS' and SeasonID = 1999
~~~

create procedure UpdateSeries2000
as
update Game set SeriesID = 
case 
--East First
when GameID in(
	40000004,
	40000011,
	40000021,
	40000031
) then '4000010'
when GameID in(
	40000006,
	40000013,
	40000020,
	40000029
) then '4000011'
when GameID in(
	40000003,
	40000009,
	40000017
) then '4000012'
when GameID in(
	40000008,
	40000016,
	40000024,
	40000032,
	40000040
) then '4000013'
--West First
when GameID in(
	40000002,
	40000010,
	40000019,
	40000025
) then '4000014'
when GameID in(
	40000005,
	40000015,
	40000022
) then '4000015'

when GameID in(
	40000007,
	40000014,
	40000023,
	40000030
) then '4000016'
when GameID in(
	40000001,
	40000012,
	40000018,
	40000028,
	40000034
) then '4000017'

--Semis

when GameID in(
	40000062,
	40000063,
	40000064,
	40000065,
	40000066,
	40000067,
	40000068
) then '4000020'
when GameID in(
	40000041,
	40000042,
	40000044,
	40000045,
	40000046,
	40000047,
	40000048
) then '4000021'
when GameID in(
	40000055,
	40000056,
	40000057,
	40000058,
	40000059
) then '4000022'
when GameID in(
	40000043,
	40000049,
	40000050,
	40000051
) then '4000023'

--Conf Finals

when GameID in(
	40000076,
	40000077,
	40000078,
	40000079,
	40000080,
	40000081,
	40000082
) then '4000030'
when GameID in(
	40000069,
	40000070,
	40000071,
	40000072
) then '4000031'

--Finals
when GameID in(
	40000085,
	40000086,
	40000087,
	40000083,
	40000084
) then '4000040'
else SeriesID end
where GameType = 'PS' and SeasonID = 2000
~~~

create procedure UpdateTeamBoxWinLoss
as
begin
    set nocount on;    
    update t set 
        Wins = (
            select sum(Win) 
            from teambox t2 inner join 
		            game g2 on t2.GameID = g2.GameID and t2.SeasonID = g2.SeasonID and (t2.TeamID = g2.HomeID or t2.TeamID = g2.AwayID)  
            where t.SeasonID = t2.SeasonID and g2.Date <= g.Date and left(g2.GameID, 1) != '6'
            and t.TeamID = t2.TeamID and g.GameType = g2.GameType and case when g.GameType = 'PS' and left(g.GameID, 1) = 4 then g.SeriesID else 0 end = case when g2.GameType = 'PS' and left(g2.GameID, 1) = 4 then g2.SeriesID else 0 end 
            ),
        Losses = (
            select sum(case when Win = 0 and g2.Date != cast(getdate() as date) then 1 else 0 end) 
            from teambox t2 inner join 
		            game g2 on t2.GameID = g2.GameID and t2.SeasonID = g2.SeasonID and (t2.TeamID = g2.HomeID or t2.TeamID = g2.AwayID)  
            where t.SeasonID = t2.SeasonID and g2.Date <= g.Date and left(g2.GameID, 1) != '6'
            and t.TeamID = t2.TeamID and g.GameType = g2.GameType and case when g.GameType = 'PS' and left(g.GameID, 1) = 4 then g.SeriesID else 0 end = case when g2.GameType = 'PS' and left(g2.GameID, 1) = 4 then g2.SeriesID else 0 end 
            )
    from TeamBox t inner join
            Game g on t.GameID = g.GameID and t.SeasonID = g.SeasonID and (t.TeamID = g.HomeID or t.TeamID = g.AwayID)  and left(g.GameID, 1) != '6'
end
~~~

create procedure UpdateTeamConfDiv
as
begin
   set nocount on;   
   update Team set 
       Conference = Case 
           when TeamID in(1610612737, 1610612738, 1610612741, 1610612739, 1610612748, 1610612749, 1610612751, 1610612752, 1610612753, 1610612754, 
                          1610612755, 1610612761, 1610612764, 1610612765, 1610612766) then 'East'
           when TeamID in(1610612742, 1610612743, 1610612744, 1610612745, 1610612746, 1610612747, 1610612750, 1610612756, 1610612757, 
                          1610612758, 1610612759, 1610612760, 1610612762, 1610612763) then 'West' 
           when TeamID = 1610612740 and SeasonID < 2004 then 'East'
           when TeamID = 1610612740 and SeasonID >= 2004 then 'West'
           else null end,
       Division = Case 
           when SeasonID >= 2004 and TeamID in(1610612738,1610612751,1610612752,1610612755,1610612761) then 'Atlantic'
           when SeasonID >= 2004 and TeamID in(1610612741,1610612739,1610612765,1610612754,1610612749) then 'Central'
           when SeasonID >= 2004 and TeamID in(1610612737, 1610612766, 1610612748, 1610612753, 1610612764) then 'Southeast'
           when SeasonID >= 2004 and TeamID in(1610612743, 1610612750, 1610612760, 1610612757, 1610612762) then 'Northwest'
           when SeasonID >= 2004 and TeamID in(1610612744, 1610612746, 1610612747, 1610612756, 1610612758) then 'Pacific'
           when SeasonID >= 2004 and TeamID in(1610612742, 1610612745, 1610612763, 1610612740, 1610612759) then 'Southwest' 
           when SeasonID < 2004 and TeamID in(1610612738, 1610612748, 1610612751, 1610612752, 1610612753, 1610612755, 1610612764) then 'Atlantic'
           when SeasonID < 2004 and TeamID in(1610612737, 1610612766, 1610612740, 1610612741, 1610612739, 1610612765, 1610612754, 1610612749, 1610612761) then 'Central'
           when SeasonID < 2004 and TeamID in(1610612742, 1610612743, 1610612745, 1610612750, 1610612759, 1610612763, 1610612762) then 'Midwest'
           when SeasonID < 2004 and TeamID in(1610612744, 1610612746, 1610612747, 1610612760, 1610612756, 1610612757, 1610612758) then 'Pacific' 
           else null end
end
~~~

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
~~~

create procedure GameExtLabels
as
begin
  set nocount on;   
  update e set 
      Label = case when left(right(g.GameID, 3), 1) = '1' then concat(t.Conference, ' - ', 'First Round')
                   when left(right(g.GameID, 3), 1) = '2' then concat(t.Conference, ' - ', 'Conf. Semifinals')
                   when left(right(g.GameID, 3), 1) = '3' then concat(t.Conference, ' - ', 'Conf. Finals')
                   else 'NBA Finals' end,
      LabelDetail = concat('Game ', right(g.GameID, 1))
  from GameExt e 
  inner join Game g on e.SeasonID = g.SeasonID and e.GameID = g.GameID
  inner join Team t on g.SeasonID = t.SeasonID and g.HomeID = t.TeamID
  where g.SeasonID >= 2019
  and (Label = '' or Label is null)
  and g.SeriesID is not null;
  update GameExt set Label = 'East Play-In' where GameID in(52000101, 52000111, 52000201,52100101, 52100111, 52100201, 52200101, 52200111, 52200201, 52300101, 52300111, 52300201);
  update GameExt set Label = 'West Play-In' where GameID in(52000121, 52000131, 52000211, 52100121, 52100131, 52100211, 52200121, 52200131, 52200211, 52300121, 52300131, 52300211);
  update GameExt set Label = 'NBA Mexico City Game' where GameID in(22300172, 22400147);
  update GameExt set Label = 'NBA Paris Games' where GameID in(22300527);
  update GameExt set Label = 'NBA Paris Games' where GameID in(22400621, 22400633);
  update GameExt set Label = 'Emirates NBA Cup', LabelDetail = 'Championship' where GameID in(62300001, 62400001);
  update GameExt set Label = 'Emirates NBA Cup', LabelDetail = 'East Group A' where GameID in(22300001, 22300008, 22300018, 22300019, 22300029, 22300030, 22300039, 22300040, 22300047, 22300055, 
  22400003, 22400004, 22400010, 22400014, 22400022, 22400027, 22400040, 22400043, 22400050, 22400053);
  update GameExt set Label = 'Emirates NBA Cup', LabelDetail = 'East Group B' where GameID in(22300002, 22300003, 22300009, 22300017, 22300027, 22300028, 22300045, 22300049, 22300056, 22300057,
  22400002, 22400005, 22400009, 22400011, 22400029, 22400036, 22400045, 22400046, 22400052, 22400054);
  update GameExt set Label = 'Emirates NBA Cup', LabelDetail = 'East Group C' where GameID in(22300004, 22300010, 22300020, 22300031, 22300033, 22300038, 22300043, 22300046, 22300053, 22300054, 
  22400001, 22400012, 22400013, 22400021, 22400028, 22400030, 22400035, 22400041, 22400047, 22400051);
  update GameExt set Label = 'Emirates NBA Cup', LabelDetail = 'East Quarterfinal' where right(GameID, 4) in('1201', '1202') and SeasonID >= 2023;
  update GameExt set Label = 'Emirates NBA Cup', LabelDetail = 'East Semifinal' where right(GameID, 4) = '1229' and SeasonID >= 2023;
  update GameExt set Label = 'Emirates NBA Cup', LabelDetail = 'West Group A' where GameID in(22300007, 22300012, 22300015, 22300023, 22300026, 22300035, 22300036, 22300041, 22300042, 22300044,
  22400008, 22400016, 22400020, 22400031, 22400034, 22400037, 22400044, 22400049, 22400059, 22400060);
  update GameExt set Label = 'Emirates NBA Cup', LabelDetail = 'West Group B' where GameID in(22300006, 22300011, 22300014, 22300022, 22300024, 22300034, 22300037, 22300048, 22300052, 22300059,
  22400006, 22400015, 22400018, 22400025, 22400026, 22400038, 22400039, 22400048, 22400055, 22400057);
  update GameExt set Label = 'Emirates NBA Cup', LabelDetail = 'West Group C' where GameID in(22300005, 22300013, 22300016, 22300021, 22300025, 22300032, 22300050, 22300051, 22300058, 22300060, 
  22400007, 22400017, 22400019, 22400023, 22400024, 22400032, 22400033, 22400042, 22400056, 22400058);
  update GameExt set Label = 'Emirates NBA Cup', LabelDetail = 'West Quarterfinal' where right(GameID, 4) in('1203', '1204')and SeasonID >= 2023;
  update GameExt set Label = 'Emirates NBA Cup', LabelDetail = 'West Semifinal' where right(GameID, 4) = '1230'and SeasonID >= 2023;
  update GameExt set Label = 'SoFi Play-In Tournament', LabelDetail = 'East' where GameID in(52400101, 52400111, 52400201);
  update GameExt set Label = 'SoFi Play-In Tournament', LabelDetail = 'West' where GameID in(52400121, 52400131, 52400211);
end
~~~


--Find bad PlayByPlay games
create procedure FindPbpRepairs @SeasonID int
as
--Get the last ActionID for each Game
with GameLastAction as(
select p.SeasonID, p.GameID, max(ActionID) MaxAction
from PlayByPlay p 
where SeasonID = @SeasonID
group by p.SeasonID, p.GameID),
--Get the First ActionID for each Game. Should be 1, but just to make sure.
GameFirstAction as(
select p.SeasonID, p.GameID, min(ActionID) MinAction
from PlayByPlay p 
where SeasonID = @SeasonID
group by p.SeasonID, p.GameID)
--Using the last (max) ActionID, look for any games where the Clock doesnt hit all zeros
select g.SeasonID, g.GameID, g.MaxAction Action, p.Qtr, p.Clock
from GameLastAction g
inner join PlayByPlay p on g.SeasonID = p.SeasonID and g.GameID = p.GameID and g.MaxAction = p.ActionID
where p.Clock != '00:00.00' and g.SeasonID = @SeasonID
union
--Using the first (min) ActionID, look for any games where the Clock doesnt start at 12:00.00
select g.SeasonID, g.GameID, g.MinAction Action, p.Qtr, p.Clock
from GameFirstAction g
inner join PlayByPlay p on g.SeasonID = p.SeasonID and g.GameID = p.GameID and g.MinAction = p.ActionID
where p.Clock != '12:00.00' and g.SeasonID = @SeasonID
union
--Make sure we get any games where the count of Actions doesn't equal the last ActionID. Make sure we arent getting the games from GameFirstAction too
select p.SeasonID, p.GameID, count(p.ActionID) Actions, 1 Qtr, 'Placehold' Clock
from PlayByPlay p
left join GameFirstAction g on p.SeasonID = g.SeasonID and p.GameID = g.GameID
where p.GameID != g.GameID and p.SeasonID = @SeasonID
group by p.SeasonID, p.GameID
having count(p.ActionID) < max(p.ActionID)
~~~
*/


