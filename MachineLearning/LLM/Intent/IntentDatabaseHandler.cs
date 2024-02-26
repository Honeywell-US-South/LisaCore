using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisaCore.MachineLearning.LLM.Intent
{
    internal class IntentDatabaseHandler
    {
        private readonly string connectionString;

        public IntentDatabaseHandler(string databasePath)
        {
            if (!Directory.Exists(databasePath)) { Directory.CreateDirectory(databasePath); }
            var dbFile = Path.Combine(databasePath, "intents.db");
            connectionString = $"Data Source={dbFile}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Create table if not exists
                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS intents (id INTEGER PRIMARY KEY, text TEXT UNIQUE, label TEXT, timestamp DATETIME)";
                createTableCmd.ExecuteNonQuery();
            }
        }

        public void AddTextIfNotExist(string text, string? label = null)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Check if text exists
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "SELECT label FROM intents WHERE text = $text";
                checkCmd.Parameters.AddWithValue("$text", text);
                var existingLabel = checkCmd.ExecuteScalar() as string;

                if (existingLabel == null)
                {
                    // Insert new record
                    var addCmd = connection.CreateCommand();
                    addCmd.CommandText = "INSERT INTO intents (text, label, timestamp) VALUES ($text, $label, $timestamp)";
                    addCmd.Parameters.AddWithValue("$text", text);
                    addCmd.Parameters.AddWithValue("$label", label?.Trim() ?? string.Empty);
                    addCmd.Parameters.AddWithValue("$timestamp", DateTime.UtcNow);
                    addCmd.ExecuteNonQuery();
                }
                else if (!string.Equals(existingLabel, label?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    // Update label and timestamp if label doesn't match
                    var updateCmd = connection.CreateCommand();
                    updateCmd.CommandText = "UPDATE intents SET label = $label, timestamp = $timestamp WHERE text = $text";
                    updateCmd.Parameters.AddWithValue("$text", text);
                    updateCmd.Parameters.AddWithValue("$label", label?.Trim() ?? string.Empty);
                    updateCmd.Parameters.AddWithValue("$timestamp", DateTime.UtcNow);
                    updateCmd.ExecuteNonQuery();
                }
            }
        }

        public bool UpdateLabel(string text, string newLabel)
        {
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    var updateCmd = connection.CreateCommand();
                    updateCmd.CommandText = "UPDATE intents SET label = $label, timestamp = $timestamp WHERE text = $text";
                    updateCmd.Parameters.AddWithValue("$text", text);
                    updateCmd.Parameters.AddWithValue("$label", newLabel);
                    updateCmd.Parameters.AddWithValue("$timestamp", DateTime.UtcNow);
                    updateCmd.ExecuteNonQuery();
                    return true;
                }
            } catch (Exception ex) { Console.Out.WriteLineAsync(ex.ToString()); }
            return false;
        }

        public List<IntentData> GetLabeledData()
        {
            List<IntentData> dataList = new List<IntentData>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT text, label FROM intents WHERE label IS NOT NULL AND label != ''";

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new IntentData
                        {
                            Text = reader.GetString(0),
                            Label = reader.GetString(1),
                            
                        });
                    }
                }
            }

            return dataList;
        }

        public List<string> GetLabels()
        {
            List<string> labels = new List<string>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT DISTINCT label FROM intents WHERE label IS NOT NULL AND label != ''";

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        labels.Add(reader.GetString(0));
                    }
                }
            }

            return labels;
        }
    }
}
