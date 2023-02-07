using Ruanmou.Framework.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ruanmou.Framework.DBExtend
{
    public class TestExpression
    {

        public void Test()
        {
            Expression<Func<SqlDataReader, BaseModel>> expression = r => new BaseModel()
            {
                Id = (int)r["Id"]
            };


        }

        public static Func<SqlDataReader, BaseModel> Get()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(SqlDataReader), "r");
            MethodInfo methodInfoGet = typeof(SqlDataReader).GetMethod("get_Item", new Type[] { typeof(string) });
            var getData = Expression.Call(parameterExpression, methodInfoGet, new Expression[]
                {
                    Expression.Constant("Id", typeof(string))
                });
            var convert = Expression.Convert(getData, typeof(int));
            PropertyInfo idPropertyInfo = typeof(BaseModel).GetProperty("Id");
            Expression<Func<SqlDataReader, BaseModel>> expressionNew = Expression.Lambda<Func<SqlDataReader, BaseModel>>(Expression.MemberInit(Expression.New(typeof(BaseModel)), new MemberBinding[]
            {
                Expression.Bind(idPropertyInfo, convert)
            }), new ParameterExpression[]
            {
                parameterExpression
            });

            return expressionNew.Compile();
        }
    }
}
