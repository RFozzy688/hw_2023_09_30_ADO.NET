using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace hw_2023_09_30
{
    internal class Program
    {
        SqlConnection conn = null;
        public Program() 
        {
            conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnStrLibrary"].ConnectionString;
        }
        static void Main(string[] args)
        {
            Program pr = new Program();

            Console.WriteLine("Список должников:");
            pr.ListOfDebtors();

            Console.WriteLine("список авторов книги №3");
            pr.ListOfBookAuthors();
        }
        public void ListOfDebtors() // список должников
        {
            SqlDataReader rdr = null;
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                string strQuery = "SELECT Student.first_name, Student.last_name" +
                    " FROM Student JOIN S_Cards ON Student.id = S_Cards.id_student" +
                    " WHERE date_in IS NULL";

                cmd.CommandText = strQuery;
                cmd.Connection = conn;
                rdr = cmd.ExecuteReader();

                int count = 1;

                while (rdr.Read())
                {
                    Console.WriteLine($"{count++}. {rdr[0]} {rdr[1]}");
                }

                Console.WriteLine();
            }
            finally
            {
                if (rdr != null) { rdr.Close(); }
                if (conn != null) { conn.Close(); }
            }
        }
        public void ListOfBookAuthors() // список авторов книги №3 (по порядку из таблицы ‘Book’)
        {
            SqlDataReader rdr = null;

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                string str = "SELECT Author.first_name, Author.last_name" +
                    " FROM Author JOIN Book ON Author.id = Book.id_author" +
                    " WHERE Book.id = @numBook";

                cmd.CommandText = str;
                cmd.Connection = conn;
                cmd.Parameters.Add("@numBook", System.Data.SqlDbType.Int).Value = 3;

                rdr = cmd.ExecuteReader();

                while(rdr.Read())
                {
                    Console.WriteLine($"{rdr[0]} {rdr[1]}");
                }
            }
            finally
            {
                conn?.Close();
            }
        }
    }
}
