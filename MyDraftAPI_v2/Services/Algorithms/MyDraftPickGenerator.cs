using Database.Model;
using DbData;
using MyDraftAPI_v2.FantasyDataModel;

namespace DraftService
{
    public class MyDraftPickGenerator
    {
        private readonly AppDataContext _db;

        public MyDraftPickGenerator(AppDataContext db) { _db = db; }

        public static IList<MyDraftPick> generateDraftPicks(MyFantasyLeague league)
        {
            MyFantasyLeague.DraftOrderType draftType = league.draftOrderType;

            if (draftType == MyFantasyLeague.DraftOrderType.auction)
                return null;

            List<UserLeagueTeam> teams = new List<UserLeagueTeam>(league.teams);

            teams.Sort(delegate (UserLeagueTeam team1, UserLeagueTeam team2)
            {
                return (int)(team1.DraftPosition - team2.DraftPosition);
            });

            int numberOfTeams = teams.Count;
            int rounds = league.rounds;
            int totalPicks = rounds * numberOfTeams;
            IList<MyDraftPick> draftPicks = new List<MyDraftPick>(totalPicks);

            for (int overall = 1; overall <= totalPicks; overall++)
            {
                int round = roundForOverall(overall, numberOfTeams);
                int teamIndex = -1;
                switch (draftType)
                {
                    case MyFantasyLeague.DraftOrderType.snake:
                        {
                            if (round % 2 != 0)
                            {
                                teamIndex = (overall - 1) % numberOfTeams;
                            }
                            else
                            {
                                int remainder = overall % numberOfTeams;
                                if (remainder > 0)
                                {
                                    teamIndex = numberOfTeams - remainder;
                                }
                                else
                                {
                                    teamIndex = 0;
                                }
                            }
                            break;
                        }
                    case MyFantasyLeague.DraftOrderType.straight:
                        teamIndex = (overall - 1) % numberOfTeams;
                        break;
                    case MyFantasyLeague.DraftOrderType.thirdRoundReversal:
                        if (round < 3)
                        {
                            if (round % 2 != 0)
                            {
                                teamIndex = (overall - 1) % numberOfTeams;
                            }
                            else
                            {
                                int remainder = overall % numberOfTeams;
                                if (remainder > 0)
                                {
                                    teamIndex = numberOfTeams - remainder;
                                }
                                else
                                {
                                    teamIndex = 0;
                                }
                            }
                        }
                        else
                        {
                            if (round % 2 != 0)
                            {
                                int remainder = overall % numberOfTeams;
                                if (remainder > 0)
                                {
                                    teamIndex = numberOfTeams - remainder;
                                }
                                else
                                {
                                    teamIndex = 0;
                                }
                            }
                            else
                            {
                                teamIndex = (overall - 1) % numberOfTeams;
                            }
                        }
                        break;
                    case MyFantasyLeague.DraftOrderType.thirdRoundFlip:
                        if (round < 3)
                        {
                            if (round % 2 != 0)
                            {
                                teamIndex = (overall - 1) % numberOfTeams;
                            }
                            else
                            {
                                int remainder = overall % numberOfTeams;
                                if (remainder > 0)
                                {
                                    teamIndex = numberOfTeams - remainder;
                                }
                                else
                                {
                                    teamIndex = 0;
                                }
                            }
                        }
                        else
                        {
                            if (round % 2 != 0)
                            {
                                int remainder = overall % numberOfTeams;
                                if (remainder > 0)
                                {
                                    teamIndex = numberOfTeams - remainder;
                                }
                                else
                                {
                                    teamIndex = 0;
                                }
                            }
                            else
                            {
                                teamIndex = (overall - 1) % numberOfTeams;
                            }
                        }
                        break;
                    default:
                        break;
                }

                UserLeagueTeam team = teams[teamIndex];
                int pickInRound = pickInRoundForOverallPick(overall, numberOfTeams);
                MyDraftPick draftPick = new MyDraftPick(league.identifier, overall, round, pickInRound, team.ID, 0, 0, false);
                draftPick.league = league;
                draftPicks.Add(draftPick);
            }

            return draftPicks;
        }

        private static int roundForOverall(int overall, int numTeams)
        {
            int round = -1;

            if (overall <= numTeams)
            {
                round = 1;
            }
            else if (overall % numTeams == 0)
            {
                round = overall / numTeams;
            }
            else
            {
                round = overall / numTeams + 1;
            }

            return round;
        }

        private static int pickInRoundForOverallPick(int overallPick, int numTeams)
        {
            return (overallPick - 1) % numTeams + 1;
        }

        public static IList<int> overallPicksInRound(int round, int numTeams)
        {
            IList<int> results = new List<int>(numTeams);
            int firstPickInRound = (round - 1) * numTeams + 1;
            for (int i = 0; i < numTeams; i++)
            {
                int pickOverall = firstPickInRound + i;
                results.Add(pickOverall);
            }

            return results;
        }
    }
}
