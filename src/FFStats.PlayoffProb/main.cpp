#include <QtCore/QCommandLineParser>

#include "playoffprobcalculator.h"

int main(int argc, char** argv) {
  QCoreApplication app(argc, argv);

  QCommandLineParser parser;
  parser.addOption(QCommandLineOption("year", "Year", "year"));
  parser.addOption(QCommandLineOption("week", "From week", "week", "1"));

  if (!parser.parse(app.arguments())) {
    qWarning() << parser.errorText();
    return 1;
  }

  if (!parser.isSet("year")) {
    qWarning() << "Year is missing";
    return 1;
  }

  year_t year = parser.value("year").toUShort();
  week_t week = parser.value("week").toUShort();

  Q_ASSERT(week >= 1 && week <= 14);

  PlayoffProbCalculator::calculate(year, week);

  return 0;
}
