#ifndef PLAYOFF_PROB_CALCULATOR
#define PLAYOFF_PROB_CALCULATOR

#include "analyzer.h"
#include "math.h"

//#define TIME_MEASURE

#ifdef TIME_MEASURE
#include <chrono>
#endif

namespace ffstats::playoffprob {

  class PlayoffProbCalculator {
  public:
    static QVector<PlayoffProbability> calculate(const Standings& start_standings, const Schedule& schedule) {
      Analyzer analyzer;

#ifdef TIME_MEASURE
      const auto t0 = std::chrono::steady_clock::now();
#endif

      generateOutcomes(schedule, start_standings, analyzer);

#ifdef TIME_MEASURE
      const auto t1 = std::chrono::steady_clock::now();
#endif

      auto summary = analyzer.summarize();

#ifdef TIME_MEASURE
      const auto nsecs = std::chrono::nanoseconds{ t1 - t0 }.count();
      std::cout << "Elapsed time: " << (nsecs / 1e9) << " s\n";
#endif

      return summary;
    }

  private:
    // generate all possible results for a single game
    static QVector<GameResult> generateGameResults(const Game& game) {
      return { { game.first, game.second }, { game.second, game.first } };
    }

    // generate all possible results for a single week
    static QVector<WeekResults> generateWeekResults(const QVector<Game>& games) {
      QVector<WeekResults> results;
      results.reserve(_pow(2, NUM_TEAMS / 2));

      // possible results for each game (winning/losing team)
      const auto game_1_results = generateGameResults(games[0]);
      const auto game_2_results = generateGameResults(games[1]);
      const auto game_3_results = generateGameResults(games[2]);
      const auto game_4_results = generateGameResults(games[3]);

      for (const GameResult& result_1 : game_1_results) {
        for (const GameResult& result_2 : game_2_results) {
          for (const GameResult& result_3 : game_3_results) {
            for (const GameResult& result_4 : game_4_results) {
              results.append({ result_1, result_2, result_3, result_4 });
            }
          }
        }
      }

      return results;
    }

    static void sortStandings(Standings& standings) {
      std::sort(standings.records.begin(), standings.records.end());
    }

    static void generateOutcomes(const Schedule& schedule, const Standings& prev_week_standings, Analyzer& analyzer) {
      const week_t current_week = prev_week_standings.week + 1;

      if (!schedule.contains(current_week)) {
        // reached end of schedule, add outcome
        analyzer.addOutcome(prev_week_standings);

        return;
      }

      const auto week_results = generateWeekResults(schedule[current_week]);

      for (const auto& result : week_results) {
        auto new_standings = prev_week_standings;
        ++new_standings.week;

        for (const auto& game_result : result) {
          new_standings.addResult(game_result);
        }

        sortStandings(new_standings);

        generateOutcomes(schedule, new_standings, analyzer);
      }
    }
  };

}

#endif
