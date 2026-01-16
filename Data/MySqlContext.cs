using MySql.Data.MySqlClient;

namespace LicenciaSistemas.Data
{
    public class MySqlContext
    {
        private readonly IConfiguration _config;

        public MySqlContext(IConfiguration config)
        {
            _config = config;
        }

        public MySqlConnection GetConnection()
        {
            var cs = _config.GetConnectionString("MySqlConnection");
            return new MySqlConnection(cs);
        }
    }
}
