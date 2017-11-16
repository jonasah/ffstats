#ifndef DATABASE_H
#define DATABASE_H

#include <QtSql/QSqlDatabase>
#include <QtSql/QSqlError>

class Database {
public:
  static bool init() {
    auto db = QSqlDatabase::addDatabase("QSQLITE");
    db.setDatabaseName(R"(..\FFStats.DbHandler\ffstats.db)");
    return db.open();
  }

  static QString getLastError() {
    return QSqlDatabase::database().lastError().text();
  }
};


class SelectQuery {
public:
  class const_iterator;

  SelectQuery(const QString& table, const QString& filter) {
    m_model.setTable(table);
    m_model.setFilter(filter);
  }

  void setSort(int column, Qt::SortOrder sort_order) {
    m_model.setSort(column, sort_order);
  }

  void execute() {
    m_model.select();
  }

  auto rowCount() const {
    return m_model.rowCount();
  }

  const_iterator begin() const {
    return const_iterator(&m_model, 0);
  }

  const_iterator end() const {
    return const_iterator(&m_model, m_model.rowCount());
  }

  class const_iterator {
  public:
    explicit const_iterator(const QSqlTableModel* const model, const int row) :
      c_model(model),
      m_row(row)
    {
    }

    int row() const {
      return m_row;
    }

    bool operator!=(const const_iterator& rhs) {
      return m_row != rhs.m_row;
    }

    const_iterator& operator++() {
      ++m_row;
      return *this;
    }

    QSqlRecord operator*() const {
      return c_model->record(m_row);
    }

  private:
    const QSqlTableModel* const c_model;
    int m_row;
  };

private:
  QSqlTableModel m_model;
};

#endif
