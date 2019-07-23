using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using pizzaApp.Domain.Model;
using pizzaApp.Domain.Request;
using pizzaApp.Services.Interfaces;

namespace pizzaApp.Services
{
    public class PizzaService : IPizzaService
    {
        private IConfiguration _connectionString;
        private ICacheService _cacheService;
        private static readonly MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        private static readonly string key = "Pizza_CacheData_";

        public PizzaService(IConfiguration configuration, ICacheService cacheService)
        {
            _connectionString = configuration;
            _cacheService = cacheService;
        }

        public List<Pizzas> GetPizzaAndTopping()
        {
            List<Pizzas> pizzas = null;
            Pizzas pizza = null;
            List<string> toppings = null;
            Dictionary<int, List<string>> dict = new Dictionary<int, List<string>>();

            using (var con = GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = "dbo.Pizzas_GetAllWithToppings";
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        pizza = new Pizzas();
                        int index = 0;

                        pizza.Id = reader.GetInt32(index++);
                        pizza.Name = reader.GetString(index++);
                        pizza.DateCreated = reader.GetDateTime(index++);
                        pizza.DateModified = reader.GetDateTime(index++);

                        if(pizzas == null)
                        {
                            pizzas = new List<Pizzas>();
                        }
                        pizzas.Add(pizza);
                    }   

                    reader.NextResult();

                    while (reader.Read())
                        {
                            int index = 0;
                            int pizzaId = reader.GetInt32(index++);
                            string topping = reader.GetString(index++);
                            if (toppings == null)
                            {
                            toppings = new List<string>();
                            }

                        if (!dict.ContainsKey(pizzaId))
                        {
                            dict[pizzaId] = new List<string>();
                        }
                        if (topping != null)
                            {
                                dict[pizzaId].Add(topping);
                            }       
                        }
                   if(dict != null)
                    {
                        foreach (Pizzas onePizza in pizzas)
                        {
                            if (dict.ContainsKey(onePizza.Id))
                            {
                                onePizza.Toppings = dict[onePizza.Id];
                            }
                        }
                    }
                   
                    return pizzas;
                }
            }
        }

            return toppings;
        }

        private SqlConnection GetConnection()
        {
            var con = new SqlConnection(_connectionString.GetSection("ConnectionStrings").GetSection("default").Value);

            con.Open();
            return con;
        }
    }
}

