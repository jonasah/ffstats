#ifndef TYPEDEFS_H
#define TYPEDEFS_H

#include <array>

#include <QtCore/QMap>
#include <QtCore/QVector>

#define NUM_TEAMS 8
#define NUM_PLAYOFF_TEAMS 4

namespace ffstats::playoffprob {

  using team_t = quint16;
  using year_t = quint16;
  using week_t = quint16;
  using rank_t = quint16;
  using win_t = quint16;
  using loss_t = quint16;
  using win_loss_t = qint16;

  // std::array with size NUM_TEAMS
  template <typename T>
  using TeamAssociativeArray = std::array<T, NUM_TEAMS>;

  using Game = QPair<team_t, team_t>;
  using Schedule = QMap<week_t, QVector<Game>>;

}

#endif
