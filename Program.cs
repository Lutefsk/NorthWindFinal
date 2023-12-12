using NLog;
using System.Linq;
using NorthWindFinal.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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
        Console.WriteLine("1) Display active Products"); 
        Console.WriteLine("2) Display inactive Products"); 
        Console.WriteLine("3) Display a specific Product "); 
        Console.WriteLine("4) Add a new Product");
        Console.WriteLine("5) Edit a Product");
        Console.WriteLine("6) Display all Categories"); 
        Console.WriteLine("7) Display all Categories with Products");
        Console.WriteLine("8) Add a new Category");
        Console.WriteLine("9) Edit a Category");
        Console.WriteLine("10) Display specific Category with Products");
        Console.WriteLine("\"q\" to quit");

        choice = Console.ReadLine();
        Console.Clear();
        logger.Info($"Option {choice} selected");

        if (choice == "1") //display active prodcuts in products table
        {
            bool discontinued = false;
            var query = db.Products.Where(p => p.Discontinued == discontinued).OrderBy(p => p.ProductName);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} {discontinued} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (choice == "2") //display inactive prodcuts in products table
        {
            bool discontinued = true;
            var query = db.Products.Where(p => p.Discontinued == discontinued).OrderBy(p => p.ProductName);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} {discontinued} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (choice == "3") //display specific prodcuts in products table
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Enter the product ID:");
            Console.ForegroundColor = ConsoleColor.Blue;
            int id = int.Parse(Console.ReadLine());
            logger.Info($"ProductId {id} selected");
            Product product = db.Products.SingleOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                Console.WriteLine($"\t{product.ProductName}");
                Console.WriteLine($"\tCategoryID: {product.CategoryId}");
                Console.WriteLine($"\tSupplierID: {product.SupplierId}");
                Console.WriteLine($"\tQuantity Per Unit: {product.QuantityPerUnit}");
                Console.WriteLine($"\tUnit Price: {product.UnitPrice}");
                Console.WriteLine($"\tUnits In Stock: {product.UnitsInStock}");
                Console.WriteLine($"\tUnits On Order: {product.UnitsOnOrder}");
                Console.WriteLine($"\tReorder Level: {product.ReorderLevel}");
                Console.WriteLine($"\tDiscontinued: {product.Discontinued}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Product {id} not found.");
                logger.Warn($"Product not found with ID: {id}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (choice == "4") //add a new product
        {
            Product product = new Product();
            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();
            Console.WriteLine("What is the name of the product:");
            product.ProductName = Console.ReadLine();
            Console.WriteLine("What Supplier ID should the Product be assigned to:");
            product.SupplierId = int.Parse(Console.ReadLine());
            Console.WriteLine("What category ID should the Product be included in:");
            product.CategoryId = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter the quantity per unit of product:");
            product.QuantityPerUnit = Console.ReadLine();
            Console.WriteLine("Enter the product price in US $:");
            product.UnitPrice = short.Parse(Console.ReadLine());
            Console.WriteLine("How many units of the product in stock:");
            product.UnitsInStock = short.Parse(Console.ReadLine());
            Console.WriteLine("How many units of the product are on order:");
            product.UnitsOnOrder = short.Parse(Console.ReadLine());
            Console.WriteLine("What is the reorder quantity:");
            product.ReorderLevel = short.Parse(Console.ReadLine());
            Console.WriteLine("Is product discontinued yes or no:");

            // allow use of yes or no for boolean discountinued value
            string discontinuedYesorNo;
            while (true)
            {
                discontinuedYesorNo = Console.ReadLine().ToLower();
                if (discontinuedYesorNo == "yes" || discontinuedYesorNo == "no")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("enter 'yes' or 'no'.");
                }
            }
            product.Discontinued = discontinuedYesorNo == "yes";

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Products.Any(p => p.ProductName == product.ProductName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                }
                else
                {
                    // add the category to the database
                    db.Products.Add(product);
                    db.SaveChanges();
                    logger.Info("Validation passed");
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
        }

        else if (choice == "5") //edit a Product
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Enter the product id to edit:");
            Console.ForegroundColor = ConsoleColor.White;
            int editProductid = int.Parse(Console.ReadLine());
            Product editProduct = db.Products.SingleOrDefault(p => p.ProductId == editProductid);
            if (editProduct != null)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Product Name:");
                Console.ForegroundColor = ConsoleColor.White;
                editProduct.ProductName = Console.ReadLine();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Supplier ID:");
                Console.ForegroundColor = ConsoleColor.White;
                editProduct.SupplierId = int.Parse(Console.ReadLine());

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Category ID:");
                Console.ForegroundColor = ConsoleColor.White;
                editProduct.CategoryId = int.Parse(Console.ReadLine());

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Update the quantity per unit of product:");
                Console.ForegroundColor = ConsoleColor.White;
                editProduct.QuantityPerUnit = Console.ReadLine();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Update product price in US $:");
                Console.ForegroundColor = ConsoleColor.White;
                editProduct.UnitPrice = decimal.Parse(Console.ReadLine());

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Edit the units of the product in stock:");
                Console.ForegroundColor = ConsoleColor.White;
                editProduct.UnitsInStock = short.Parse(Console.ReadLine());

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Edit the units on order of the product:");
                Console.ForegroundColor = ConsoleColor.White;
                editProduct.UnitsOnOrder = short.Parse(Console.ReadLine());

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Edit the reorder level of the product:");
                Console.ForegroundColor = ConsoleColor.White;
                editProduct.ReorderLevel = short.Parse(Console.ReadLine());

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Is the product discountinued?:");
                Console.ForegroundColor = ConsoleColor.White;
                // allow use of yes or no for boolean discountinued value
                string discontinuedYesorNo;
                while (true)
                {
                    discontinuedYesorNo = Console.ReadLine().ToLower();
                    if (discontinuedYesorNo == "yes" || discontinuedYesorNo == "no")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("enter 'yes' or 'no'.");
                    }
                }
                editProduct.Discontinued = discontinuedYesorNo == "yes";

                // Save changes to the database
                db.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{editProductid} was updated");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Product {editProductid} not found.");
                logger.Warn($"Product ID: {editProductid} is not in the database.");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        else if (choice == "6") //display all categories
        {
            var query = db.Categories.OrderBy(p => p.CategoryName);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName} - {item.Description}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (choice == "7") //display all products with categories
        {
            var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
            foreach (var item in query)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{item.CategoryName}");
                foreach (Product p in item.Products)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\t{p.ProductName}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
        else if (choice == "8") //add a new category
        {
            Category category = new Category();
            Console.WriteLine("Enter Category Name:");
            category.CategoryName = Console.ReadLine();
            Console.WriteLine("Enter the Category Description:");
            category.Description = Console.ReadLine();
            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                }
                else
                {
                    // add the category to the database
                    db.Categories.Add(category);
                    db.SaveChanges();
                    logger.Info("Validation passed");
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
        }
        else if (choice == "9") //edit a category
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Category to edit :");
            Console.ForegroundColor = ConsoleColor.White;
            string categoryName = Console.ReadLine();
            Category editCategory = db.Categories.FirstOrDefault(c => c.CategoryName == categoryName);
            if (editCategory != null)
            {
                Console.WriteLine("Edit Category Description:");
                string newDescription = Console.ReadLine();

                // Update category description
                editCategory.Description = newDescription;

                // Save changes to the database
                db.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{categoryName} was updated");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{categoryName} not found.");
                logger.Warn($"Category {categoryName} is not in the database.");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        else if (choice == "10") //Display specific category with it's products
        {
             Console.ForegroundColor = ConsoleColor.Gray;
        {
            var query = db.Categories.OrderBy(p => p.CategoryId);

            Console.WriteLine("Select the category whose products you want to display:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            int id = int.Parse(Console.ReadLine());
            Console.Clear();
            logger.Info($"CategoryId {id} selected");
            Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
            Console.WriteLine($"{category.CategoryName} - {category.Description}");
            foreach (Product p in category.Products)
            {
                Console.WriteLine($"\t{p.ProductName}");
            }
        }
    }
    Console.WriteLine();

} while (choice.ToLower() != "q") ;
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}
logger.Info("Program ended");
