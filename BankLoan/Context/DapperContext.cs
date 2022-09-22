using System.Data;
using System.Data.SqlClient;

namespace BankLoan.Context
{
    public class DapperContext
    {
        private readonly IConfiguration configuration;
        private readonly string ConnectionString;
        public DapperContext(IConfiguration configuration)
        {
            this.configuration = configuration;
            ConnectionString = configuration.GetConnectionString("sqlConnection");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(ConnectionString);
    }
}
