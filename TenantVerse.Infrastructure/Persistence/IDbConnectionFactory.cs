using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
namespace TenantVerse.Infrastructure.Persistence
{
    public interface IDbConnectionFactory
    {
            IDbConnection CreateConnection();
    }
}