using NLog;
using System.Linq;
using NorthWindFinal.Model;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

try
{
    var db = new NWContext();
    string choice;
    do
    {
        Console.WriteLine("1) Display all Products");
        Console.WriteLine("2) Display a specific Product ");
        Console.WriteLine("3) Add a new Product");
        Console.WriteLine("4) Edit a Product");
        Console.WriteLine("5) Display all Categories");
        Console.WriteLine("6) Display all Categories with Products");
        Console.WriteLine("7) Add a new Category");
        Console.WriteLine("8) Edit a Category");
        Console.WriteLine("10) Display specific Category with Products");
        Console.WriteLine("\"q\" to quit");

        choice = Console.ReadLine();
        Console.Clear();
        logger.Info($"Option {choice} selected");
        if (choice == "1")
        {
            var query = db.Products.OrderBy(p => p.ProductName);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.WriteLine();

    } while (choice.ToLower() != "q");
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}
logger.Info("Program ended");
