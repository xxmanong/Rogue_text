using System;
using System.Collections.Generic;

namespace okcopy.DataAccess
{

    [Serializable]
    public class DbObject : Dictionary<string, object>
    {

        /// <summary>
        /// 重写构造函数
        /// <para>字典名称忽略大小写匹配</para>
        /// </summary>
        public DbObject()
            : base(StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// 重写构造函数
        /// <para>字典名称忽略大小写匹配</para>
        /// </summary>
        /// <param name="dictionary"></param>
        public DbObject(IDictionary<string, object> dictionary)
            : base(dictionary, StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// 重写
        /// <para>不存在的字典返回null, 不作异常处理</para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new public object this[string key]
        {
            get { return this.ContainsKey(key) ? base[key] : null; }
            set { base[key] = value; }
        }

        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            this[key] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] GetNames()
        {
            var em = this.Keys.GetEnumerator();
            List<string> names = new List<string>();

            while (em.MoveNext())
                names.Add(em.Current.ToString());

            return names.ToArray();
        }

        /// <summary>
        /// GetString
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public String GetString(string name)
        {
            return this.GetValue<String>(name);
        }
        /// <summary>
        /// Boolean
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Boolean GetBoolean(string name)
        {
            return this.GetValue<Boolean>(name);
        }
        /// <summary>
        /// Get Int16
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Int16 GetInt16(string name)
        {
            return this.GetValue<Int16>(name);
        }
        /// <summary>
        /// Get Int32
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Int32 GetInt32(string name)
        {
            return this.GetValue<Int32>(name);
        }
        /// <summary>
        /// Get Int64
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Int64 GetInt64(string name)
        {
            return this.GetValue<Int64>(name);
        }
        /// <summary>
        /// Get Double value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Double GetDouble(string name)
        {
            return this.GetValue<Double>(name);
        }
        /// <summary>
        /// Get Decimal
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Decimal GetDecimal(string name)
        {
            return this.GetValue<Decimal>(name);
        }

        /// <summary>
        /// GetDateTime
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DateTime GetDateTime(string name)
        {
            string val = this.GetString(name);

            if (string.IsNullOrEmpty(val))
                return new DateTime(1900, 1, 1);

            long timeStamp = 0;
            if (long.TryParse(val, out timeStamp))
                return UnixTimeToTime(val);

            return Convert.ToDateTime(val);
        }
        private DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// GetValue
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetValue(string name)
        {
            return this[name];
        }

        /// <summary>
        /// get DbObject
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DbObject GetObject(string name)
        {
            var jObj = this[name] as Newtonsoft.Json.Linq.JObject;
            if (jObj == null) return null;

            return (DbObject)jObj.ToObject(typeof(DbObject));
        }
        /// <summary>
        /// get DbArray
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsNullOrEmpty(string name)
        {
            return string.IsNullOrEmpty(GetString(name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DbArray GetArray(string name)
        {

            if (this[name] is DbArray)
                return (DbArray)this[name];

            var jArr = this[name] as Newtonsoft.Json.Linq.JArray;
            if (jArr == null) return null;

            DbArray arr = new DbArray();

            foreach (Newtonsoft.Json.Linq.JObject obj in jArr)
            {
                arr.Add((DbObject)obj.ToObject(typeof(DbObject)));
            }

            return arr;
        }

        /// <summary>
        /// getValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetValue<T>(string name)
        {
            if (this[name] == DBNull.Value || this[name] == null)
                return default(T);

            return (T)Convert.ChangeType(this[name], typeof(T));
        }

    }
}
