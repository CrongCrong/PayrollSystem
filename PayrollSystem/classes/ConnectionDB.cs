using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollSystem.classes
{
    public class ConnectionDB
    {
        string myConnection = "Server=" + Properties.Settings.Default.connectionDB + "; port=3306;username=root;password=admin; convert zero datetime=True";
        public MySqlConnection myConn = new MySqlConnection();
        MySqlDataReader reader;

        public MySqlDataReader getSelectConnection(string queryString, List<string> parameters)
        {

            try
            {
                myConn = new MySqlConnection(myConnection);
                MySqlCommand command = new MySqlCommand(queryString, myConn);

                command.CommandType = CommandType.Text;
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        var p1 = command.CreateParameter();
                        p1.Value = parameters[i].ToString();
                        command.Parameters.Add(p1);
                    }
                }


                myConn.Open();
                command.Connection = myConn;

                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return reader;
        }

        public DataTable fillDataGridView(string queryString, List<string> parameters)
        {
            DataTable dataTable = new DataTable();
            myConn = new MySqlConnection(myConnection);

            MySqlCommand command = new MySqlCommand(queryString, myConn);

            command.CommandType = CommandType.Text;

            if (parameters != null)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    var p1 = command.CreateParameter();
                    p1.Value = parameters[i].ToString();
                    command.Parameters.Add(p1);
                }
            }

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            adapter.Fill(dataTable);

            return dataTable;
        }

        public bool AddRecordToDatabase(string queryString, List<string> parameters)
        {
            bool ifCorrect = false;

            try
            {
                myConn = new MySqlConnection(myConnection);
                MySqlCommand command = new MySqlCommand(queryString, myConn);

                command.CommandType = CommandType.Text;

                myConn.Open();
                command.Connection = myConn;

                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        var p1 = command.CreateParameter();
                        p1.Value = parameters[i].ToString();
                        command.Parameters.Add(p1);
                    }
                }

                command.ExecuteNonQuery();
                ifCorrect = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ifCorrect;
        }

        public int getMaxID(string queryString)
        {
            int x = 0;

            myConn = new MySqlConnection(myConnection);
            MySqlCommand command = new MySqlCommand(queryString, myConn);

            myConn.Open();

            IDataReader reader = command.ExecuteReader();
            if (reader != null && reader.Read())
            {
                x = reader.FieldCount;
                x = reader.RecordsAffected;
                x = reader.GetInt32(0);
            }
            reader.Close();
            return x;
        }

        public bool closeConnection()
        {
            bool ifConnected = false;

            if (this.myConn.State == ConnectionState.Open)
            {
                this.myConn.Close();
                ifConnected = true;
            }

            return ifConnected;
        }

        public void writeLogFile(string activity)
        {
            string filePath = @"C:\Logs\Error.txt";

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + activity + Environment.NewLine + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }
    }
}
