#include <array>

#include <QtCore/QCommandLineParser>
#include <QtCore/QDebug>
#include <QtCore/QMap>
#include <QtCore/QVarLengthArray>
#include <QtCore/QVector>

#include "xml/pugixml.hpp"

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

enum class Team : unsigned short {
  ViciousVoxels = 0,
  RetroHawks = 1,
  TheBelichickians = 2,
  Bonecrushers = 3,
  KingsOfTheNorth = 4,
  RamshallEagles = 5,
  NangijalaIF = 6,
  TheShamones = 7,

  Unknown = NUM_TEAMS
};

constexpr auto teamIndex(const Team t) {
  return static_cast<std::underlying_type_t<Team>>(t);
}

uint qHash(Team team, uint seed) {
  return qHash(teamIndex(team), seed);
}

QString teamToString(const Team team) {
  switch (team) {
  case Team::ViciousVoxels:
    return "Vicious Voxels";
  case Team::RetroHawks:
    return "Retro Hawks";
  case Team::TheBelichickians:
    return "The Belichickians";
  case Team::Bonecrushers:
    return "Bonecrushers";
  case Team::KingsOfTheNorth:
    return "Kings of the North";
  case Team::RamshallEagles:
    return "Ramshall Eagles";
  case Team::NangijalaIF:
    return "Nangijala IF";
  case Team::TheShamones:
    return "The Shamones";
  }

  return "Unknown";
}

Team stringToTeam(const QString& team_name) {
  if (team_name == QStringLiteral("Vicious Voxels")) {
    return Team::ViciousVoxels;
  }
  else if (team_name == QStringLiteral("Retro Hawks")) {
    return Team::RetroHawks;
  }
  else if (team_name == QStringLiteral("The Belichickians")) {
    return Team::TheBelichickians;
  }
  else if (team_name == QStringLiteral("Bonecrushers")) {
    return Team::Bonecrushers;
  }
  else if (team_name == QStringLiteral("Kings of the North")) {
    return Team::KingsOfTheNorth;
  }
  else if (team_name == QStringLiteral("Ramshall Eagles")) {
    return Team::RamshallEagles;
  }
  else if (team_name == QStringLiteral("Nangijala IF")) {
    return Team::NangijalaIF;
  }
  else if (team_name == QStringLiteral("The Shamones")) {
    return Team::TheShamones;
  }

  return Team::Unknown;
}

// std::array with size NUM_TEAMS which can be indexed by Team
template <typename T>
class TeamAssociativeArray : public std::array<T, NUM_TEAMS> {
public:
  using std::array<T, NUM_TEAMS>::operator[];

  T& operator[](const Team t) {
    return this->operator[](teamIndex(t));
  }

  const T& operator[](const Team t) const {
    return this->operator[](teamIndex(t));
  }
};

using week_t = quint16;
using rank_t = quint16;
using win_t = quint16;
using loss_t = quint16;
using win_loss_t = qint16;

// a team's win/loss record and head to head record against other teams
struct TeamRecord {
  Team team;
  win_t win;
  loss_t loss;

  // head to head record, positive more wins, negative more losses
  TeamAssociativeArray<win_loss_t> head2head;

  TeamRecord() : team(Team::Unknown), win(0), loss(0) {}
  TeamRecord(const Team t, const win_t w, const loss_t l) : team(t), win(w), loss(l) {}

  void setHead2HeadRecord(const Team vs, const win_loss_t h2h) {
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
  Team team;
  win_loss_t win_loss;

  SubTeamRecord() = default;
  SubTeamRecord(const Team t) : team(t), win_loss(0) {}
};

bool operator<(const SubTeamRecord& record1, const SubTeamRecord& record2) {
  // here 'less than' means 'ranked above'
  return (record1.win_loss > record2.win_loss);
}

// result of a single game
struct GameResult {
  Team win;
  Team loss;

  GameResult() = default;
  GameResult(const Team w, const Team l) : win(w), loss(l) {}
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

  auto teamRank(const Team team) const {
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

  bool inPlayoffs(const Team team, const bool include_tiebreakers) const {
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

  win_t numWinsForTeam(const Team team) const {
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

  QHash<Team, TeamRecord> findTiebreakerTeams() const {
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
  mutable QHash<Team, TeamRecord> m_tiebreaker_team_cache;
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

    {
      m_playoff[Team::ViciousVoxels] = { Q_UINT64_C(0), Q_UINT64_C(0) };
      m_playoff[Team::RetroHawks] = { Q_UINT64_C(0), Q_UINT64_C(0) };
      m_playoff[Team::TheBelichickians] = { Q_UINT64_C(0), Q_UINT64_C(0) };
      m_playoff[Team::Bonecrushers] = { Q_UINT64_C(0), Q_UINT64_C(0) };
      m_playoff[Team::KingsOfTheNorth] = { Q_UINT64_C(0), Q_UINT64_C(0) };
      m_playoff[Team::RamshallEagles] = { Q_UINT64_C(0), Q_UINT64_C(0) };
      m_playoff[Team::NangijalaIF] = { Q_UINT64_C(0), Q_UINT64_C(0) };
      m_playoff[Team::TheShamones] = { Q_UINT64_C(0), Q_UINT64_C(0) };
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

    static const auto teams =
        QVector<Team>() << Team::ViciousVoxels << Team::RetroHawks << Team::TheBelichickians << Team::Bonecrushers
                        << Team::KingsOfTheNorth << Team::RamshallEagles << Team::NangijalaIF << Team::TheShamones;

    for (const auto team : teams) {
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

    std::underlying_type_t<Team> team_index = 0;

    for (const auto& playoffs : m_playoff) {
      const auto team = static_cast<Team>(team_index);
      const auto playoffs_excl = playoffs.first;
      const auto playoffs_incl = playoffs.second;

      const auto tiebreaker_counter = playoffs_incl - playoffs_excl;

      const auto playoffs_excl_prob = playoffs_excl / static_cast<double>(m_counter);
      const auto playoffs_incl_prob = playoffs_incl / static_cast<double>(m_counter);
      const auto tiebreaker_prob = tiebreaker_counter / static_cast<double>(m_counter);

      qDebug() << teamToString(team);
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

using Game = QPair<Team, Team>;
using Schedule = QMap<week_t, QVector<Game>>;

//QDebug operator<<(QDebug dbg, const Standings& standings) {
//  for (const TeamRecord& record : standings.records) {
//    dbg.nospace()
//        << teamToString(record.team) << '\t'
//        << record.win << '-' << record.loss << endl;

//    for (auto it = record.head2head.cbegin(); it != record.head2head.cend(); ++it) {
//      dbg.nospace() << "  " << teamToString(it.key()) << '\t' << it.value() << endl;
//    }
//  }

//  return dbg;
//}

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

Standings readStandings(const week_t week, const pugi::xml_node& xml) {
  Standings standings;
  standings.week = week;

  for (const auto& week_elem : xml) {
    const auto week_no = week_elem.attribute("no").as_int();

    if (week_no != week) {
      continue;
    }

    auto counter = size_t{0};

    for (const auto& team_elem : week_elem) {
      const auto team = stringToTeam(QString(team_elem.attribute("name").as_string()));
      const auto record = QString(team_elem.attribute("record").as_string()).split('-');

      Q_ASSERT(record.size() == 2);

      TeamRecord team_record(team, record.at(0).toUShort(), record.at(1).toUShort());

      // count win and loss for assert
      auto win = 0;
      auto loss = 0;

      for (const auto& h2h_elem : team_elem) {
        const auto team = stringToTeam(QString(h2h_elem.attribute("team").as_string()));
        const auto record = QString(h2h_elem.attribute("record").as_string()).split('-');

        Q_ASSERT(record.size() == 2);

        team_record.setHead2HeadRecord(team, record.at(0).toShort() - record.at(1).toShort());

        win += record.at(0).toInt();
        loss += record.at(1).toInt();
      }

      Q_ASSERT(win == team_record.win);
      Q_ASSERT(loss == team_record.loss);

      standings.records[counter] = team_record;

      ++counter;
    }

    Q_ASSERT(counter == NUM_TEAMS);

    break;
  }

  return standings;
}

Schedule readSchedule(const week_t starting_week, const pugi::xml_node& xml) {
  Schedule schedule;

  for (const auto& week_elem : xml) {
    const auto week = static_cast<week_t>(week_elem.attribute("no").as_uint());

    if (week < starting_week) {
      continue;
    }

    schedule.insert(week, QVector<Game>());

    for (auto game_it = week_elem.begin(); game_it != week_elem.end(); ++game_it) {
      const auto game_elem = *game_it;

      const auto team1 = stringToTeam(game_elem.first_child().attribute("name").as_string());
      const auto team2 = stringToTeam(game_elem.last_child().attribute("name").as_string());

      schedule[week].append(Game(team1, team2));
    }

#ifdef MAX_WEEKS_TO_SIMULATE
    static auto counter = 0;
    ++counter;

    if (counter == MAX_WEEKS_TO_SIMULATE) {
      break;
    }
#endif
  }

  return schedule;
}

int main(int argc, char** argv) {
  QCoreApplication app(argc, argv);

  QCommandLineParser parser;
  parser.addOption(QCommandLineOption("week", "From week", "week", "0"));

  if (!parser.parse(app.arguments())) {
    qWarning() << parser.errorText();
    return 1;
  }

  week_t week = parser.value("week").toUShort();

  Q_ASSERT(week <= 14);

  Standings start_standings;
  Schedule schedule;

  // initial values
  {
    auto&& xml_doc = pugi::xml_document{};
    xml_doc.load_file("fantasyfootball.xml");
    const auto xml_root = xml_doc.document_element();

    start_standings = readStandings(week, xml_root.child("standings"));
    schedule = readSchedule(week+1, xml_root.child("schedule"));
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
