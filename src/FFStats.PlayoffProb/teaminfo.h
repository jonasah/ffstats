#ifndef TEAM_INFO_H
#define TEAM_INFO_H

#include "typedefs.h"

namespace ffstats::playoffprob {

class TeamInfo {
 public:
  static void addConversion(const team_t database_id, const team_t temp_id) {
    s_database_ids.push_back(database_id);
    s_db_to_temp_table.insert({database_id, temp_id});
    s_temp_to_db_table.insert({temp_id, database_id});
  }

  static bool exists(const team_t database_id) {
    return s_db_to_temp_table.contains(database_id);
  }

  static team_t getDatabaseId(const team_t temp_id) {
    return s_temp_to_db_table[temp_id];
  }

  static const auto& getDatabaseIds() { return s_database_ids; }

  static team_t getTemporaryId(const team_t database_id) {
    return s_db_to_temp_table[database_id];
  }

 private:
  // list of team ids in database
  static std::vector<team_t> s_database_ids;
  // conversion from team id in database to temporary team id
  static std::map<team_t, team_t> s_db_to_temp_table;
  // conversion from temporary team id to team id in database
  static std::map<team_t, team_t> s_temp_to_db_table;
};

std::vector<team_t> TeamInfo::s_database_ids;
std::map<team_t, team_t> TeamInfo::s_db_to_temp_table;
std::map<team_t, team_t> TeamInfo::s_temp_to_db_table;

}

#endif
