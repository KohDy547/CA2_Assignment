using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CA2_Assignment.Models.CscModels
{
    public class PremiumMemberContext
    {
        public string ConnectionString { get; set; }

        public PremiumMemberContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<PremiumMembers> GetAllAlbums()
        {
            List<PremiumMembers> list = new List<PremiumMembers>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from PremiumMembers", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PremiumMembers()
                        {
                            Id = reader["Id"].ToString()
                        });
                    }
                }
            }
            return list;
        }

    }
}
