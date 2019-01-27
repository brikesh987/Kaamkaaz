using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace KaamkaazServices.Helper
{

    public class JsonObjectTypeHandler : SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.Value = (value == null)
                ? (object)DBNull.Value
                : JsonConvert.SerializeObject(value);
            parameter.DbType = DbType.String;
        }

        public object Parse(Type destinationType, object value)
        {
            return JsonConvert.DeserializeObject(value.ToString(), destinationType);
        }
    }
}
