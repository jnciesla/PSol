using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Bindings;
using PSol.Data.Models;
using PSol.Data.Repositories;

namespace PSol.Server
{
    class SQL
    {
        public SqlConnection conn = new SqlConnection("Data Source=./;Initial Catalog=PSOL;User ID=root;Password=N^mLAd4h&E8x6#nT");

        public bool AccountExists(string username)
        {
            conn.Open();
            bool exists = false;
            SqlCommand cmd = new SqlCommand("SELECT * FROM [PSOL].[dbo].[users] WHERE [name] = '" + username + "'", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                exists = true;
            }
            conn.Close();
            return exists;
        }

        public bool PasswordOK(string username, string password)
        {
            conn.Open();
            bool OK = false;
            var MD5password = CalculateMD5Hash(password);
            SqlCommand cmd = new SqlCommand("SELECT * FROM [PSOL].[dbo].[users] WHERE [name] = '" + username + "' AND [password] = '" + MD5password + "'", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                OK = true;
            }
            conn.Close();
            return OK;
        }
        
        private string CalculateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
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
