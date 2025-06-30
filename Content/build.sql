


create table Season(
SeasonID			int,
ChampionID			int,
Games				int,
PlayoffGames		int,
HistoricLoaded		int,
CurrentLoaded		int,
Primary Key(SeasonID))

create table Team(
SeasonID			int,
TeamID				int,
City				varchar(255),
Name				varchar(255),
Tricode				varchar(255),
Wins				int,
Losses				int,
FullName			varchar(255),
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
select SeasonID, Game, Loaded, 
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
*/


