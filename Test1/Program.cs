using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Test1.Model;

internal class Program
{
    private static void Main(string[] args)
    {
        DisplayMenuContents();
        MenuHandler();

        
        
        void MenuHandler()
        {
            var exit = false;

            do
            {
                SQLiteConnection sqlite_conn = CreateConnection();
                CreateTable(sqlite_conn);
                var menuOption = Console.ReadKey(true);

                switch (menuOption.KeyChar)
                {
                    case '1':
                        InsertData(sqlite_conn);
                        Console.WriteLine("");
                        DisplayMenuContents();
                        break;
                    case '2':
                        ReadData(sqlite_conn);
                        Console.WriteLine("");
                        DisplayMenuContents();

                        break;
                    case '3':
                        Console.WriteLine("Enter a name.");
                        var name = Console.ReadLine();
                        searchData(sqlite_conn, name );
                        Console.WriteLine("");
                        DisplayMenuContents();
                        break;
                    case '4':
                        Console.WriteLine("Enter a name.");
                        var productName = Console.ReadLine();
                        deleteData(sqlite_conn, productName);
                        Console.WriteLine("");
                        DisplayMenuContents();
                        break;
                    case '5':
                        CloseConnection(sqlite_conn);
                        exit = true;
                        break;
                    default:
                        CloseConnection(sqlite_conn);
                        exit = true;
                        break;
                }

            } while (!exit);
        }


        void DisplayMenuContents()
        {
            Console.WriteLine("");

            Console.WriteLine("Please select an option:");
            Console.WriteLine("[1] Insert a product.");
            Console.WriteLine("[2] List all products.");
            Console.WriteLine("[3] Search a product.");
            Console.WriteLine("[4] Delete a product.");
            Console.WriteLine("[5] Exit App.");

            Console.WriteLine("");
        }


        SQLiteConnection CreateConnection()
        {
            // Create a new database connection:
            SQLiteConnection sqlite_conn = new SQLiteConnection("Data Source=test1.db; Version = 3; New = True; Compress = True; ");
           
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return sqlite_conn;
        }

        void CloseConnection(SQLiteConnection conn)
        {
            //Close the connection
            conn.Close();
        }

        void CreateTable(SQLiteConnection conn)
        {
            // Create table if not exists.
            //Represents a SQL statement to be executed against a SQLite database.
            //Create and returrns a SqlCommand.
            SQLiteCommand sqlite_cmd = conn.CreateCommand();

            //Returns or sets the command string for the specified data source.
            sqlite_cmd.CommandText = "CREATE TABLE if not exists Item (Id INTEGER NOT NULL UNIQUE, Name TEXT NOT NULL, ExpirationDate TEXT NOT NULL, Type TEXT NOT NULL, NetPrice REAL NOT NULL, Weight REAL NOT NULL, Quantity INTEGER NOT NULL, IsDeleted INTEGER NOT NULL, PRIMARY KEY(Id AUTOINCREMENT))";
            sqlite_cmd.ExecuteNonQuery();

        }

        Item CreateObj()
        {
            // Create the object to insert in the table.
            Item obj = new Item();
            Console.WriteLine("Enter a name of the product.");
            obj.Name = Console.ReadLine();

            Console.WriteLine("Enter a valid expiration date (dd/MM/yyyy) of the product.");

            var nonValidDate = true;
            do
            {
                try
                {
                    obj.ExpirationDate = DateTime.Parse(Console.ReadLine());
                    nonValidDate = false;
                }
                catch (Exception)
                {

                    Console.WriteLine("Enter a valid expiration date (dd/MM/yyyy) of the product.");
                }
            } while (nonValidDate);
         
            Console.WriteLine("Enter the type of item.");
            obj.Type = Console.ReadLine();

            Console.WriteLine("Enter a valid net price (0,00) of the product.");
            var nonValidPrice = true;
            do
            {
                try
                {
                    obj.NetPrice = double.Parse(Console.ReadLine());
                    nonValidPrice = false;
                }
                catch (Exception)
                {

                    Console.WriteLine("Enter a valid net price (0,00) of the product.");
                }
            } while (nonValidPrice);
            

            Console.WriteLine("Enter a valid weight (0,00) of the product (Kilograms or litres).");
            var nonValidWeight = true;
            do
            {
                try
                {
                    obj.Weight = double.Parse(Console.ReadLine());
                    nonValidWeight = false;
                }
                catch (Exception)
                {

                    Console.WriteLine("Enter a valid weight (0,00) of the product (Kilograms or litres).");
                }
            } while (nonValidWeight);
            

            Console.WriteLine("Enter the quantity of the product.");
            var nonValidQuantity = true;
            do
            {
                try
                {
                    obj.Quantity = int.Parse(Console.ReadLine());
                    nonValidQuantity = false;
                }
                catch (Exception)
                {

                    Console.WriteLine("Enter the quantity of the product.");
                }
            } while (nonValidQuantity);
            return obj;
        }

        void InsertData(SQLiteConnection conn)
        {
            var obj = CreateObj();
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO Item(Name, ExpirationDate, Type, NetPrice, Weight, Quantity, IsDeleted) VALUES(@name, @expirationDate, @type, @netPrice, @weight, @quantity, false)";
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@name", obj.Name));
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@expirationDate", obj.ExpirationDate));
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@type", obj.Type));
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@netPrice", obj.NetPrice));
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@weight", obj.Weight));
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@quantity", obj.Quantity));
            sqlite_cmd.CommandType = CommandType.Text;
            sqlite_cmd.ExecuteNonQuery();

        }

        void ReadData(SQLiteConnection conn)
        {
            List<Item> items = new List<Item>();
            //Methods for reading the result of a command executed.
           
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Item";
            //Sends the CommandText to the Connection and builds a SqlDataReader 
            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();

            //Read the table.
            while (sqlite_datareader.Read())
            {
                var isDeleted = int.Parse(sqlite_datareader["IsDeleted"].ToString());
                if (isDeleted == 0)
                {
                    items.Add(new Item()
                    {
                        Id = int.Parse(sqlite_datareader["Id"].ToString()),
                        Name = sqlite_datareader["Name"].ToString(),
                        ExpirationDate = DateTime.Parse(sqlite_datareader["ExpirationDate"].ToString()),
                        Type = sqlite_datareader["Type"].ToString(),
                        NetPrice = double.Parse(sqlite_datareader["NetPrice"].ToString()),
                        Weight = double.Parse(sqlite_datareader["Weight"].ToString()),
                        Quantity = int.Parse(sqlite_datareader["Quantity"].ToString()),
                        IsDeleted = int.Parse(sqlite_datareader["IsDeleted"].ToString())
                    });
                };
            }
            sqlite_datareader.Close();

            //Check if the table contains data.
            if (items.Count > 0)
            {
                Console.WriteLine("Inventory:");
                items.ForEach(item => Console.WriteLine("{0}: Expiration Date: {1}, Type: {2}, Net Price: {3}, Weight: {4}, Quantity: {5} and isDeleted: {6}", item.Name, item.ExpirationDate, item.Type, item.NetPrice, item.Weight, item.Quantity, item.IsDeleted));
            }
            else
            {
                Console.WriteLine("No products.");
            }
            
        }

        void searchData(SQLiteConnection conn, string name)
        {
            Item item = null;
           
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Item Where name ='" + name + "'";
            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                var isDeleted = int.Parse(sqlite_datareader["IsDeleted"].ToString());
                if (isDeleted == 0)
                {
                    item = new Item()
                    {
                        Id = int.Parse(sqlite_datareader["Id"].ToString()),
                        Name = sqlite_datareader["Name"].ToString(),
                        ExpirationDate = DateTime.Parse(sqlite_datareader["ExpirationDate"].ToString()),
                        Type = sqlite_datareader["Type"].ToString(),
                        NetPrice = double.Parse(sqlite_datareader["NetPrice"].ToString()),
                        Weight = double.Parse(sqlite_datareader["Weight"].ToString()),
                        Quantity = int.Parse(sqlite_datareader["Quantity"].ToString()),
                        IsDeleted = int.Parse(sqlite_datareader["IsDeleted"].ToString())
                    };
                }
                    
            }
            sqlite_datareader.Close();

            //Check if the class contains a item or null.
            if (Object.ReferenceEquals(null,item))
            {
                Console.WriteLine("There is no product with that name.");
            }
            else
            {
                Console.WriteLine("{0}: Expiration Date: {1}, Type: {2}, Net Price: {3}, Weight: {4}, Quantity: {5} and isDeleted: {6}", item.Name, item.ExpirationDate, item.Type, item.NetPrice, item.Weight, item.Quantity, item.IsDeleted);
            }

        }

        void deleteData(SQLiteConnection conn, string productName)
        {
            searchData(conn, productName);
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "UPDATE Item set IsDeleted = 1 Where name ='" + productName + "'" ;
            sqlite_cmd.ExecuteNonQuery();

        }
    }
}