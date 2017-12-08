#ifndef FFSTATS_PLAYOFFPROB_H
#define FFSTATS_PLAYOFFPROB_H

#ifdef FFSTATS_PLAYOFFPROB_EXPORTS
#define FFSTATS_PLAYOFFPROB_API __declspec(dllexport)
#else
#define FFSTATS_PLAYOFFPROB_API __declspec(dllimport)
#endif

#include "typedefs.h"

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
    ApiHead2HeadRecord h2h_records[NUM_TEAMS - 1];
  };

  struct ApiStandings {
    int week;
    ApiTeamRecord records[NUM_TEAMS];
  };

  // schedule input
  struct ApiGame {
    int team1;
    int team2;
  };

  struct ApiScheduleWeek {
    int week;
    ApiGame games[NUM_TEAMS / 2];
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
    ApiPlayoffProb playoff_probs[NUM_TEAMS];
  };

  FFSTATS_PLAYOFFPROB_API void Calculate(const ApiStandings& api_standings,
                                         const ApiSchedule& api_schedule,
                                         ApiPlayoffProbOutput* api_playoff_probs);

}

#endif
