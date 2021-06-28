using Repository.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace Repository.Extentions
{
    /*
        Lưu ý khi sử dụng :
            - gán [OutputParameter] attributr cho tham số dạng output 
              ( phải khác null hoặc không mang kiểu nullable type )
  
            - ToList<ResultModel> nên dùng domain model được generate bởi Entity Framework để tránh không 
              tương thích kiểu dữ liệu , tên biến với database .... 
    */
    public static class SqlClientExtention
    {
        public static OracleParameterCollection AddParams<ParamModel>(this OracleParameterCollection paramsCollection
            , ParamModel model, Type modelType)
            where ParamModel : class
        {
            IDictionary<string, object> parameters = model.ToDictionary();

            foreach (var item in parameters)
            {
                if (Attribute.IsDefined(modelType.GetProperty(item.Key), typeof(ExcludeParameterAttribute)))
                    continue;

                    var sqlParameter = new OracleParameter($"@{item.Key.ToLower()}", item.Value);
                if (Attribute.IsDefined(modelType.GetProperty(item.Key), typeof(OutputParameterAttribute)))
                {
                    sqlParameter.Direction = ParameterDirection.Output;
                }
                else if (item.Value == null)
                {
                    // không truyền null vào sp vì sẽ bị mất default value ở sp
                    continue;
                }

                paramsCollection.Add(sqlParameter);
            }

            return paramsCollection;
        }

        public static OracleParameterCollection AddParams(this OracleParameterCollection paramsCollection, object model)
        {
            IDictionary<string, object> parameters = model.ToDictionary();
            foreach (var item in parameters)
            {
                if (item.Value == null)
                {
                    continue;
                }

                var sqlParameter = new OracleParameter($"@{item.Key.ToLower()}", item.Value);
                paramsCollection.Add(sqlParameter);
            }

            return paramsCollection;
        }

        public static object GetValueOf(this OracleDataReader reader, int i, string type)
        {
            string[] typeArr = new string[]
            {
                typeof(int).ToString(),     // 0
                typeof(String).ToString(),  // 1
                typeof(string).ToString(),  // 2
                typeof(float).ToString(),   // 3
                typeof(double).ToString()   // 4
            };

            int? action = null;
            for (int j = 0; j < typeArr.Length; j++)
            {
                if (typeArr[j] == type)
                {
                    action = j;
                    break;
                }
            }

            switch (action)
            {
                case 0:
                    return reader.GetInt32(i);
                case 1:
                case 2:
                    return reader.GetString(i);
                case 3:
                    return reader.GetFloat(i);
                case 4:
                    return reader.GetDouble(i);
                default:
                    return "Data Type is not declared. Can not convert!";
            }
        }

        public static List<T> ToList<T>(this OracleDataReader reader)
            where T : class, new()
        {
            if (reader.HasRows == false)
            {
                return null;
            }
            
            var result = new List<T>();
            while (reader.Read())
            {
                var ModelInstance = new T();
                for (int i = reader.FieldCount - 1; i >= 0; i--)
                {
                    var name = reader.GetName(i);

                    if (ModelInstance.GetType().GetProperty(name) == null)
                        continue;

                    if( reader.IsDBNull(i) == false)
                    {
                        var dataType = ModelInstance.GetType()
                            .GetProperty(name).PropertyType.ToString();

                        var dbValue = reader.GetValueOf(i, dataType);

                        try
                        {
                            ModelInstance.SetPropertyValue(name, dbValue);
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                }
                result.Add(ModelInstance);
            }

            reader.NextResult();
            return result;
        }

        public static List<List<T>> ToVariableList<T>(this OracleDataReader reader)
        {
            if (reader.HasRows == false)
            {
                return null;
            }

            var result = new List<List<T>>();
            while (reader.Read())
            {
                var list = new List<T>();
                for (int i = reader.FieldCount - 1; i >= 0; i--)
                {
                    if (reader.IsDBNull(i) == false)
                    {
                        var dataType = typeof(T).ToString();
                        var dbValue = reader.GetValueOf(i, dataType);

                        try
                        {
                            list.Add((T)dbValue);
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                }
                result.Add(list);
            }

            reader.NextResult();
            return result;
        }
    }
}
