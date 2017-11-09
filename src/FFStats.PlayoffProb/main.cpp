#include <array>

#include <QtCore/QCommandLineParser>
#include <QtCore/QDebug>
#include <QtCore/QMap>
#include <QtCore/QVarLengthArray>
#include <QtCore/QVector>

#include <QtSql/QSqlDatabase>
#include <QtSql/QSqlError>
#include <QtSql/QSqlRecord>
#include <QtSql/QSqlTableModel>

#define NUM_TEAMS 8
#define NUM_PLAYOFF_TEAMS 4

//#define MAX_WEEKS_TO_SIMULATE 4

//#define TIME_MEASURE

#ifdef TIME_MEASURE
#include <chrono>
#endif

namespace {
  constexpr unsigned int _pow(const unsigned int base, const unsigned int exp) {
    return exp == 0 ? 1 : base * _pow(base, exp-1);
  }
}

using team_t = quint16;
using year_t = quint16;
using week_t = quint16;
using rank_t = quint16;
using win_t = quint16;
using loss_t = quint16;
using win_loss_t = qint16;

// conversion from team id in database to temporary team id
QHash<team_t, team_t> g_team_to_temp_table;
// conversion from temporary team id to team id in database
QHash<team_t, team_t> g_temp_to_team_table;
// team name lookup table
QHash<team_t, QString> g_team_names;

QString teamToString(const team_t team) {
  return g_team_names[team];
}

// std::array with size NUM_TEAMS
template <typename T>
using TeamAssociativeArray = std::array<T, NUM_TEAMS>;

// a team's win/loss record and head to head record against other teams
struct TeamRecord {
  team_t team;
  win_t win;
  loss_t loss;

  // head to head record, positive more wins, negative more losses
  TeamAssociativeArray<win_loss_t> head2head;

  TeamRecord() : team(NUM_TEAMS), win(0), loss(0) {}
  TeamRecord(const team_t t, const win_t w, const loss_t l) : team(t), win(w), loss(l) {}

  void setHead2HeadRecord(const team_t vs, const win_loss_t h2h) {
    head2head[vs] = h2h;
  }
};

bool operator<(const TeamRecord& record1, const TeamRecord& record2) {
  // here 'less than' means 'ranked above'
  if (record1.win == record2.win) {
    return (record1.head2head[record2.team] > 0);
  }

  return (record1.win > record2.win);
}

struct SubTeamRecord {
  team_t team;
  win_loss_t win_loss;

  SubTeamRecord() = default;
  SubTeamRecord(const team_t t) : team(t), win_loss(0) {}
};

bool operator<(const SubTeamRecord& record1, const SubTeamRecord& record2) {
  // here 'less than' means 'ranked above'
  return (record1.win_loss > record2.win_loss);
}

// result of a single game
struct GameResult {
  team_t win;
  team_t loss;

  GameResult() = default;
  GameResult(const team_t w, const team_t l) : win(w), loss(l) {}
};

// game results for one week
using WeekResults = std::array<GameResult, NUM_TEAMS / 2>;

struct Standings {
  week_t week;
  mutable std::array<TeamRecord, NUM_TEAMS> records;

  Standings() : week(0), m_cache_valid(false) {}

  const TeamRecord& teamRanked(const rank_t rank) const {
    // rank is 1-based
    // records is 0-based
    return records[rank-1];
  }

  void addResult(const GameResult& result) {
    for (TeamRecord& record : records) {
      if (result.win == record.team) {
        ++record.win;
        ++record.head2head[result.loss]; // add win against losing team
      }
      else if (result.loss == record.team) {
        ++record.loss;
        --record.head2head[result.win]; // add loss against winning team;
      }
    }

    m_cache_valid = false;
  }

  auto teamRank(const team_t team) const {
    rank_t rank = 1;
    for (const TeamRecord& record : records) {
      if (team == record.team) {
        return rank;
      }

      ++rank;
    }

    qWarning() << "PERKELE";
    return rank_t{0};
  }

  bool inPlayoffs(const team_t team, const bool include_tiebreakers) const {
    if (numWinsForTeam(team) > numWinsForRank(NUM_PLAYOFF_TEAMS + 1)) {
      return true;
    }
    else if (numWinsForTeam(team) < numWinsForRank(NUM_PLAYOFF_TEAMS)) {
      return false;
    }

    // team in potential tiebreaker
    const auto tiebreaker_teams = findTiebreakerTeams();

    if (tiebreaker_teams.isEmpty() || !tiebreaker_teams.contains(team)) {
      // tiebreaker solved, or team is not involved in tiebreaker
      return (teamRank(team) <= NUM_PLAYOFF_TEAMS);
    }

    // can't break tiebreaker
    return include_tiebreakers;
  }

  win_t numWinsForTeam(const team_t team) const {
    for (const TeamRecord& record : records) {
      if (team == record.team) {
        return record.win;
      }
    }

    qWarning() << "PERKELE";
    return 0;
  }

  win_t numWinsForRank(const rank_t rank) const {
    return teamRanked(rank).win;
  }

  bool hasTiebreaker() const {
    return !findTiebreakerTeams().isEmpty();
  }

  QHash<team_t, TeamRecord> findTiebreakerTeams() const {
    if (m_cache_valid) {
      return m_tiebreaker_team_cache;
    }

    auto& teams = m_tiebreaker_team_cache;
    teams.clear();

    if (numWinsForRank(NUM_PLAYOFF_TEAMS) > numWinsForRank(NUM_PLAYOFF_TEAMS + 1)) {
      // no tiebreaker if teams around playoff break position don't have same number of wins
      m_cache_valid = true;
      return teams;
    }

    rank_t rank_first = 0;

    for (const TeamRecord& record : records) {
      if (record.win == numWinsForRank(NUM_PLAYOFF_TEAMS)) {
        teams.insert(record.team, record);

        if (rank_first == 0) {
          rank_first = teamRank(record.team);
        }
      }
    }

    // should find at least two teams
    Q_ASSERT(teams.size() >= 2);

    if (teams.size() == 2) {
      // check h2h record
      const auto team1_record = teams.values().first();
      const auto team2_record = teams.values().last();

      if (team1_record.head2head[team2_record.team] != 0) {
        teams.clear(); // no tiebreaker
      }
    }
    else if (teams.size() > 2) {
      // make sub-records only including tiebreaker teams
      QVarLengthArray<SubTeamRecord, NUM_TEAMS> sub_records;

      for (const auto& my_record : teams) {
        SubTeamRecord sub_record(my_record.team);

        for (const auto& other_record : teams) {
          if (other_record.team != my_record.team) {
            sub_record.win_loss += my_record.head2head[other_record.team];
          }
        }

        sub_records.append(sub_record);
      }

      // sort by win-loss record
      std::sort(sub_records.begin(), sub_records.end());

      // modify records according to sorted sub-records
      auto rank = rank_first;

      for (const auto& sub_record : sub_records) {
        records[rank-1] = teams[sub_record.team];

        ++rank;
      }

      // no of playoff teams within tiebreaker
      const size_t num_playoff_teams = NUM_PLAYOFF_TEAMS - rank_first + 1;
      Q_ASSERT(num_playoff_teams >= 1);

      const SubTeamRecord& last_team_in = sub_records[static_cast<int>(num_playoff_teams - 1)];
      const SubTeamRecord& first_team_out = sub_records[static_cast<int>(num_playoff_teams)];

      if (last_team_in.win_loss == first_team_out.win_loss) {
        for (const auto& sub_record : sub_records) {
          if (sub_record.win_loss != last_team_in.win_loss) {
            // not involved in tiebreaker
            teams.remove(sub_record.team);
          }
        }

        Q_ASSERT(teams.size() >= 2);
      }
      else {
        // no tiebreaker
        teams.clear();
      }
    }

    m_cache_valid = true;
    return teams;
  }

private:
  mutable QHash<team_t, TeamRecord> m_tiebreaker_team_cache;
  mutable bool m_cache_valid = false;
};

class Analyzer {
public:
  Analyzer() :
    m_counter(0),
    m_tiebreaker_count(0)
  {
    for (rank_t rank = 1; rank <= NUM_TEAMS; ++rank) {
      for (win_t wins = 0; wins <= 14; ++wins) {
        m_wins_per_rank[rank-1][wins] = Q_UINT64_C(0);
      }
    }

    for (team_t team = 0; team < NUM_TEAMS; ++team) {
      m_playoff[team] = { Q_UINT64_C(0), Q_UINT64_C(0) };
    }
  }

  void addOutcome(const Standings& outcome) {
    ++m_counter;

    for (rank_t rank = 1; rank <= NUM_TEAMS; ++rank) {
      const auto wins = outcome.numWinsForRank(rank);

      ++m_wins_per_rank[rank-1][wins];
    }

    if (outcome.hasTiebreaker()) {
      ++m_tiebreaker_count;
    }

    for (team_t team = 0; team < NUM_TEAMS; ++team) {
      if (outcome.inPlayoffs(team, false)) {
        ++m_playoff[team].first;
      }
      if (outcome.inPlayoffs(team, true)) {
        ++m_playoff[team].second;
      }
    }
  }

  void summarize() const {
    qDebug() << "No of outcomes:" << m_counter << endl;

    const auto tiebreaker_prob = m_tiebreaker_count / static_cast<double>(m_counter);

    auto rank = 1;

    for (const auto& wins_array : m_wins_per_rank) {
      qDebug().nospace() << "Wins for rank " << rank;

      auto num_wins = 0;

      for (const auto outcomes : wins_array) {
        if (outcomes > 0) {
          const auto frac = outcomes / static_cast<double>(m_counter);

          qDebug().nospace() << "  " << num_wins << ": " << qSetRealNumberPrecision(4) << frac << " (" << outcomes << ")";
        }

        ++num_wins;
      }

      ++rank;
    }

    qDebug().nospace() << endl << "Tiebreaker: " << qSetRealNumberPrecision(4) << tiebreaker_prob << " (" << m_tiebreaker_count << ")";

    qDebug() << endl << "-- Playoff probabilities --";

    team_t team_index = 0;

    for (const auto& playoffs : m_playoff) {
      const auto team = team_index;
      const auto playoffs_excl = playoffs.first;
      const auto playoffs_incl = playoffs.second;

      const auto tiebreaker_counter = playoffs_incl - playoffs_excl;

      const auto playoffs_excl_prob = playoffs_excl / static_cast<double>(m_counter);
      const auto playoffs_incl_prob = playoffs_incl / static_cast<double>(m_counter);
      const auto tiebreaker_prob = tiebreaker_counter / static_cast<double>(m_counter);

      qDebug() << teamToString(g_temp_to_team_table[team]);
      qDebug().nospace() << "  Excl. tiebreakers\t" << qSetRealNumberPrecision(4) << playoffs_excl_prob << "\t(" << playoffs_excl << ")";
      qDebug().nospace() << "  Tiebreaker\t" << qSetRealNumberPrecision(4) << tiebreaker_prob << "\t(" << tiebreaker_counter << ')';
      qDebug().nospace() << "  Incl. tiebreakers\t" << qSetRealNumberPrecision(4) << playoffs_incl_prob << "\t(" << playoffs_incl << ')';

      ++team_index;
    }
  }

private:
  quint64 m_counter;

  std::array<std::array<quint64, 15>, NUM_TEAMS> m_wins_per_rank;
  quint64 m_tiebreaker_count;

  TeamAssociativeArray<QPair<quint64, quint64>> m_playoff;
};

using Game = QPair<team_t, team_t>;
using Schedule = QMap<week_t, QVector<Game>>;

// generate all possible results for a single game
QVector<GameResult> generateGameResults(const Game& game) {
  return {{ game.first, game.second }, { game.second, game.first }};
}

// generate all possible results for a single week
QVector<WeekResults> generateWeekResults(const QVector<Game>& games) {
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
          results.append({result_1, result_2, result_3, result_4});
        }
      }
    }
  }

  return results;
}

void sortStandings(Standings& standings) {
  std::sort(standings.records.begin(), standings.records.end());
}

void generateOutcomes(const Schedule& schedule, const Standings& prev_week_standings, Analyzer& analyzer) {
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

Standings readStandings(const year_t year, const week_t week) {
  Standings standings;
  standings.week = week;

  // fetch team records
  {
    QSqlTableModel model;
    model.setTable("TeamRecords");
    model.setFilter(QString("Year = %1 AND Week = %2").arg(year).arg(week));
    model.setSort(4, Qt::AscendingOrder); // sort by rank
    model.select();

    Q_ASSERT(model.rowCount() == NUM_TEAMS);

    for (int i = 0; i < model.rowCount(); ++i) {
      auto sql_record = model.record(i);
      const auto team = sql_record.value("TeamId").value<team_t>();
      const auto win = sql_record.value("Win").value<wint_t>();
      const auto loss = sql_record.value("Loss").value<loss_t>();

      // use temporary zero-based team id during calculations
      const auto temp_team = g_team_to_temp_table.keys().count();
      g_team_to_temp_table.insert(team, temp_team);
      g_temp_to_team_table.insert(temp_team, team);

      TeamRecord team_record(temp_team, win, loss);

      standings.records[i] = team_record;
    }
  }

  // fetch h2h records
  for (auto& team_record : standings.records) {
    QSqlTableModel model;
    model.setTable("Head2HeadRecords");
    model.setFilter(QStringLiteral("Year = %1 AND Week = %2 AND TeamId = %3").arg(year).arg(week).arg(g_temp_to_team_table[team_record.team]));
    model.select();

    Q_ASSERT(model.rowCount() == NUM_TEAMS - 1);

    for (int j = 0; j < model.rowCount(); ++j) {
      auto sql_record = model.record(j);
      const auto opponent = sql_record.value("OpponentId").value<team_t>();
      const auto w = sql_record.value("Win").value<win_loss_t>();
      const auto l = sql_record.value("Loss").value<win_loss_t>();

      team_record.setHead2HeadRecord(g_team_to_temp_table[opponent], w - l);
    }
  }

  // fetch team names
  {
    // convert team ids to list of strings
    auto team_ids = g_team_to_temp_table.keys();
    QStringList team_ids_as_string;
    std::transform(team_ids.cbegin(), team_ids.cend(), std::back_inserter(team_ids_as_string), [](auto id) { return QString::number(id); });

    QSqlTableModel model;
    model.setTable("Teams");
    model.setFilter(QString("Id in (%1)").arg(team_ids_as_string.join(',')));
    model.select();

    for (int i = 0; i < model.rowCount(); ++i) {
      auto sql_record = model.record(i);
      const auto team_id = sql_record.value("Id").value<team_t>();
      const auto team_name = sql_record.value("Name").toString();
      g_team_names.insert(team_id, team_name);
    }
  }

  return standings;
}

Schedule readSchedule(const year_t year, const week_t starting_week) {
  Schedule schedule;

#ifdef MAX_WEEKS_TO_SIMULATE
  const week_t end_week = qMin(starting_week + MAX_WEEKS_TO_SIMULATE - 1, 14);
#else
  const week_t end_week = 14;
#endif

  QSqlTableModel games_model;
  games_model.setTable("Games");
  games_model.setFilter(QString("Year = %1 AND Week >= %2 AND Week <= %3").arg(year).arg(starting_week).arg(end_week));
  games_model.select();

  for (int i = 0; i < games_model.rowCount(); ++i) {
    auto game_sql_record = games_model.record(i);
    const auto week = game_sql_record.value("Week").value<week_t>();

    if (!schedule.contains(week)) {
      schedule.insert(week, QVector<Game>());
    }

    const auto team1 = game_sql_record.value("Team1Id").value<team_t>();
    const auto team2 = game_sql_record.value("Team2Id").value<team_t>();

    Q_ASSERT(g_team_to_temp_table.contains(team1));
    Q_ASSERT(g_team_to_temp_table.contains(team2));

    schedule[week].append(Game(g_team_to_temp_table[team1], g_team_to_temp_table[team2]));
  }

  return schedule;
}

bool initDatabaseConnection() {
  auto db = QSqlDatabase::addDatabase("QSQLITE");
  db.setDatabaseName(R"(..\FFStats.DbHandler\ffstats.db)");
  return db.open();
}

QString getDatabaseError() {
  return QSqlDatabase::database().lastError().text();
}

int main(int argc, char** argv) {
  QCoreApplication app(argc, argv);

  QCommandLineParser parser;
  parser.addOption(QCommandLineOption("year", "Year", "year"));
  parser.addOption(QCommandLineOption("week", "From week", "week", "1"));

  if (!parser.parse(app.arguments())) {
    qWarning() << parser.errorText();
    return 1;
  }

  if (!parser.isSet("year")) {
    qWarning() << "Year is missing";
    return 1;
  }

  year_t year = parser.value("year").toUShort();
  week_t week = parser.value("week").toUShort();

  Q_ASSERT(week >= 1 && week <= 14);

  if (!initDatabaseConnection()) {
    qWarning() << "Failed to initialize database: " << getDatabaseError();
    return 1;
  }

  Standings start_standings;
  Schedule schedule;

  // initial values
  {
    start_standings = readStandings(year, week);
    schedule = readSchedule(year, week+1);
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
    const auto nsecs = std::chrono::nanoseconds{t1-t0}.count();
    qDebug().noquote() << QString("\nElapsed time: %1 s").arg(nsecs / 1e9);
#endif
  }

  return 0;
}
