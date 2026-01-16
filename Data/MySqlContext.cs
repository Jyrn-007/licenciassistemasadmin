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
            return new MySqlConnection(
                _config.GetConnectionString("MySqlConnection"));
        }
    }
}
