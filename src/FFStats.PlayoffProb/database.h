#ifndef DATABASE_H
#define DATABASE_H

#include <QtSql/QSqlDatabase>
#include <QtSql/QSqlError>
#include <QtSql/QSqlField>
#include <QtSql/QSqlQuery>
#include <QtSql/QSqlTableModel>

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


class QueryInterface {
public:
  virtual ~QueryInterface() = default;

  virtual bool execute() = 0;
};

class SelectQuery : QueryInterface {
public:
  class const_iterator;

  SelectQuery(const QString& table, const QString& filter) {
    m_model.setTable(table);
    m_model.setFilter(filter);
  }

  void setSort(int column, Qt::SortOrder sort_order) {
    m_model.setSort(column, sort_order);
  }

  bool execute() override {
    return m_model.select();
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


class InsertQuery : QueryInterface {
public:
  explicit InsertQuery(const QString& table) {
    m_model.setTable(table);
    m_model.setEditStrategy(QSqlTableModel::OnManualSubmit);
  }

  bool execute() override {
    return m_model.submitAll();
  }

  void addRow(const QHash<QString, QVariant>& column_data) {
    const auto row = m_model.rowCount();
    m_model.insertRow(row);

    for (auto it = column_data.cbegin(), e = column_data.cend(); it != e; ++it) {
      m_model.setData(m_model.index(row, m_model.fieldIndex(it.key())), it.value());
    }
  }

private:
  QSqlTableModel m_model;
};


class DeleteQuery : QueryInterface {
public:
  DeleteQuery(const QString& table, const QString& filter) :
    m_query(QStringLiteral("DELETE FROM %1 WHERE %2").arg(table, filter))
  {
  }

  bool execute() override {
    return m_query.exec();
  }

private:
  QSqlQuery m_query;
};

#endif
