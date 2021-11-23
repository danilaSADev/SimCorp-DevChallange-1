using SCDevChallengeApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace SCDevChallengeApi.BLL
{
    public class FinancialStorage : IFinancialStorage
    {
        private readonly string CSV_PATH = Path.Combine(Environment.CurrentDirectory, @"Input\", "data.csv");
        private bool _isLoaded = false;
        public List<FinancialAsset> AssetsList { get; } = new List<FinancialAsset>();
        public FinancialStorage() 
        {
            LoadFinancialInformation();
            LoadDataBase();
        }

        private void LoadDataBase()
        {
            if (_isLoaded)
            {
                return;
            }

            using (var connection = new SqliteConnection($"DataSource={Environment.CurrentDirectory}/finances.db;Mode=ReadWriteCreate;"))
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                // creating table
                command.CommandText = "CREATE TABLE FinancialStorage(_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, portfolio TEXT NOT NULL, owner TEXT NOT NULL, instrument TEXT NOT NULL, price REAL NOT NULL)";
                command.ExecuteNonQuery();

                // TODO add values from CSV files if database is new
            }
            _isLoaded = true;
        }

        public void LoadFinancialInformation() 
        {
            if (_isLoaded)
            {
                return;
            }

            using (var reader = new StreamReader(CSV_PATH))
            {
                string line = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    FromCSVAssetParser parser = new FromCSVAssetParser(line);
                    AssetsList.Add(parser.Parse());
                }
            }

            _isLoaded = true;
        }
    }
}
