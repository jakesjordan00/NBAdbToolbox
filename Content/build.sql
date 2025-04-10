
create table Season(
SeasonID			int,
ChampionID			int,
Games				int,
PlayoffGames		int,
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
PlayerID					int,
Status						varchar(20),
Starter						int,
Position					varchar(2),
Minutes						varchar(30),
MinutesCalculated			varchar(30),
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
Primary Key(SeasonID, GameID, TeamID, PlayerID),
Foreign Key (SeasonID) references Season(SeasonID),
Foreign Key (SeasonID, GameID) references Game(SeasonID, GameID),
Foreign Key (SeasonID, TeamID) references Team(SeasonID, TeamID),
Foreign Key (SeasonID, PlayerID) references Player(SeasonID, PlayerID))

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
EventMsgTypeID		int,
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
ReboundsTeamDefensive		int,
ReboundsTeamOffensive		int,
ReboundsTotal				int,
Assists						int,
Steals						int,
Turnovers					int,
Blocks						int,
FoulsPersonal				int,
Primary Key (SeasonID, GameID, TeamID, MatchupID, Unit),
Foreign Key (SeasonID) references Season(SeasonID),
Foreign Key (SeasonID, GameID) references Game(SeasonID, GameID),
Foreign Key (SeasonID, TeamID) references Team(SeasonID, TeamID),
Foreign Key (SeasonID, MatchupID) references Team(SeasonID, TeamID))






/*
--The below section will be used to create a new string for procedure creation
~create procedure SeasonInsert
as
insert into Season values(2012, 1610612748, 1229, 85)
insert into Season values(2013, 1610612759, 1230, 89)
insert into Season values(2014, 1610612744, 1230, 81)
insert into Season values(2015, 1610612739, 1230, 86)
insert into Season values(2016, 1610612744, 1230, 79)
insert into Season values(2017, 1610612744, 1230, 81)
insert into Season values(2018, 1610612761, 1230, 82)
insert into Season values(2019, 1610612747, 1059, 83)
insert into Season values(2020, 1610612749, 1080, 85)
insert into Season values(2021, 1610612744, 1230, 87)
insert into Season values(2022, 1610612743, 1230, 84)
insert into Season values(2023, 1610612738, 1230, 82)
insert into Season values(2024, null, 1131, 0)
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
select SeasonID, Games, PlayoffGames
from Season
order by SeasonID desc
~~~



create procedure TeamCheck @TeamID int, @SeasonID int
as
select TeamID, (select count(TeamID) from Team where SeasonID = @SeasonID) Teams
from Team
where TeamID = @TeamID and SeasonID = @SeasonID
~~~


create procedure TeamUpdate @TeamID int, @SeasonID int, @W int, @L int
as
update Team set Wins = @W, Losses = @L where TeamID = @teamID and SeasonID = @SeasonID
~~~

create procedure TeamInsert
@SeasonID			int,
@TeamID				int,
@City				varchar(255),
@Name				varchar(255),
@Tricode			varchar(255),
@Wins				int,
@Losses				int
as
insert into Team values(
@SeasonID			,
@TeamID				,
@City				,
@Name				,
@Tricode			,
@Wins				,
@Losses				,
concat('(', @Tricode, ') ', @City, ' ', @Name))
~~~


create procedure ArenaCheck @ArenaID int, @SeasonID int
as
select ArenaID, (select count(ArenaID) from Arena where SeasonID = @SeasonID) Arenas
from Arena
where ArenaID = @ArenaID and SeasonID = @SeasonID
~~~

create procedure ArenaInsert 
@SeasonID			int,
@ArenaID			int,
@TeamID				int,
@City				varchar(255),
@Country			varchar(255),
@Name				varchar(255),
@PostalCode			varchar(255),
@State				varchar(255),
@StreetAddress		varchar(255),
@Timezone			varchar(255)
as
insert into Arena values(
@SeasonID			,
@ArenaID			,
@TeamID				,
@City				,
@Country			,
@Name				,
@PostalCode			,
@State				,
@StreetAddress		,
@Timezone			)
~~~

create procedure OfficialCheck @OfficialID int, @SeasonID int
as
select OfficialID, (select count(OfficialID) from Official where SeasonID = @SeasonID) Officials
from Official
where OfficialID = @OfficialID and SeasonID = @SeasonID
~~~

create procedure OfficialInsert
@SeasonID			int,
@OfficialID			int,
@Name				varchar(255),
@Number				varchar(3)
as
insert into Official values(
@SeasonID			,
@OfficialID			,
@Name				,
@Number				)
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





create procedure PlayerInsert
@SeasonID			int,
@PlayerID			int,
@Name				varchar(255),
@Number				varchar(3),
@Position			varchar(100)
as
insert into Player values(
@SeasonID			,
@PlayerID			,
@Name				,
@Number				,
@Position			)
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

create procedure GameInsert
@SeasonID			int,
@GameID				int,
@Date				date,
@GameType			varchar(10),
@HomeID				int,
@HScore				int,
@AwayID				int,
@AScore				int,
@WinnerID			int,
@WScore				int,
@LoserID			int,
@LScore				int,
@SeriesID			varchar(20),
@Label				varchar(100),
@LabelDetail		varchar(100),
@Datetime			datetime
as
insert into Game values(
@SeasonID			,
@GameID				,
@Date				,
@GameType			,
@HomeID				,
@HScore				,
@AwayID				,
@AScore				,
@WinnerID			,
@WScore				,
@LoserID			,
@LScore				,
@SeriesID			,
@Label				,
@LabelDetail		,
@Datetime			)
~~~

create procedure TeamBoxInsert
@SeasonID      int,
@GameID        int,
@TeamID        int,
@MatchupID     int,
@FGM           int,
@FGA           int,
@FGpct         float,
@FG2M          int,
@FG2A          int,
@FG2pct        float,
@FG3M          int,
@FG3A          int,
@FG3pct        float,
@FTM           int,
@FTA           int,
@FTpct         float,
@RebD          int,
@RebO          int,
@RebT          int,
@Assists       int,
@Turnovers     int,
@AtoR          float,
@Steals        int,
@Blocks        int,
@Points        int,
@PointsAgainst int,
@FoulsPersonal int
as
insert into TeamBox(SeasonID, GameID, TeamID, MatchupID, FGM, FGA, [FG%], FG2M, FG2A, [FG2%], FG3M, FG3A, [FG3%], FTM, FTA, [FT%], RebD, RebO, RebT, Assists, Turnovers, AtoR, Steals, Blocks, Points, PointsAgainst, FoulsPersonal)
values(
@SeasonID,
@GameID,
@TeamID,
@MatchupID,
@FGM,
@FGA,
@FGpct,
@FG2M,
@FG2A,
@FG2pct,
@FG3M,
@FG3A,
@FG3pct,
@FTM,
@FTA,
@FTpct,
@RebD,
@RebO,
@RebT,
@Assists,
@Turnovers,
@AtoR,
@Steals,
@Blocks,
@Points,
@PointsAgainst,
@FoulsPersonal)
~~~

*/