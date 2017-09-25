using okcopy.DataAccess; 
//using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace okcopy.DataAccess
{

  /// <summary>
  /// 
  /// </summary>
  public abstract class DbFactory : IDisposable
  {

    /// <summary>
    /// 
    /// </summary>
    public DbFactory()
    {
      _conn = CreateConnection();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cnstr"></param>
    protected DbFactory(DbContextType type, string cnstr)
    {
      _conn = CreateConnection(type, cnstr);
    }

    #region opertions

    #region Query

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public DbObject Single(string sql, params object[] args)
    {
      var arr = Query(sql, args);
      return arr != null && arr.Count > 0 ? arr[0] : null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public T Single<T>(string sql, params object[] args)
    {
      var arr = Query<T>(sql, args);
      return arr != null && arr.Count > 0 ? arr[0] : default(T);
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <returns></returns>
    public DbArray Query(string sql, params object[] args)
    {
      var list = new DbArray();
      this.Open();
      using (DbCommand cmd = _conn.CreateCommand())
      {

        cmd.CommandText = sql;
        cmd.Transaction = _tran;
        AddDbParameter(cmd, args);

        using (DbDataReader dr = cmd.ExecuteReader())
        {
          while (dr.Read())
          {
            DbObject obj = new DbObject();
            for (var i = 0; i < dr.FieldCount; i++)
              obj.Add(dr.GetName(i), dr.GetValue(i));

            list.Add(obj);
          }
          dr.Close();
        }
      }

      return list;
    }

    /// <summary>
    /// 查询（返回对象集）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public List<T> Query<T>(string sql, params object[] args)
    {
      var list = new List<T>();
      foreach (var obj in Query(sql, args))
      {
        var type = typeof(T);
        var newObj = Activator.CreateInstance<T>();

        foreach (var pi in type.GetProperties())
        {
          var val = obj.GetValue(pi.Name);
          if (val != DBNull.Value && val != null)
          {
            val = Convert.ChangeType(val, pi.PropertyType);
            pi.SetValue(newObj, val);
          }

        }

        list.Add(newObj);
      }
      return list;

    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="paging"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public DbPagingResult PageQuery(DbPagingParams paging, params object[] args)
    {
      var sql = BuildPagingSqlString(paging); // 生成分页脚本
      var data = this.Query(sql, args);

      // get summary
      var obj = this.GetSummary(paging.SqlString, paging.SumamryFields, args);
      var total = obj.GetInt64("CNT");

      return new DbPagingResult
      {
        total = total,
        data = data,
        summary = total > 0 ? obj : null
      };

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sql">SQL语句</param>
    /// <param name="fields">汇总字段（或表达式）</param>
    /// <param name="args">SQL查询参数</param>
    /// <returns></returns>
    public DbObject GetSummary(string sql, string[] fields, object[] args)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("SELECT CNT=COUNT(1) ");

      if (fields != null)
      {
        foreach (var field in fields)
        {
          if (field.IndexOf('=') > -1)
            sb.AppendFormat(", {0}", field);
          else
            sb.AppendFormat(", [{0}]=SUM([{0}])", field);
        }
      }

      sb.AppendFormat(" FROM ({0}) t ", sql);

      return this.Single(sb.ToString(), args);

    }


    #endregion

    #region Execute

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public int Execute(string sql, params object[] args)
    {
      using (DbCommand cmd = _conn.CreateCommand())
      {
        this.Open();

        cmd.CommandText = sql;
        cmd.Transaction = _tran;

        AddDbParameter(cmd, args);
        return cmd.ExecuteNonQuery();
      }
    }

    #endregion

    #region ExecuteProc

    /// <summary>
    /// 执行存储过程
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public int ExecuteProc(string sql, params object[] args)
    {
      using (DbCommand cmd = _conn.CreateCommand())
      {
        this.Open();

        cmd.CommandText = sql;
        cmd.Transaction = _tran;
        cmd.CommandType = CommandType.StoredProcedure;

        AddDbParameter(cmd, args);
        return cmd.ExecuteNonQuery();
      }
    }

    #endregion

    #region ExecuteScalar

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public bool Exists(string sql, params object[] args)
    {
      return GetRowCount(sql, args) > 0;
    }
    /// <summary>
    /// 汇总
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public long GetRowCount(string sql, params object[] args)
    {
      sql = string.Format("SELECT COUNT(*) cnt from ({0}) t", sql);
      return this.ExecuteScalar<Int64>(sql, args);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public T ExecuteScalar<T>(string sql, params object[] args)
    {
      var val = this.ExecuteScalar(sql, args);
      return val == null || val == DBNull.Value ? default(T)
          : (T)Convert.ChangeType(val, typeof(T));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public object ExecuteScalar(string sql, params object[] args)
    {

      using (DbCommand cmd = _conn.CreateCommand())
      {
        this.Open();

        cmd.CommandText = sql;
        cmd.Transaction = _tran;

        AddDbParameter(cmd, args);
        return cmd.ExecuteScalar();
      }
    }

    #endregion

    #region Insert

    /// <summary>
    /// 插入
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int Insert(string tableName, object obj)
    {

      var str = _type == DbContextType.MySql ? "?" :
          _type == DbContextType.Oracle ? ":" : "@";

      var data = ConvertToDictionary(obj);

      // 插入时移除主键
      if (obj is IDbTableModel)
      {
        var pKeys = ((IDbTableModel)obj).GetPrimaryKeys();
        foreach (var k in pKeys)
        {
          if (!data.ContainsKey(k)) continue;
          data.Remove(k);
        }
      }

      // 
      var sql = string.Format("INSERT INTO {0}({2}) VALUES ({1}{3})",
          tableName, str, string.Join(",", data.Keys), string.Join("," + str, data.Keys));

      using (var cmd = _conn.CreateCommand())
      {
        this.Open();
        cmd.CommandText = sql;
        cmd.Transaction = _tran;

        foreach (string key in data.Keys)
        {
          var p = cmd.CreateParameter();
          p.ParameterName = key;
          p.Value = data[key] ?? DBNull.Value;
          cmd.Parameters.Add(p);
        }

        return cmd.ExecuteNonQuery();
      }

    }

    /// <summary>
    /// 获取最近一次产生的Identity值
    /// <para>用于新增后得到唯一ID</para>
    /// <para>Oracle暂不支持</para>
    /// </summary>
    /// <param name="tr">事务</param>
    /// <returns></returns>
    public long GetIdentity()
    {
      switch (_type)
      {
        case DbContextType.Oracle:
          throw new NotImplementedException();
        case DbContextType.MySql:
          return ExecuteScalar<long>("SELECT LAST_INSERT_ID()", null);
        default:
          return ExecuteScalar<long>("SELECT @@IDENTITY AS Id", null);
      }
    }

    #endregion

    #region Update

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="tableName">要更新的表名</param>
    /// <param name="values">更新字段及对应值 </param>
    /// <param name="where">where类型是String表示为Where语句</param>
    /// <returns></returns>
    public int Update(string tableName, object values, object where)
    {
      if (_type == DbContextType.Oracle)
      {
        throw new NotImplementedException("该方法暂不支持Oracle!");
      }

      var str = _type == DbContextType.MySql ? "?" : "@";
      var fields = ConvertToDictionary(values);

      var sb = new StringBuilder();
      sb.AppendFormat("UPDATE {0} ", tableName);
      sb.AppendFormat("SET {0} ", string.Join(", ",
          fields.Keys.ToList().ConvertAll<string>(key => key + "=" + str + key).ToArray()));

      using (var cmd = _conn.CreateCommand())
      {
        foreach (string key in fields.Keys)
        {
          var p = cmd.CreateParameter();
          p.ParameterName = key;
          p.Value = fields[key] ?? DBNull.Value;
          cmd.Parameters.Add(p);
        }

        if (where != null)
        {
          sb.Append("WHERE ");
          if (where is String)
          {
            sb.Append(where.ToString());
          }
          else
          {
            var wheres = ConvertToDictionary(where);

            sb.Append(string.Join(" and ",  // eg: field1=@_field1;
             wheres.Keys.ToList().ConvertAll<string>(key => key + "=" + str + "_" + key).ToArray()));

            foreach (string key in wheres.Keys)
            {
              var p = cmd.CreateParameter();
              p.ParameterName = "_" + key;
              p.Value = wheres[key] ?? DBNull.Value;
              cmd.Parameters.Add(p);
            }

          }
        }

        cmd.CommandText = sb.ToString();
        cmd.Transaction = _tran;

        this.Open();
        return cmd.ExecuteNonQuery();
      }

    }

    #endregion

    #endregion

    #region base

    private DbConnection _conn = null;
    private DbTransaction _tran = null;
    private DbContextType _type = DbContextType.Default;

    /// <summary>
    /// 创建默认连接
    /// </summary>
    /// <returns></returns>
    protected abstract DbConnection CreateConnection();
    /// <summary>
    /// 创建数据库连接
    /// </summary>
    /// <param name="cnstr"></param>
    /// <returns></returns>
    protected virtual DbConnection CreateConnection(DbContextType type, string cnstr)
    {

      _type = type;


      return new SqlConnection(cnstr); // default or mssql;
      //switch (type)
      //{
      //  case DbContextType.MySql:
      //    return new MySqlConnection(cnstr);

      //  case DbContextType.Oracle:
      //    return new OracleConnection(cnstr);

      //  default:
      //    return new SqlConnection(cnstr); // default or mssql;
      //}

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public void BeginTransaction()
    {
      this.Open();
      _tran = _conn.BeginTransaction();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public void Commit()
    {
      if (_tran != null)
        _tran.Commit();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public void Rollback()
    {
      if (_tran != null)
        _tran.Rollback();
    }

    /// <summary>
    /// 
    /// </summary>
    protected void Open()
    {
      if (_conn != null && _conn.State == ConnectionState.Closed)
        _conn.Open();
    }

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    public void Close()
    {
      try { _tran.Rollback(); }
      catch { }

      try { _conn.Close(); }
      catch { }
    }

    /// <summary>
    /// 销毁时关闭连接
    /// </summary>
    public void Dispose()
    {

      if (_conn.State == ConnectionState.Open
          || _conn.State == ConnectionState.Broken)
      {
        this.Close();
      }
    }
    #endregion

    #region tools

    // add db params
    private void AddDbParameter(DbCommand cmd, object[] args)
    {

      if (args == null || args.Length == 0) return;

      cmd.Parameters.Clear();

      for (var i = 0; i < args.Length; i++)
      {

        if (args[i] is DbParameter)
        {
          cmd.Parameters.Add(args[i]);
        }
        else if (args[i] is IDictionary)
        {
          var em = ((IDictionary)args[i]).GetEnumerator();

          while (em.MoveNext())
          {
            var pp = cmd.CreateParameter();
            pp.ParameterName = Convert.ToString(em.Key);
            pp.Value = em.Value ?? DBNull.Value;
            cmd.Parameters.Add(pp);
          }
        }
        //else if (args[i] is DbObject)
        //{
        //    var obj = ((DbObject)args[i]);
        //    foreach (var p in obj.keys)
        //    {
        //        var pp = cmd.CreateParameter();
        //        pp.ParameterName = p.Name;
        //        pp.Value = p.GetValue(args[0], null) ?? DBNull.Value;
        //        cmd.Parameters.Add(pp);
        //    }
        //}
        else if (args[i].GetType().Name.StartsWith("<>f__AnonymousType") || args[i] is IDbTableModel)
        {
          foreach (var p in args[0].GetType().GetProperties())
          {
            var pp = cmd.CreateParameter();
            pp.ParameterName = p.Name;
            pp.Value = p.GetValue(args[0], null) ?? DBNull.Value;
            cmd.Parameters.Add(pp);
          }
        }
        else
        {
          var pp = cmd.CreateParameter();
          if (_type != DbContextType.MySql)
            pp.ParameterName = "p" + i;

          pp.Value = args[i] ?? DBNull.Value;
          cmd.Parameters.Add(pp);
        }
      }
    }

    // build paging sql string 
    private string BuildPagingSqlString(DbPagingParams paging)
    {

      StringBuilder sb = new StringBuilder();

      if (_type == DbContextType.MySql)
      {
        sb.AppendFormat("SELECT * FROM ({0}) as t ", paging.SqlString);

        if (!string.IsNullOrEmpty(paging.SortField))
          sb.AppendFormat(" order by {0} {1} ", paging.SortField, paging.SortOrder);

        sb.AppendFormat("LIMIT {0},{1} ", paging.PageIndex * paging.PageSize, paging.PageSize);
      }
      else
      {
        if (string.IsNullOrEmpty(paging.SortField))
          throw new Exception("SQL分页查询必须指定排序字段");

        var start = paging.PageIndex * paging.PageSize;
        var end = start + paging.PageSize;

        sb.Append("select * from (");
        sb.Append("     select *, ROW_NUMBER() OVER (");
        sb.AppendFormat(" ORDER BY {0} {1} ", paging.SortField, paging.SortOrder);
        sb.AppendFormat(") as RN from ({0}) as gg", paging.SqlString);
        sb.AppendFormat(") as t where t.RN between {0} and {1}", start + 1, end);

      }

      return sb.ToString();
    }


    // 
    private Dictionary<string, object> ConvertToDictionary(object obj)
    {
      if (obj is IDictionary)
      {
        var em = ((IDictionary)obj).GetEnumerator();
        var data = new Dictionary<string, object>();

        while (em.MoveNext())
          data.Add(em.Key.ToString(), em.Value);

        return data;
      }
      else
      {
        return obj.GetType().GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(obj, null));
      }
    }

    #endregion


  }
}
