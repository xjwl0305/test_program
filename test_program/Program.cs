using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Text;

namespace test_program
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        
        public static void CallDB()
        {
            string paths = AppDomain.CurrentDomain.BaseDirectory;
            string DBpath = Application.StartupPath + @"\identifier.sqlite";
            using (SqliteConnection connection = new SqliteConnection(DBpath))
            {
                connection.Open();

                CreateTableIfNotExists(connection);

                // Create
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = connection;
                insertCommand.CommandText = "INSERT INTO mytable(id, NAME, age, DESCRIPTION) VALUES (@id, @NAME, @age, @DESCRIPTION)";

                insertCommand.Parameters.Add("@id", SqliteType.Integer);
                insertCommand.Parameters.Add("@NAME", SqliteType.Text, 50);
                insertCommand.Parameters.Add("@age", SqliteType.Integer);
                insertCommand.Parameters.Add("@DESCRIPTION", SqliteType.Text, 150);

                string nameValue = "Name" + Guid.NewGuid().ToString();
                insertCommand.Parameters[0].Value = (int)DateTime.Now.Ticks;
                insertCommand.Parameters[1].Value = nameValue;
                insertCommand.Parameters[2].Value = 10;
                insertCommand.Parameters[3].Value = nameValue + "_Description";

                int affected = insertCommand.ExecuteNonQuery();
                Console.WriteLine("# of affected row: " + affected);

                // Update
                SqliteCommand updateCommand = new SqliteCommand();
                updateCommand.Connection = connection;
                updateCommand.CommandText = "UPDATE mytable SET DESCRIPTION=@DESCRIPTION WHERE NAME=@NAME";

                updateCommand.Parameters.Add("@NAME", SqliteType.Text, 50);
                updateCommand.Parameters.Add("@DESCRIPTION", SqliteType.Text, 150);

                updateCommand.Parameters[0].Value = nameValue;
                updateCommand.Parameters[1].Value = nameValue + "_Description2";

                affected = updateCommand.ExecuteNonQuery();
                Console.WriteLine("# of affected row: " + affected);

                // Select - ExecuteScalar
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = connection;
                selectCommand.CommandText = "SELECT count(*) FROM mytable";

                object result = selectCommand.ExecuteScalar();
                Console.WriteLine("# of records: " + result);

                // Select - DataTable
                DataSet ds = new DataSet();

                /* DataAdapter 미구현

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM mytable", sqlConnection);
                da.Fill(ds, "mytable");

                DataTable dt = ds.Tables["mytable"];
                foreach (DataRow dr in dt.Rows)
                {
                    Console.WriteLine(string.Format("Name = {0}, Desc = {1}", dr["NAME"], dr["DESCRIPTION"]));
                }
                */

                // Delete
                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = connection;
                deleteCommand.CommandText = "DELETE FROM mytable WHERE NAME=@NAME";

                deleteCommand.Parameters.Add("@NAME", SqliteType.Text, 50);
                deleteCommand.Parameters[0].Value = nameValue;

                affected = deleteCommand.ExecuteNonQuery();
                Console.WriteLine("# of affected row: " + affected);
            }
        }
        private static void CreateTableIfNotExists(SqliteConnection conn)
        {
            string sql = "create table if not exists mytable(id int, NAME varchar(50), age int, DESCRIPTION varchar(150))";

            using (SqliteCommand command = new SqliteCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }

            sql = "create index if not exists idx_NAME on mytable(NAME)";
            using (SqliteCommand command = new SqliteCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }
        }
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}