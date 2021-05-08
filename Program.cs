using Coffee.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Coffee

{
    class Program
    {
        public static string GetConnectionString()
        {
            string connetionString = @"Server=localhost;Database=coffee;Trusted_Connection=True;";
            return connetionString;
        }
        static void Main(string[] args)
        {
            int[] availableCoins = { 50, 100, 200, 500 };
            int amount = 0;

            List<Products> products = GetProducts();
            Ingredients ingredients = GetIngredients();
            
            //Server=localhost;Database=master;Trusted_Connection=True;
            SqlConnection cnn = new SqlConnection(GetConnectionString());
            cnn.Open();
            Console.WriteLine("Connection Open  !");
            cnn.Close();

            Console.Write("Coins: ");
            string coinLine = Console.ReadLine();
            string[] coinsArray = coinLine.Split(',');
            for (int i = 0; i < coinsArray.Length; i++)
            {
                int currentCoin = Convert.ToInt32(coinsArray[i]);
                if (Array.IndexOf(availableCoins, currentCoin) < 0)
                {
                    Console.WriteLine("Not valid coin");
                    return;
                }
                else
                    amount += currentCoin;
            }

            bool coffeeFinded = false;
            int change = 0;
            while(true)
            {
                Console.Write("Coffee Type: ");
                int coffeeType = Convert.ToInt32(Console.ReadLine());

                foreach (Products product in products)
                {
                    if (product.Id == coffeeType)
                    {
                        coffeeFinded = true;
                        if (amount >= product.Price)
                        {
                            if (ingredients.Coffee < product.Coffee)
                            {
                                Console.WriteLine("Coffee is few");
                                break;
                            }


                            if (ingredients.Sugar < product.Sugar)
                            {
                                Console.WriteLine("Sugar is few");
                                break;
                            }


                            if (ingredients.Water < product.Water)
                            {
                                Console.WriteLine("Water is few");
                                break;
                            }

                            ingredients.Coffee -= product.Coffee;
                            ingredients.Sugar -= product.Sugar;
                            ingredients.Water -= product.Water;

                            change = amount - product.Price;
                            if (change > 0)
                            {
                                Console.WriteLine($"Your change: {change}");
                            }

                            Console.WriteLine("Coffee is ready");
                            break;
                        }
                        else if (amount < product.Price)
                        {
                            Console.WriteLine("Low coins");
                            break;
                        }
                    }
                }

                amount = change >= 0 ? change : amount;

                if(amount == 0)
                {
                    Console.WriteLine("You have not money");
                    break;
                }

                if(amount > 0)
                {
                    int EnteredKey;

                    do
                    {
                        Console.WriteLine("Please Enter 0");
                        EnteredKey = Convert.ToInt32(Console.ReadLine());
                    } while (EnteredKey != 0);

                    do
                    {
                        Console.WriteLine("Enter 1 if you want to take change or 2 to take coffee");
                        EnteredKey = Convert.ToInt32(Console.ReadLine());
                    } while (EnteredKey != 1 && EnteredKey != 2);

                    if(EnteredKey == 1)
                    {
                        Console.WriteLine($"Your change: {amount}");
                        break;
                    }
                }

                if (!coffeeFinded)
                {
                    Console.WriteLine("Not valid coffee");
                }
            }
        }

        public static List<Products> GetProducts()
        {
            List<Products> products = new List<Products>();
            SqlConnection cnn = new SqlConnection(GetConnectionString());
            using (SqlCommand command = new SqlCommand("SELECT * FROM Products", cnn)) //pass SQL query created above and connection
            {
                cnn.Open();
                SqlDataReader reader = command.ExecuteReader(); //execute the Query
                while (reader.Read())
                {
                    Products product = new Products();
                    product.Id = Convert.ToInt32(reader["Id"]);
                    product.Name = reader["Name"].ToString();
                    product.Price = Convert.ToInt32(reader["Price"]);
                    product.Coffee = Convert.ToDouble(reader["Coffee"]);
                    product.Sugar = Convert.ToDouble(reader["Sugar"]);
                    product.Water = Convert.ToDouble(reader["Water"]);
                    products.Add(product);
                }
            }
            return products;
        }

        public static Ingredients GetIngredients()
        {
            Ingredients ingredients = new Ingredients();
            SqlConnection cnn = new SqlConnection(GetConnectionString());
            using (SqlCommand command = new SqlCommand("SELECT * FROM Ingredients", cnn)) //pass SQL query created above and connection
            {
                cnn.Open();
                SqlDataReader reader = command.ExecuteReader(); //execute the Query
                
                if(reader.Read())
                {
                    ingredients.Coffee = Convert.ToDouble(reader["Coffee"]);
                    ingredients.Sugar = Convert.ToDouble(reader["Sugar"]);
                    ingredients.Water = Convert.ToDouble(reader["Water"]);
                }
            }

            return ingredients;
        }
    }

}
