using Repository.Extentions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Oracle.ManagedDataAccess.Client;

namespace Repository.SqlDataProvider
{
    public interface ISQLHelper: IDisposable
    {
        public int ExecuteNonQuery(string text, bool isStoreProcedure = false, object p = null);
        public void ExecuteReader(string text, bool isStoreProcedure = false, object p = null);

        public List<TResult> FetchRowSet<TResult>() where TResult : class, new();

        public new void Dispose();
    }
}
