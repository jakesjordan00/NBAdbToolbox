


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
Label				varchar(100),
LabelDetail			varchar(100),
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
OfficialID			int,
Official2ID			int,
Official3ID			int,
OfficialAlternateID int,
Status				varchar(50),
Attendance			int,
Sellout				int,
Primary Key(SeasonID, GameID),
Foreign Key (SeasonID) references Season(SeasonID),
Foreign Key (SeasonID, GameID) references Game(SeasonID, GameID),
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
insert into Season values(2023, 1610612738, 1230, 88, 0, 0)
insert into Season values(2024, null, 1230, 62, 0, 0)
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
select SeasonID, Games, PlayoffGames, HistoricLoaded, CurrentLoaded, (select count(p.SeasonID) Rows from PlayerBox p where p.seasonID = s.SeasonID) +
(select count(p.SeasonID) Rows from PlayByPlay p where p.seasonID = s.SeasonID) PBPandBox
from Season s
order by SeasonID desc
~~~



create procedure TeamCheck @TeamID int, @SeasonID int
as
select TeamID
from Team
where TeamID = @TeamID and SeasonID = @SeasonID
~~~


create procedure TeamUpdate @TeamID int, @SeasonID int, @W int, @L int
as
update Team set Wins = @W, Losses = @L where TeamID = @teamID and SeasonID = @SeasonID
~~~

create procedure ArenaCheck @ArenaID int, @SeasonID int
as
select ArenaID, (select count(ArenaID) from Arena where SeasonID = @SeasonID) Arenas
from Arena
where ArenaID = @ArenaID and SeasonID = @SeasonID
~~~


create procedure OfficialCheck @OfficialID int, @SeasonID int
as
select OfficialID, (select count(OfficialID) from Official where SeasonID = @SeasonID) Officials
from Official
where OfficialID = @OfficialID and SeasonID = @SeasonID
~~~



create procedure PlayerCheck @PlayerID	int, @SeasonID int
as
select * from Player where PlayerID = @PlayerID and SeasonID = @SeasonID
~~~

create procedure PlayerUpdate 
@SeasonID			int,
@PlayerID			int,
@Name				varchar(255),
@Number				varchar(3),
@Position			varchar(100)
as
update Player set Name = @Name, Number = @Number, Position = @Position
where PlayerID = @PlayerID and SeasonID = @SeasonID
~~~





create procedure InactiveInsert
@SeasonID			int,
@PlayerID			int,
@Name				varchar(255)
as
Insert into Player(SeasonID, PlayerID, Name) values(
@SeasonID,
@PlayerID,
@Name)
~~~

create procedure GameCheck @SeasonID int, @GameID int
as
select *
from Game g
where g.SeasonID = @SeasonID and g.GameID = @GameID
~~~


create procedure GameUpdate @SeasonID int, @GameID int, @HomeID int, @AwayID int, @HScore int, @AScore int,
@WinnerID int, @LoserID int, @WScore int, @LScore int
as
update Game set 
HomeID = @HomeID, AwayID = @AwayID, HScore = @HScore, AScore = @AScore,
WinnerID = @WinnerID, LoserID = @LoserID, WScore = @WScore, LScore = @LScore
where GameID = @GameID and SeasonID = @SeasonID
~~~

create procedure TeamBoxCheck 
@SeasonID      int,
@GameID        int,
@TeamID        int,
@MatchupID     int
as
select SeasonID, GameID, TeamID, Points, PointsAgainst
from TeamBox t
where t.SeasonID = @SeasonID and t.GameID = @GameID and t.TeamID = @TeamID and t.MatchupID = @MatchupID
~~~




create procedure TeamBoxLineupCheck 
@SeasonID      int,
@GameID        int,
@TeamID        int,
@MatchupID     int,
@Unit			varchar(30)
as
select SeasonID, GameID, TeamID, Unit, Points
from TeamBoxLineups t
where t.SeasonID = @SeasonID and t.GameID = @GameID and t.TeamID = @TeamID and t.MatchupID = @MatchupID and t.Unit = @Unit
~~~

create procedure PlayerBoxCheck 
@SeasonID      int,
@GameID        int,
@TeamID        int,
@MatchupID     int,
@PlayerID      int
as
select SeasonID, GameID, TeamID, PlayerID, Minutes
from PlayerBox p
where p.SeasonID = @SeasonID and p.GameID = @GameID and p.TeamID = @TeamID and p.MatchupID = @MatchupID and p.PlayerID = @PlayerID
~~~



create procedure PlayerBoxUpdateHistoric 
@SeasonID int, @GameID int, @TeamID int, @PlayerID int, @FGM int, @FGA int, @FGpct float, @FG2M int, @FG2A int, @FG2pct float, 
@FG3M int, @FG3A int, @FG3pct float, @FTM int, @FTA int, @FTpct float, @RebD int, @RebO int, @RebT int, @Assists int, 
@Turnovers int, @AtoR float, @Steals int, @Blocks int, @Points int, @FoulsPersonal int, @Minutes varchar(30)
as
update PlayerBox set 
FGM					=	@FGM           ,
FGA					=	@FGA           ,
[FG%]				=	@FGpct         ,
FG2M				=	@FG2M          ,
FG2A				=	@FG2A          ,
[FG2%]				=	@FG2pct        ,
FG3M				=	@FG3M          ,
FG3A				=	@FG3A          ,
[FG3%]				=	@FG3pct        ,
FTM					=	@FTM           ,
FTA					=	@FTA           ,
[FT%]				=	@FTpct         ,
ReboundsDefensive   =	@RebD          ,
ReboundsOffensive   =	@RebO          ,
ReboundsTotal       =	@RebT          ,
Assists				=	@Assists       ,
Turnovers			=	@Turnovers     ,
AssistsTurnoverRatio=	@AtoR          ,
Steals				=	@Steals        ,
Blocks				=	@Blocks        ,
Points				=	@Points        ,
FoulsPersonal		=	@FoulsPersonal ,
Minutes				=	@Minutes
where SeasonID = @SeasonID and GameID = @GameID and TeamID = @TeamID and PlayerID = @PlayerID
~~~


create procedure PlayByPlayCheckHistorical @SeasonID int, @GameID int
as
select p.SeasonID, p.GameID, count(p.GameID) Actions
from PlayByPlay p
where p.SeasonID = @SeasonID and p.GameID = @GameID
group by p.SeasonID, p.GameID
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
@Source)
update Season set HistoricLoaded = 1 where SeasonID = @Season and @Historic = 1
update Season set CurrentLoaded  = 1 where SeasonID = @Season and @Current = 1
~~~





create procedure NewPlayerCheckHistorical @PlayerID int, @SeasonID int, @TeamID int, @GameID int	   
as																									   
select p.PlayerID, b.minutes, s.Position																 
from Player p left join
		PlayerBox b on p.PlayerID = b.PlayerID and p.SeasonID = b.SeasonID left join
		StartingLineups s on b.PlayerID = s.PlayerID and b.GameID = s.GameID and b.TeamID = s.TeamID and b.MatchupID = s.MatchupID and b.SeasonID = s.SeasonID
where p.PlayerID = @PlayerID and p.SeasonID = @SeasonID and ((b.GameID = @GameID and b.TeamID = @TeamID) or b.PlayerID is null or s.PlayerID is null)
~~~


create procedure PlayByPlayCleanup
as
update PlayByPlay set TeamID = PlayerID, Tricode = (select Tricode from Team t where PlayerID = t.TeamID and t.SeasonID = PlayByPlay.SeasonID), PlayerID = null 
where PlayerID in((select distinct TeamID from Team t where t.SeasonID = PlayByPlay.SeasonID))
update PlayByPlay set OfficialID = PlayerID, PlayerID = null
where PlayerID in((select distinct OfficialID from Official o where o.SeasonID = PlayByPlay.SeasonID))
~~~

create procedure SelectGamesDeletePBP @Season int
as
select distinct GameID from Game where SeasonID = @Season 
delete from PlayByPlay where SeasonID = @Season
~~~

create procedure DeleteBeforePopulate @SeasonID int
as
delete from util.MissingData	where SeasonID = @SeasonID
delete from StartingLineups		where SeasonID = @SeasonID
delete from TeamBoxLineups		where SeasonID = @SeasonID
delete from PlayByPlay			where SeasonID = @SeasonID
delete from PlayerBox			where SeasonID = @SeasonID
delete from TeamBox				where SeasonID = @SeasonID
delete from GameExt				where SeasonID = @SeasonID
delete from Game				where SeasonID = @SeasonID
delete from Player				where SeasonID = @SeasonID
delete from Official			where SeasonID = @SeasonID
delete from Arena				where SeasonID = @SeasonID
delete from Team				where SeasonID = @SeasonID
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
    execute sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL';
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


*/


