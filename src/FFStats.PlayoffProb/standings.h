#ifndef STANDINGS_H
#define STANDINGS_H

#include <cassert>

#include "gameresult.h"
#include "teamrecord.h"

namespace ffstats::playoffprob {

  struct Standings {
    week_t week;
    mutable std::array<TeamRecord, NUM_TEAMS> records;

    Standings() : week(0), m_cache_valid(false) {}

    const auto& teamRanked(const rank_t rank) const {
      // rank is 1-based
      // records is 0-based
      return records[rank - 1];
    }

    void addResult(const GameResult& result) {
      for (auto& record : records) {
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

    rank_t teamRank(const team_t team) const {
      rank_t rank = 1;
      for (const auto& record : records) {
        if (team == record.team) {
          return rank;
        }

        ++rank;
      }

      assert(false);
      return 0;
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

      if (tiebreaker_teams.empty() || !tiebreaker_teams.contains(team)) {
        // tiebreaker solved, or team is not involved in tiebreaker
        return (teamRank(team) <= NUM_PLAYOFF_TEAMS);
      }

      // can't break tiebreaker
      return include_tiebreakers;
    }

    win_t numWinsForTeam(const team_t team) const {
      for (const auto& record : records) {
        if (team == record.team) {
          return record.win;
        }
      }

      assert(false);
      return 0;
    }

    win_t numWinsForRank(const rank_t rank) const {
      return teamRanked(rank).win;
    }

    bool hasTiebreaker() const {
      return !findTiebreakerTeams().empty();
    }

    std::map<team_t, TeamRecord> findTiebreakerTeams() const {
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

      for (const auto& record : records) {
        if (record.win == numWinsForRank(NUM_PLAYOFF_TEAMS)) {
          teams.insert({ record.team, record });

          if (rank_first == 0) {
            rank_first = teamRank(record.team);
          }
        }
      }

      // should find at least two teams
      assert(teams.size() >= 2);

      if (teams.size() == 2) {
        // check h2h record
        auto it = teams.cbegin();
        const auto team1_record = it->second;
        const auto team2_record = (++it)->second;

        if (team1_record.head2head[team2_record.team] != 0) {
          teams.clear(); // no tiebreaker
        }
      }
      else if (teams.size() > 2) {
        // make sub-records only including tiebreaker teams
        std::vector<SubTeamRecord> sub_records;
        sub_records.reserve(NUM_TEAMS);

        for (const auto& [my_key, my_record]: teams) {
          SubTeamRecord sub_record(my_record.team);

          for (const auto& [other_key, other_record] : teams) {
            if (other_record.team != my_record.team) {
              sub_record.win_loss += my_record.head2head[other_record.team];
            }
          }

          sub_records.push_back(sub_record);
        }

        // sort by win-loss record
        std::sort(sub_records.begin(), sub_records.end());

        // modify records according to sorted sub-records
        auto rank = rank_first;

        for (const auto& sub_record : sub_records) {
          records[rank - 1] = teams[sub_record.team];

          ++rank;
        }

        // no of playoff teams within tiebreaker
        const size_t num_playoff_teams = NUM_PLAYOFF_TEAMS - rank_first + 1;
        assert(num_playoff_teams >= 1);

        const auto& last_team_in = sub_records[static_cast<int>(num_playoff_teams - 1)];
        const auto& first_team_out = sub_records[static_cast<int>(num_playoff_teams)];

        if (last_team_in.win_loss == first_team_out.win_loss) {
          for (const auto& sub_record : sub_records) {
            if (sub_record.win_loss != last_team_in.win_loss) {
              // not involved in tiebreaker
              teams.erase(sub_record.team);
            }
          }

          assert(teams.size() >= 2);
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
    mutable std::map<team_t, TeamRecord> m_tiebreaker_team_cache;
    mutable bool m_cache_valid = false;
  };

}

#endif
