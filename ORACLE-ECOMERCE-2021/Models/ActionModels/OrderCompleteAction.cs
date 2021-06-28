using Oracle.ManagedDataAccess.Client;
using Repository.DomainModels;
using Repository.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORACLE_ECOMERCE_2021.Models.ActionModels
{
    public class OrderCompleteAction: ControllerAction
    {
        private ISQLHelper _db;

        public OrderCompleteAction()
        {
            var conn = new OracleConnection(_connStr);
            _db = new OracleHelper(conn);
        }

        public void Execute(List<CartItem> cartItems)
        {
            // Create New Order
            //
            _db.ExecuteReader("SELECT MAX(ID) FROM ORDERS");
            var orders = _db.FetchRowSet<Order>();

            int orderId = 1;
            if (orders != null)
                orderId = orders.First().ID + 1;

            _db.ExecuteNonQuery(@$"
                INSERT INTO ORDERS ( id )
                WITH recs AS ( 
                    SELECT {orderId} as id FROM dual
                )
                SELECT * FROM recs"
            );

            // Add Order Lines
            List<string> insertVals = new List<string>();
            foreach (var item in cartItems)
            {
                var p = item.Product;
                insertVals.Add(@$"
                    SELECT {orderId} as order_id, {p.ID} as product_id, {p.UNIT_PRICE} as unit_price, {item.Quantity} as quantity, '{p.NAME}' as product_name FROM dual 
                ");
            }

            _db.ExecuteNonQuery(@$"
                INSERT INTO ORDER_LINES(order_id, product_id, unit_price, quantity, product_name)
                WITH recs AS (
                {string.Join("UNION ALL", insertVals) }
                )
                SELECT* FROM recs
            ");

            _db.Dispose();

        }
    }
}
