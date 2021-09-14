using System;
using System.Data;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// This example code shows how you could implement the required main function for a 
// Console UWP Application. You can replace all the code inside Main with your own custom code.

// You should also change the Alias value in the AppExecutionAlias Extension in the 
// Package.appxmanifest to a value that you define. To edit this file manually, right-click
// it in Solution Explorer and select View Code, or open it with the XML Editor.

namespace sqlitiamo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ControlCenter();
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine($"arg[{i}] = {args[i]}");
                }
            }
        }

        static void ControlCenter()
        {
            GetHelp();
            string input;
            input = Console.ReadLine();

            while (!input.Equals("quit"))
            {
                if (input.Equals("ls"))
                {
                    ListDBs();
                }
                else if (input.Equals("explore"))
                {
                    GetPath();
                }
                else if (input.Equals("help"))
                {
                    GetHelp();
                }
                else if (input.Equals(""))
                {
                    // do nothing
                }
                else
                {
                    ControlArgument(input);
                }

                input = Console.ReadLine();
            }
        }

        static void ControlArgument(string input)
        {
            string[] cmd = input.Split(' ');

            if (cmd[0].Equals("create"))
            {
                CreateDB(cmd[1]);
            }
            else if (cmd[0].Equals("rm"))
            {
                RemoveDB(cmd[1]);
            }
            else if (cmd[0].Equals("use"))
            {
                ChooseDB(cmd[1]);
            }
            else
            {
                GetHelp();
            }
        }

        static void ControlSQL(string name)
        {
            var database = new DataStorage(name);

            Console.Write($"{database.Database}@ ");
            string command;
            command = Console.ReadLine();

            while (!command.Equals("close"))
            {
                if (command.StartsWith("create", StringComparison.CurrentCultureIgnoreCase)
                    || command.StartsWith("drop", StringComparison.CurrentCultureIgnoreCase)
                    || command.StartsWith("insert", StringComparison.CurrentCultureIgnoreCase)
                    || command.StartsWith("delete", StringComparison.CurrentCultureIgnoreCase)
                    || command.StartsWith("update", StringComparison.CurrentCultureIgnoreCase)
                    || command.StartsWith("alter", StringComparison.CurrentCultureIgnoreCase))
                {
                    int rows = database.ExecuteWrite(command);
                    Console.WriteLine($"\t{rows} rows are affected");
                }
                else if (command.StartsWith("select", StringComparison.CurrentCultureIgnoreCase))
                {
                    var reader = database.ExecuteRead(command);
                    if (reader != null)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"\t{reader.GetName(i)}");
                        }
                        Console.Write("\n");

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"\t------");
                        }
                        Console.Write("\n");

                        int index = 1;
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write($"\t{reader[i].ToString()}");
                            }
                            Console.Write("\n");
                            index++;
                        }
                        reader.Close();
                    }
                }
                else
                {
                    Console.WriteLine("\tundefined command detected");
                }

                Console.Write($"{database.Database}@ ");
                command = Console.ReadLine();
            }

            database.Close();
        }

        //-----------------------------------------------------------------------

        static async void ListDBs()
        {
            var dbs = await ApplicationData.Current.LocalCacheFolder.GetFilesAsync();
            if (dbs.Count != 0)
            {
                foreach (var db in dbs)
                {
                    if (db.FileType.Equals(".db"))
                        Console.WriteLine($"\t{db.DisplayName}");
                }
            }
            else
            {
                Console.WriteLine("\tno database here, maybe create one");
            }
        }

        static async void CreateDB(string name)
        {
            try
            {
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(name + ".db");
                Console.WriteLine($"\tthere will be database {name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\tException: {ex.Message}");
            }
        }

        static async void RemoveDB(string name)
        {
            try
            {
                Console.Write($"Do you want to remove the database {name}?(y/n)");
                var confirm = Console.ReadKey();
                if (confirm.Key != ConsoleKey.N)
                {
                    await (await ApplicationData.Current.LocalCacheFolder.GetFileAsync(name + ".db")).DeleteAsync();
                    Console.WriteLine($"\n\tdatabase {name}'s gone");
                }
                else
                {
                    Console.Write("\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n\tException: {ex.Message}");
            }
        }

        static void ChooseDB(string name)
        {
            ControlSQL(name);
        }

        //-----------------------------------------------------------------------

        static void GetPath()
        {
            string path = ApplicationData.Current.LocalCacheFolder.Path;
            Console.WriteLine($"\t{path}");
        }

        static void GetHelp()
        {
            Console.WriteLine("sqlitiamo = sqlite ti amo = we sqlite");
            Console.WriteLine("Commands in app");
            Console.WriteLine("\tls\tlist user-friendly names of your databases");
            Console.WriteLine("\texplore\texplore the folder of databases");
            Console.WriteLine("\tcreate\tcreate a new database with specific name");
            Console.WriteLine("\trm\tremove the database with specific name");
            Console.WriteLine("\tuse\tuse and open the database with specific name");
            Console.WriteLine("\tquit\texit this app");
            Console.WriteLine("\thelp\tshow this text");
            Console.WriteLine("Commands in database");
            Console.WriteLine("\tclose\tclose the current database");
            Console.WriteLine("\tyou can execute sql commands directly in database\n");
        }
    }
}
