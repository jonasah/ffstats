#ifndef GAME_RESULT_H
#define GAME_RESULT_H

#include "typedefs.h"

// result of a single game
struct GameResult {
  team_t win;
  team_t loss;

  GameResult() = default;
  GameResult(const team_t w, const team_t l) : win(w), loss(l) {}
};


// game results for one week
using WeekResults = std::array<GameResult, NUM_TEAMS / 2>;

#endif
