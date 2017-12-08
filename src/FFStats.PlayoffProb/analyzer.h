#ifndef ANALYZER_H
#define ANALZYER_H

#include "standings.h"
#include "teaminfo.h"

namespace ffstats::playoffprob {

  struct PlayoffProbability {
    team_t team;
    double excl_tiebreakers;
    double incl_tiebreakers;
  };

  class Analyzer {
  public:
    Analyzer() :
      m_counter(0)
    {
      for (team_t team = 0; team < NUM_TEAMS; ++team) {
        m_playoff[team] = { Q_UINT64_C(0), Q_UINT64_C(0) };
      }
    }

    void addOutcome(const Standings& outcome) {
      ++m_counter;

      for (team_t team = 0; team < NUM_TEAMS; ++team) {
        if (outcome.inPlayoffs(team, false)) {
          ++m_playoff[team].first;
        }
        if (outcome.inPlayoffs(team, true)) {
          ++m_playoff[team].second;
        }
      }
    }

    QVector<PlayoffProbability> summarize() const {
      auto playoff_probs = QVector<PlayoffProbability>{};
      team_t team_index = 0;

      for (const auto& playoffs : m_playoff) {
        const auto team_id = TeamInfo::getDatabaseId(team_index);
        const auto playoffs_excl = playoffs.first;
        const auto playoffs_incl = playoffs.second;

        const auto playoffs_excl_prob = playoffs_excl / static_cast<double>(m_counter);
        const auto playoffs_incl_prob = playoffs_incl / static_cast<double>(m_counter);

        playoff_probs.append({ team_id, playoffs_excl_prob, playoffs_incl_prob });

        ++team_index;
      }

      return playoff_probs;
    }

  private:
    quint64 m_counter;

    TeamAssociativeArray<QPair<quint64, quint64>> m_playoff;
  };

}

#endif
