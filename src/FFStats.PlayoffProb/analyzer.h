#ifndef ANALYZER_H
#define ANALZYER_H

#include <QtCore/QDebug>

#include "standings.h"
#include "teaminfo.h"

class Analyzer {
public:
  Analyzer() :
    m_counter(0),
    m_tiebreaker_count(0)
  {
    for (rank_t rank = 1; rank <= NUM_TEAMS; ++rank) {
      for (win_t wins = 0; wins <= 14; ++wins) {
        m_wins_per_rank[rank - 1][wins] = Q_UINT64_C(0);
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

      ++m_wins_per_rank[rank - 1][wins];
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

      qDebug() << TeamInfo::getNameForTemporaryId(team);
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

#endif
