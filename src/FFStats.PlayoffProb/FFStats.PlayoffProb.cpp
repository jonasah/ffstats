// FFStats.PlayoffProb.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#include "FFStats.PlayoffProb.h"
#include "playoffprobcalculator.h"

//#define MAX_WEEKS_TO_SIMULATE 4

extern "C" {

  void Calculate(const ApiStandings& api_standings, 
                 const ApiSchedule& api_schedule,
                 ApiPlayoffProbOutput* api_playoff_probs)
  {
    using namespace ffstats::playoffprob;

    auto standings = Standings{};
    auto schedule = Schedule{};

    // create temporary zero-based team id during calculations
    {
      auto temp_team = team_t{0};

      for (const auto& record : api_standings.records) {
        TeamInfo::addConversion(record.team, temp_team);

        ++temp_team;
      }
    }

    // convert standings struct
    {
      standings.week = api_standings.week;

      auto idx = 0U;

      for (const auto& record : api_standings.records) {
        Q_ASSERT(TeamInfo::exists(record.team));

        const auto temp_team = TeamInfo::getTemporaryId(record.team);

        standings.records[idx].team = temp_team;
        standings.records[idx].win = record.win;
        standings.records[idx].loss = record.loss;

        for (const auto& h2h_record : record.h2h_records) {
          Q_ASSERT(TeamInfo::exists(h2h_record.opponent));

          const auto temp_opponent = TeamInfo::getTemporaryId(h2h_record.opponent);

          standings.records[idx].setHead2HeadRecord(temp_opponent, h2h_record.win - h2h_record.loss);
        }

        ++idx;
      }
    }

    // convert schedule struct
    {
      for (const auto& week : api_schedule.weeks) {
        if (week.week <= standings.week) {
          continue;
        }

        auto games = QVector<Game>{};

        for (const auto& game : week.games) {
          Q_ASSERT(TeamInfo::exists(game.team1));
          Q_ASSERT(TeamInfo::exists(game.team2));

          games.append({
            TeamInfo::getTemporaryId(game.team1),
            TeamInfo::getTemporaryId(game.team2)
          });
        }

        schedule.insert(week.week, games);

#ifdef MAX_WEEKS_TO_SIMULATE
        static auto counter = 0;
        ++counter;

        if (counter == MAX_WEEKS_TO_SIMULATE) {
          break;
        }
#endif
      }
    }

    auto playoff_probs = ffstats::playoffprob::PlayoffProbCalculator::calculate(standings, schedule);

    // convert playoff prob output
    {
      auto idx = 0U;

      for (const auto playoff_prob : playoff_probs) {
        api_playoff_probs->playoff_probs[idx].team = playoff_prob.team;
        api_playoff_probs->playoff_probs[idx].excl_tiebreakers = playoff_prob.excl_tiebreakers;
        api_playoff_probs->playoff_probs[idx].incl_tiebreakers = playoff_prob.incl_tiebreakers;

        ++idx;
      }
    }
  }

}
