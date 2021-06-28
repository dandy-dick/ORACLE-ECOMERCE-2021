using Oracle.ManagedDataAccess.Client;
using Repository.DomainModels;
using Repository.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORACLE_ECOMERCE_2021.Models.ActionModels
{
    public class GetProductListAction: ControllerAction
    {
        private ISQLHelper _db;

        public GetProductListAction()
        {
            var conn = new OracleConnection(_connStr);
            _db = new OracleHelper(conn);
        }

        public List<Product> Execute()
        {
            string query = "SELECT * FROM Products FETCH NEXT 10 ROWS ONLY";
            _db.ExecuteReader(query);
            var products = _db.FetchRowSet<Product>();

            

            _db.Dispose();

            return products;
        }
    }
}
