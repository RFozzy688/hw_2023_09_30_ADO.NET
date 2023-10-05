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

            Console.WriteLine("Список авторов книги №3:");
            pr.ListOfBookAuthors();

            Console.WriteLine("Список доступных книг:");
            pr.ListBooksAvailable();

            Console.WriteLine("Список книг, которые на руках у пользователя:");
            pr.UserListOfBooks();

            Console.WriteLine("Список книг, взятых за последнее время:");
            pr.ListOfBooksTakenRecently();

            Console.WriteLine("Количество книг, взятых определённым посетителем за последний год");
            pr.QuantityBooks();

            Console.WriteLine("Пакетная обработка двух запросов:");
            pr.BatchProcessing_2_Requests();

            Console.WriteLine("Пакетная обработка трех запросов:");
            pr.BatchProcessing_3_Requests();

            Console.WriteLine("Обнулите задолженности всех должников:");
            pr.ClearAllDebts();
        }
        public void ListOfDebtors() // список должников
        {
            SqlDataReader rdr = null;
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                string strQuery = "SELECT DISTINCT Student.first_name, Student.last_name" +
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

                Console.WriteLine();
            }
            finally
            {
                rdr?.Close();
                conn?.Close();
            }
        }
        public void ListBooksAvailable() // список доступных книг
        {
            SqlDataReader rdr = null;

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                string str = "SELECT Book.name" +
                    " FROM Book" +
                    " WHERE Book.quantity > 0";

                cmd.CommandText = str;
                cmd.Connection = conn;
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Console.WriteLine($"{rdr[0]}");
                }

                Console.WriteLine();
            }
            finally
            {
                rdr?.Close();
                conn?.Close();
            }
        }
        public void UserListOfBooks() // список книг, которые на руках у пользователя 
        {
            SqlDataReader rdr = null;

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                string strQuery = "SELECT Book.name" +
                    " FROM Student JOIN S_Cards ON Student.id = S_Cards.id_student" +
                    " JOIN Book ON S_Cards.id_book = Book.id" +
                    " WHERE Student.id = @numStudent AND S_Cards.date_in IS NULL";

                cmd.CommandText = strQuery;
                cmd.Connection = conn;
                cmd.Parameters.Add("@numStudent", System.Data.SqlDbType.Int).Value = 17;

                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Console.WriteLine($"{rdr[0]}");
                }

                Console.WriteLine();
            }
            finally
            {
                rdr?.Close();
                conn?.Close();
            }
        }
        public void ListOfBooksTakenRecently() // список книг, взятых за последнее время
        {
            SqlDataReader rdr = null;

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                string strQuery = "SELECT DISTINCT Book.name" +
                    " FROM Book JOIN S_Cards ON Book.id = S_Cards.id_book" +
                    " WHERE S_Cards.date_out > @dateOut";

                cmd.CommandText = strQuery;
                cmd.Connection = conn;
                cmd.Parameters.Add("@dateOut", System.Data.SqlDbType.Date).Value = "2023-09-25";

                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Console.WriteLine($"{rdr[0]}");
                }

                Console.WriteLine();
            }
            finally
            {
                rdr?.Close();
                conn?.Close();
            }
        }
        public void QuantityBooks() // количество книг, взятых определённым посетителем за последний год
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();

                string strQuery = "SELECT COUNT(Book.name)" +
                    " FROM Book JOIN S_Cards ON Book.id = S_Cards.id_book" +
                    " JOIN Student ON S_Cards.id_student = Student.id" +
                    " WHERE Student.id = @idStudent AND S_Cards.date_out > @dateOut";

                cmd.CommandText = strQuery;
                cmd.Connection = conn;
                cmd.Parameters.Add("@idStudent", System.Data.SqlDbType.Int).Value = 17;
                cmd.Parameters.Add("@dateOut", System.Data.SqlDbType.Date).Value = "2022-09-25";

                Console.WriteLine((int)cmd.ExecuteScalar());
                Console.WriteLine();
            }
            finally
            {
                conn?.Close();
            }
        }
        public void BatchProcessing_2_Requests()
        {
            SqlDataReader rdr = null;

            try
            {
                conn.Open();

                string strQuery = "SELECT Student.first_name, Student.last_name" +
                    " FROM Student JOIN S_Cards ON Student.id = S_Cards.id_student" +
                    " WHERE date_in IS NULL;" +
                    " SELECT Author.first_name, Author.last_name" +
                    " FROM Author JOIN Book ON Author.id = Book.id_author" +
                    " WHERE Book.id = @numBook";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = strQuery;
                cmd.Connection = conn;
                cmd.Parameters.Add("@numBook", System.Data.SqlDbType.Int).Value = 5;

                rdr = cmd.ExecuteReader();

                int count = 0;

                do
                {
                    if (count == 0)
                    {
                        Console.WriteLine("Список должников:");
                    }
                    else
                    {
                        Console.WriteLine("\nСписок авторов книги №5:");
                    }

                    while (rdr.Read())
                    {
                        Console.WriteLine($"{rdr[0]} {rdr[1]}");
                    }

                    count++;

                } while (rdr.NextResult());

                Console.WriteLine();

            }
            finally
            {
                rdr?.Close();
                conn?.Close();
            }
        }
        public void BatchProcessing_3_Requests()
        {
            SqlDataReader rdr = null;

            try
            {
                conn.Open();

                string strQuery = "SELECT Student.first_name, Student.last_name" +
                    " FROM Student JOIN S_Cards ON Student.id = S_Cards.id_student" +
                    " WHERE date_in IS NULL;" +
                    " SELECT Author.first_name, Author.last_name" +
                    " FROM Author JOIN Book ON Author.id = Book.id_author" +
                    " WHERE Book.id = @numBook;" +
                    " SELECT Book.name" +
                    " FROM Book" +
                    " WHERE Book.quantity > 0";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = strQuery;
                cmd.Connection = conn;
                cmd.Parameters.Add("@numBook", System.Data.SqlDbType.Int).Value = 5;

                rdr = cmd.ExecuteReader();

                int count = 0;

                do
                {
                    if (count == 0)
                    {
                        Console.WriteLine("Список должников:");
                    }
                    else if (count == 1)
                    {
                        Console.WriteLine("\nСписок авторов книги №5:");
                    }
                    else
                    {
                        Console.WriteLine("\nСписок доступных книг:");
                    }

                    while (rdr.Read())
                    {
                        if (rdr.FieldCount == 2)
                        {
                            Console.WriteLine($"{rdr[0]} {rdr[1]}");
                        }
                        else
                        {
                            Console.WriteLine($"{rdr[0]}");
                        }
                    }

                    count++;

                } while (rdr.NextResult());

                Console.WriteLine();

            }
            finally
            {
                rdr?.Close();
                conn?.Close();
            }
        }
        public void ClearAllDebts()
        {
            SqlDataReader rdr = null;

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                SqlCommand cmdUpdate = new SqlCommand();
                cmd.Connection = conn;
                cmdUpdate.Connection = conn;

                string strQuery = "SELECT CONVERT(date, GETDATE())";
                cmd.CommandText = strQuery;
                DateTime date = (DateTime)cmd.ExecuteScalar();
                string strDateNow = String.Format("{0}-{1:d2}-{2:d2}", date.Year, date.Month, date.Day);

                strQuery = "SELECT S_Cards.id" +
                    " FROM S_Cards" +
                    " WHERE S_Cards.date_in IS NULL";

                cmd.CommandText = strQuery;

                rdr = cmd.ExecuteReader();

                strQuery = "UPDATE S_Cards" +
                    " SET S_Cards.date_in = @dateIn" +
                    " WHERE S_Cards.id = @id";

                cmdUpdate.CommandText = strQuery;
                cmdUpdate.Parameters.Add("@dateIn", System.Data.SqlDbType.Date).Value = strDateNow;

                List<int> id = new List<int>();
                while (rdr.Read())
                {
                    id.Add((int)rdr[0]);
                }
                rdr?.Close();
                
                for (int i = 0; i < id.Count; i++) 
                {
                    cmdUpdate.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id[i];
                    cmdUpdate.ExecuteNonQuery();
                    cmdUpdate.Parameters.RemoveAt(1);
                }

                Console.WriteLine();
            }
            finally
            {
                rdr?.Close();
                conn?.Close();
            }
        }
    }
}
