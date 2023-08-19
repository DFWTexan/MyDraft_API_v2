using MyDraftAPI_v2.FantasyDataModel;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using static MyDraftAPI_v2.FantasyDataModel.FantasyLeage;

namespace DraftService

{
    public class DraftPickGenerator
    {
        public static IList<DraftPick> generateDraftPicks(FantasyLeague league)
        {
            FantasyLeague.DraftOrderType draftType = league.draftOrderType;

            if (draftType == FantasyLeague.DraftOrderType.auction)
                return null;

            List<FantasyTeam> teams = new List<FantasyTeam>(league.teams);
            teams.Sort(delegate (FantasyTeam team1, FantasyTeam team2)
            {
                return (int)(team1.draftPosition - team2.draftPosition);
            });

            int numberOfTeams = teams.Count;
            int rounds = league.rounds;
            int totalPicks = rounds * numberOfTeams;
            IList<DraftPick> draftPicks = new List<DraftPick>(totalPicks);

            for (int overall = 1; overall <= totalPicks; overall++)
            {
                int round = roundForOverall(overall, numberOfTeams);
                int teamIndex = -1;
                switch (draftType)
                {
                    case FantasyLeague.DraftOrderType.snake:
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
                    case FantasyLeague.DraftOrderType.straight:
                        teamIndex = (overall - 1) % numberOfTeams;
                        break;
                    case FantasyLeague.DraftOrderType.thirdRoundReversal:
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
                    case FantasyLeague.DraftOrderType.thirdRoundFlip:
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

                FantasyTeam team = teams[teamIndex];
                int pickInRound = pickInRoundForOverallPick(overall, numberOfTeams);
                DraftPick draftPick = new DraftPick(league.identifier, overall, round, pickInRound, team.identifier, null, 0, false);
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
