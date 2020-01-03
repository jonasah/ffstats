#ifndef TYPEDEFS_H
#define TYPEDEFS_H

#include <array>
#include <map>
#include <vector>

#define NUM_TEAMS 10
#define NUM_PLAYOFF_TEAMS 6

namespace ffstats::playoffprob {

  using team_t = uint16_t;
  using year_t = uint16_t;
  using week_t = uint16_t;
  using rank_t = uint16_t;
  using win_t = uint16_t;
  using loss_t = uint16_t;
  using win_loss_t = int16_t;

  // std::array with size NUM_TEAMS
  template <typename T>
  using TeamAssociativeArray = std::array<T, NUM_TEAMS>;

  using Game = std::pair<team_t, team_t>;
  using Schedule = std::map<week_t, std::vector<Game>>;

}

#endif
