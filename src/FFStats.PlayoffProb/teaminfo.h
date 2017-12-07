#ifndef TEAM_INFO_H
#define TEAM_INFO_H

#include "typedefs.h"

namespace ffstats::playoffprob {

  class TeamInfo {
  public:
    static void addName(const team_t database_id, const QString& name) {
      s_team_names.insert(database_id, name);
    }

    static QString getNameForDatabaseId(const team_t database_id) {
      return s_team_names[database_id];
    }

    static QString getNameForTemporaryId(const team_t temp_id) {
      return getNameForDatabaseId(getDatabaseId(temp_id));
    }

    static void addConversion(const team_t database_id, const team_t temp_id) {
      s_db_to_temp_table.insert(database_id, temp_id);
      s_temp_to_db_table.insert(temp_id, database_id);
    }

    static bool exists(const team_t database_id) {
      return s_db_to_temp_table.contains(database_id);
    }

    static team_t getDatabaseId(const team_t temp_id) {
      return s_temp_to_db_table[temp_id];
    }

    static auto getDatabaseIds() {
      return s_db_to_temp_table.keys();
    }

    static team_t getTemporaryId(const team_t database_id) {
      return s_db_to_temp_table[database_id];
    }

  private:
    // team name lookup table using original team ids
    static QHash<team_t, QString> s_team_names;
    // conversion from team id in database to temporary team id
    static QHash<team_t, team_t> s_db_to_temp_table;
    // conversion from temporary team id to team id in database
    static QHash<team_t, team_t> s_temp_to_db_table;
  };

  QHash<team_t, QString> TeamInfo::s_team_names;
  QHash<team_t, team_t> TeamInfo::s_db_to_temp_table;
  QHash<team_t, team_t> TeamInfo::s_temp_to_db_table;

}

#endif
