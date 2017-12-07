#ifndef FFSTATS_PLAYOFFPROB_H
#define FFSTATS_PLAYOFFPROB_H

#ifdef FFSTATS_PLAYOFFPROB_EXPORTS
#define FFSTATS_PLAYOFFPROB_API __declspec(dllexport)
#else
#define FFSTATS_PLAYOFFPROB_API __declspec(dllimport)
#endif

extern "C" {

  // standings input
  struct ApiHead2HeadRecord {
    int opponent;
    int win;
    int loss;
  };

  struct ApiTeamRecord {
    int team;
    int win;
    int loss;
    ApiHead2HeadRecord h2h_records[7];
  };

  struct ApiStandings {
    int week;
    ApiTeamRecord records[8];
  };

  // schedule input
  struct ApiGame {
    int team1;
    int team2;
  };

  struct ApiScheduleWeek {
    int week;
    ApiGame games[4];
  };

  struct ApiSchedule {
    ApiScheduleWeek weeks[14];
  };

  // output
  struct ApiPlayoffProb {
    int team;
    double excl_tiebreakers;
    double incl_tiebreakers;
  };

  struct ApiPlayoffProbOutput {
    ApiPlayoffProb playoff_probs[8];
  };

  FFSTATS_PLAYOFFPROB_API void Calculate(const ApiStandings& api_standings,
                                         const ApiSchedule& api_schedule,
                                         ApiPlayoffProbOutput* api_playoff_probs);

}

#endif
