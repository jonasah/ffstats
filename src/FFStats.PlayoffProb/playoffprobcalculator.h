#ifndef PLAYOFF_PROB_CALCULATOR
#define PLAYOFF_PROB_CALCULATOR

#include <QtSql/QSqlRecord>

#include "analyzer.h"
#include "database.h"
#include "math.h"

//#define MAX_WEEKS_TO_SIMULATE 4

//#define TIME_MEASURE

#ifdef TIME_MEASURE
#include <chrono>
#endif

class PlayoffProbCalculator {
public:
  static bool calculate(const year_t year, const week_t week) {
    if (!Database::init()) {
      qWarning() << "Failed to initialize database: " << Database::getLastError();
      return false;
    }

    Standings start_standings;
    Schedule schedule;

    // initial values
    {
      start_standings = readStandings(year, week);
      schedule = readSchedule(year, week + 1);
    }

    // do the work
    {
      Analyzer analyzer;

#ifdef TIME_MEASURE
      const auto t0 = std::chrono::steady_clock::now();
#endif

      generateOutcomes(schedule, start_standings, analyzer);

#ifdef TIME_MEASURE
      const auto t1 = std::chrono::steady_clock::now();
#endif

      analyzer.summarize();

#ifdef TIME_MEASURE
      const auto nsecs = std::chrono::nanoseconds{ t1 - t0 }.count();
      qDebug().noquote() << QString("\nElapsed time: %1 s").arg(nsecs / 1e9);
#endif
    }

    return true;
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

  static Standings readStandings(const year_t year, const week_t week) {
    Standings standings;
    standings.week = week;

    // fetch team records
    {
      SelectQuery query("TeamRecords", QString("Year = %1 AND Week = %2").arg(year).arg(week));
      query.setSort(4, Qt::AscendingOrder); // sort by rank
      query.execute();

      Q_ASSERT(query.rowCount() == NUM_TEAMS);

      for (auto it = query.begin(), e = query.end(); it != e; ++it) {
        auto sql_record = *it;
        const auto team = sql_record.value("TeamId").value<team_t>();
        const auto win = sql_record.value("Win").value<wint_t>();
        const auto loss = sql_record.value("Loss").value<loss_t>();

        // use temporary zero-based team id during calculations
        const auto temp_team = static_cast<team_t>(it.row());
        TeamInfo::addConversion(team, temp_team);

        TeamRecord team_record(temp_team, win, loss);

        standings.records[it.row()] = team_record;
      }
    }

    // fetch h2h records
    for (auto& team_record : standings.records) {
      SelectQuery query("Head2HeadRecords", QStringLiteral("Year = %1 AND Week = %2 AND TeamId = %3").arg(year).arg(week).arg(TeamInfo::getDatabaseId(team_record.team)));
      query.execute();

      Q_ASSERT(query.rowCount() == NUM_TEAMS - 1);

      for (auto sql_record : query) {
        const auto opponent = sql_record.value("OpponentId").value<team_t>();
        const auto w = sql_record.value("Win").value<win_loss_t>();
        const auto l = sql_record.value("Loss").value<win_loss_t>();

        team_record.setHead2HeadRecord(TeamInfo::getTemporaryId(opponent), w - l);
      }
    }

    // fetch team names
    {
      // convert team ids to list of strings
      auto team_ids = TeamInfo::getDatabaseIds();
      QStringList team_ids_as_string;
      std::transform(team_ids.cbegin(), team_ids.cend(), std::back_inserter(team_ids_as_string), [](auto id) { return QString::number(id); });

      SelectQuery query("Teams", QString("Id in (%1)").arg(team_ids_as_string.join(',')));
      query.execute();

      Q_ASSERT(query.rowCount() == team_ids.count());

      for (auto sql_record : query) {
        const auto team_id = sql_record.value("Id").value<team_t>();
        const auto team_name = sql_record.value("Name").toString();
        TeamInfo::addName(team_id, team_name);
      }
    }

    return standings;
  }

  static Schedule readSchedule(const year_t year, const week_t starting_week) {
    Schedule schedule;

#ifdef MAX_WEEKS_TO_SIMULATE
    const week_t end_week = qMin(starting_week + MAX_WEEKS_TO_SIMULATE - 1, 14);
#else
    const week_t end_week = 14;
#endif

    SelectQuery query("Games", QString("Year = %1 AND Week >= %2 AND Week <= %3").arg(year).arg(starting_week).arg(end_week));
    query.execute();

    for (auto sql_record : query) {
      const auto week = sql_record.value("Week").value<week_t>();

      if (!schedule.contains(week)) {
        schedule.insert(week, QVector<Game>());
      }

      const auto team1 = sql_record.value("Team1Id").value<team_t>();
      const auto team2 = sql_record.value("Team2Id").value<team_t>();

      Q_ASSERT(TeamInfo::exists(team1));
      Q_ASSERT(TeamInfo::exists(team2));

      schedule[week].append(Game(TeamInfo::getTemporaryId(team1), TeamInfo::getTemporaryId(team2)));
    }

    return schedule;
  }
};

#endif
