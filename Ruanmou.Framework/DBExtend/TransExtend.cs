using Ruanmou.Framework.AttributeExtend;
using Ruanmou.Framework.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ruanmou.Framework.DBExtend
{
    /// <summary>
    /// 1941-帅东-男-广州  179722134
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public class TransExtend<TOut> where TOut : BaseModel
    {
        private static Func<SqlDataReader, TOut> func = null;
        private static TOut ToExpression(SqlDataReader dataReader)
        {
            if (func == null)
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(SqlDataReader), "x");
                List<MemberBinding> memberBindings = new List<MemberBinding>();
                foreach (var propertyInfo in typeof(TOut).GetProperties())
                {
                    object value = dataReader[propertyInfo.GetColumnName()];
                    Expression call = ChangeTypeToExpression(value, propertyInfo.PropertyType);
                    MemberAssignment bind = Expression.Bind(propertyInfo, call);
                    memberBindings.Add(bind);
                }
                MemberInitExpression memberInit = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindings.ToArray());
                Expression<Func<SqlDataReader, TOut>> expression = Expression.Lambda<Func<SqlDataReader, TOut>>
                (
                    memberInit,
                    new ParameterExpression[] { parameterExpression }
                );
                func = expression.Compile();
            }
            return func.Invoke(dataReader);
        }

        //private static Expression ReaderToValue(SqlDataReader dataReader,PropertyInfo propertyInfo)
        //{
        //    object value = dataReader[propertyInfo.GetColumnName()];
        //    Expression.Call()
        //}


        /// <summary>
        /// 类型转换成表达式
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        private static Expression ChangeTypeToExpression(object value, Type type)
        {
            if (value == null || value is DBNull)
            {
                return Expression.Constant(null, type);//可空--数据库为null
            }
            if (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                NullableConverter nullableConverter = new NullableConverter(type);
                type = nullableConverter.UnderlyingType;
            }
            //枚举类型
            if (type.IsEnum)
            {
                value = Enum.Parse(type, value.ToString());
            }
            if (type == typeof(Guid))
            {
                Guid.TryParse(value.ToString(), out Guid newGuid);
                value = newGuid;
            }
            if (value?.GetType() == typeof(Guid))
            {
                value = value.ToString();
            }
            return Expression.Constant(value, type);
        }


        public static TOut SqlDataReaderToModel(SqlDataReader dataReader)
        {
            if (dataReader.Read())
            {
                return ToExpression(dataReader);
            }
            else
            {
                return default(TOut);
            }

        }
        public static List<TOut> SqlDataReaderToList(SqlDataReader dataReader)
        {

            List<TOut> collection = new List<TOut>();
            while (dataReader.Read())
            {
                collection.Add(ToExpression(dataReader));
            }
            return collection;
        }

    }
}
