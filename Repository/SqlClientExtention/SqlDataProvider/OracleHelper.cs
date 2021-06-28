using Repository.Extentions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Oracle.ManagedDataAccess.Client;

namespace Repository.SqlDataProvider
{
    public class OracleHelper : IDisposable, ISQLHelper
    {
        private OracleConnection _connection;
        private OracleCommand _command;
        private OracleDataReader _reader;
        
        public OracleHelper(OracleConnection connection) =>
            _connection = connection;

        private void AddParameter(object parameters)
        {
            if (parameters.GetType().IsAnonymousType())
                _command.Parameters.AddParams(parameters);
            else
                _command.Parameters.AddParams(parameters, parameters.GetType());
        }

        public void ReturnOutputParameters(object parameters)
        {
            foreach (OracleParameter p in _command.Parameters)
            {
                if (p.Direction.Equals(ParameterDirection.Output))
                {
                    string paramName = p.ParameterName.Substring(1);
                    parameters.SetPropertyValue(paramName, p.Value);
                }
            }
        }

        public List<TResult> FetchRowSet<TResult>()
            where TResult : class, new()
        {
            return _reader.ToList<TResult>();
        }

        public List<List<TResult>> FetchVariableList<TResult>()
        {
            return _reader.ToVariableList<TResult>();
        }

        public void ExecuteReader(string text, bool isStoreProcedure = false, object p = null)
        {
            _command = new OracleCommand(text, _connection);
            _command.CommandTimeout = 120;

            if (isStoreProcedure == true)
                _command.CommandType = CommandType.StoredProcedure;
            else
                _command.CommandType = CommandType.Text;

            if (p != null)
                this.AddParameter(p);

            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            _reader = _command.ExecuteReader();
        }

        public int ExecuteNonQuery(string text, bool isStoreProcedure = false, object p = null)
        {
            _command = new OracleCommand(text, _connection);
            _command.CommandTimeout = 120;

            if (isStoreProcedure == true)
                _command.CommandType = CommandType.StoredProcedure;
            else
                _command.CommandType = CommandType.Text;

            if (p != null)
                this.AddParameter(p);

            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            return _command.ExecuteNonQuery();
        }

        public void Finish()
        {
            _command.Dispose();
        }

        public void Dispose()
        {
            if (_connection != null)
                _connection.Close();
            if (_reader != null && _reader.IsClosed == false)
                _reader.Close();
            if(_command != null)
                _command.Dispose();
        }
    }
}
