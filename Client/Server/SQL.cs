using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Bindings;

namespace Server
{
    class SQL
    {
        private SqlConnection conn = new SqlConnection("Data Source=MSI;Initial Catalog=PSOL;User ID=root;Password=N^mLAd4h&E8x6#nT");

        public void ConnectToSQL()
        {
            conn.Open();
            Console.WriteLine("Connected to SQL server");
        }

        private void disconnect()
        {
            Console.WriteLine("Disconnected from SQL server");
            conn.Close();
        }

        private void read()
        {
            SqlCommand cmd = new SqlCommand("SELECT [name] FROM [PSOL].[dbo].[users]", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
            }

            reader.Close();
        }

        public void register(int index, string username, string password)
        {
            Console.WriteLine("test");
            string MD5password = CalculateMD5Hash(password);
            SqlCommand cmd = new SqlCommand("INSERT INTO [PSOL].[dbo].[users] VALUES ('" + username + "','" + MD5password + "','10','10','0')", conn);
            cmd.ExecuteNonQuery();
            ClearPlayer(index);
            Types.Player[index].Login = username;
            Types.Player[index].Name = username;
            Types.Player[index].X = 10;
            Types.Player[index].Y = 10;
            Types.Player[index].Rotation = 0;
        }

        public bool AccountExists(string username)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM [PSOL].[dbo].[users] WHERE [name] = '" + username + "'", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }

        public bool PasswordOK(string username, string password)
        {
            var MD5password = CalculateMD5Hash(password);
            SqlCommand cmd = new SqlCommand("SELECT * FROM [PSOL].[dbo].[users] WHERE [name] = '" + username + "' AND [password] = '" + MD5password + "'", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }

        public void LoadPlayer(int index, string username)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM [PSOL].[dbo].[users] WHERE [name] = '" + username + "'", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Types.Player[index].Name = reader.GetString(1);
                Types.Player[index].X = reader.GetFloat(3);
                Types.Player[index].Y = reader.GetFloat(4);
                Types.Player[index].Rotation = reader.GetFloat(5);
            }
            reader.Close();
        }

        public void SavePlayer(int index)
        {
            SqlCommand cmd = new SqlCommand("UPDATE [PSOL].[dbo].[users] SET (" +
                                            "[X] = '" + Types.Player[index].X + "'" +
                                            "[Y] = '" + Types.Player[index].Y + "'" +
                                            "[rotation] = '" + Types.Player[index].Rotation + "'" +
                                            ") WHERE [name] = '" + Types.Player[index].Name + "'", conn);
            cmd.ExecuteNonQuery();
        }

        public void SaveGame(int index = -1)
        {
            if (index == -1)
            {
                Console.WriteLine("Saving database...");
                for (var i = 1; i < Constants.MAX_PLAYERS; i++)
                {
                    if (Types.Player[i].Login != null)
                    {
                        SavePlayer(i);
                    }
                }
            }
            else
            {
                SavePlayer(index);
            }
        }

        public void ClearPlayer(int index)
        {
            Types.Player[index].Login = "";
            Types.Player[index].Password = "";
            Types.Player[index].Name = "";
        }

        private string CalculateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}
