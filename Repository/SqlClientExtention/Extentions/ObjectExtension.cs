using Repository.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Repository.Extentions
{
    public static class ObjectExtension
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static bool IsAnonymousType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }

        public static void SetPropertyValue(this object src , string propertyName , object value)
        {
            var ModelType = src.GetType();
            var _propertyInfo = ModelType.GetProperty(propertyName);

            if (_propertyInfo == null)
                return;

            var _targetType = _propertyInfo.PropertyType.IsNullableType() ?
                        Nullable.GetUnderlyingType(_propertyInfo.PropertyType) : _propertyInfo.PropertyType;
            
            var _value = Convert.ChangeType(value, _targetType);
            _propertyInfo.SetValue(src, _value, null);
        }

        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var ModelInstance = new T();

            foreach (var item in source)
            {
                ModelInstance.SetPropertyValue(item.Key, item.Value);
            }
            return ModelInstance;
        }

        public static IDictionary<string, object> ToDictionary(this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }

        public static int IndexOfAny(this string obj , string[] values , int startIndex = 0)
        {
            int index_of_firstmatch = -1;
            foreach (var val in values)
            {
                index_of_firstmatch = obj.IndexOf(val , startIndex);
                if (index_of_firstmatch != -1)
                    break;
            }

            return index_of_firstmatch;
        }

        public static int IndexOfAny(this string obj, List<string> values, int startIndex = 0)
        {
            int index_of_firstmatch = -1;
            string objLowerCase = obj.ToLower();
            foreach (var val in values)
            {
                index_of_firstmatch = objLowerCase.IndexOf(val.ToLower(), startIndex);
                if (index_of_firstmatch != -1)
                    break;
            }

            return index_of_firstmatch;
        }

        private static readonly string[] VietNamChar = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };

        public static string LocDau(this string str)
        {
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return str;
        }

        public static int GetNthIndex(this string s, char t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        
        /*
        Giống ObjectAssign của javascript, 
        */
        public static void ObjectAssign<TSource, TTarget>(this TTarget target, TSource src)
        {
            PropertyInfo[] properties = typeof(TTarget).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var key = property.Name;
                bool hasProperty = src.GetType().GetProperty(key) != null;
                if (hasProperty)
                {
                    var value = src.GetType()
                            .GetProperty(key)
                            .GetValue(src, null);

                    property.SetValue(target, value);
                }
            }
        }

        /*
         Giống ObjectAssign của javascript, có hỗ trợ "exclude" các trường không muốn assign
         */
        public static void AssignProperties<TSource, TTarget>(this TTarget target, TSource src)
            where TSource : class, new()
            where TTarget : class, new()
        {
            PropertyInfo[] properties = typeof(TTarget).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var key = property.Name;
                bool hasProperty = src.GetType().GetProperty(key) != null;
                if (hasProperty)
                {
                    // check for exclude
                    var excluded = (string)src.PropAttrValue
                                                <TSource, ExcludeParameterAttribute>(key, "Name");
                    if (excluded == "AssignProperties")  // bo qua 
                        continue;

                    var value = src.GetType()
                            .GetProperty(key)
                            .GetValue(src, null);
                    property.SetValue(target, value);
                }
            }
        }
        /*
            Lấy giá trị key từ attribute của một property của object
            property hoặc attr không tồn tại thì throw
         */
        public static object PropAttrValue<TSrc, TAttr>(this TSrc src, string propertyName, string attrName)
                    where TAttr : class
                    where TSrc : class
        {
            var srcType = typeof(TSrc);
            var attrType = typeof(TAttr);

            var prop = srcType.GetProperty(propertyName);
            if (prop != null)
            {
                var attr = prop.GetCustomAttribute(attrType);
                if (attr != null)
                {
                    try
                    {
                        var result = attrType.GetProperty(attrName).GetValue(attr, null);
                        return result;

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }

            return null;
        }

        /*
         Lấy giá trị 1 trường nếu có hoặc null nếu không
         */
        public static object GetPropertyValue<TSource>(this TSource src, string key)
                    where TSource : class, new()
        {
            var type = src.GetType();
            bool hasProperty = type.GetProperty(key) != null;
            if (hasProperty)
                return type.GetProperty(key).GetValue(src, null);
            return null;
        }

    }
}
