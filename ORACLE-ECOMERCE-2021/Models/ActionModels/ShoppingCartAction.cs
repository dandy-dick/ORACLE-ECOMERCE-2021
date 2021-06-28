using Oracle.ManagedDataAccess.Client;
using Repository.DomainModels;
using Repository.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORACLE_ECOMERCE_2021.Models.ActionModels
{
    public class ShoppingCartAction: ControllerAction
    {
        private ISQLHelper _db;

        public ShoppingCartAction()
        {
            var conn = new OracleConnection(_connStr);
            _db = new OracleHelper(conn);
        }

        public List<Product> Execute(string[] ids)
        {
            string query = "SELECT * FROM Products ";
            query += $" WHERE id IN ({ string.Join(',', ids) })";
            _db.ExecuteReader(query);
            var items = _db.FetchRowSet<Product>();

            _db.Dispose();

            return items;
        }
    }
}
