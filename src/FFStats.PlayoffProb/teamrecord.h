#ifndef TEAM_RECORD_H
#define TEAM_RECORD_H

#include "typedefs.h"

namespace ffstats::playoffprob {

// a team's win/loss record and head to head record against other teams
struct TeamRecord {
  team_t team;
  win_t win;
  loss_t loss;

  // head to head record, positive more wins, negative more losses
  TeamAssociativeArray<win_loss_t> head2head;

  TeamRecord() : team(NUM_TEAMS), win(0), loss(0) {}
  TeamRecord(const team_t t, const win_t w, const loss_t l) : team(t), win(w), loss(l) {}

  void setHead2HeadRecord(const team_t vs, const win_loss_t h2h) { head2head[vs] = h2h; }
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

}

#endif
