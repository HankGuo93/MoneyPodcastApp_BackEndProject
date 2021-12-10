using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;

namespace podcast_project
{
    public class DBInfoCheck
    {
        private IConfiguration configuration;

        public DBInfoCheck(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        private MySqlConnection GetConnection()
        {
            string _Conster = configuration.GetValue<string>("AppSettings:myConnString");
            return new MySqlConnection(_Conster);
        }
        public Boolean findIdExsit<T>(string index, string database, T id)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT {index} FROM {database} WHERE {index} =  '{id}';", conn);
                cmd.CommandType = System.Data.CommandType.Text;
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        return true;
                    }
                }
                conn.Close();
                return false;

            }
        }

        public Boolean findIdExsitTwoParam<T>(string indexOne, string indexTwo, string database, T idOne, T idTwo)
        {

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    $"SELECT {indexOne}, {indexTwo} FROM {database} WHERE {indexOne} =  {idOne} and {indexTwo} =  {idTwo};", conn
                    );
                cmd.CommandType = System.Data.CommandType.Text;
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        return true;
                    }
                }
                conn.Close();
                return false;

            }
        }
    }
}
