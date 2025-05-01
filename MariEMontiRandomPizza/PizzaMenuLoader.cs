using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariEMontiRandomPizza
{
    internal class PizzaMenuLoader
    {
        public static List<Pizza> LoadPizzasFromCsv(string filePath)
        {
            List<Pizza> pizzas = new List<Pizza>();

            try
            {
                // Check if file exists
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Il file menu non è stato trovato: {filePath}");
                }

                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);

                // Skip header if present (first line with "Pizza,Prezzo")
                int startIndex = lines[0].Trim().StartsWith("Pizza,Prezzo", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

                // Process each line
                for (int i = startIndex; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    // Parse the line
                    Pizza pizza = ParseCsvLine(line);
                    if (pizza != null)
                    {
                        pizzas.Add(pizza);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la lettura del file CSV: {ex.Message}");
                throw;
            }

            return pizzas;
        }

        // Parse a single CSV line to create a Pizza object
        private static Pizza ParseCsvLine(string line)
        {
            try
            {
                // Handle special case for quoted entries
                List<string> values = new List<string>();
                bool inQuotes = false;
                int startPos = 0;

                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '"')
                    {
                        inQuotes = !inQuotes;
                    }
                    else if (line[i] == ',' && !inQuotes)
                    {
                        // Found a comma outside of quotes, this is a delimiter
                        values.Add(line.Substring(startPos, i - startPos).Trim());
                        startPos = i + 1;
                    }
                }

                // Add the last segment
                values.Add(line.Substring(startPos).Trim());

                // If we found exactly 2 segments (name and price)
                if (values.Count == 2)
                {
                    string fullPizzaName = values[0].Trim('"');
                    string priceStr = values[1].Trim();

                    // Convert price string to decimal (handle different formats)
                    decimal price = ParsePrice(priceStr);

                    // Extract ingredients from the name if possible
                    string name, ingredients;
                    ExtractNameAndIngredients(fullPizzaName, out name, out ingredients);

                    return new Pizza(name, price, ingredients);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'analisi della riga CSV: {line}. Errore: {ex.Message}");
            }

            return null;
        }

        // Helper method to extract both clean name and ingredients from pizza full name
        private static void ExtractNameAndIngredients(string fullPizzaName, out string name, out string ingredients)
        {
            // Check if there are parentheses in the name
            int openParenIndex = fullPizzaName.IndexOf('(');

            if (openParenIndex > 0)
            {
                // Extract the clean name (part before parenthesis)
                name = fullPizzaName.Substring(0, openParenIndex).Trim();

                // Extract ingredients (content inside parentheses)
                int closeParenIndex = fullPizzaName.LastIndexOf(')');
                if (closeParenIndex > openParenIndex)
                {
                    ingredients = fullPizzaName.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1).Trim();
                }
                else
                {
                    // No closing parenthesis found
                    ingredients = string.Empty;
                }
            }
            else
            {
                // No parentheses, use the full name as the name and empty ingredients
                name = fullPizzaName;
                ingredients = string.Empty;
            }
        }

        // Helper method to parse price strings like "€ 7.80" or "7,80€"
        private static decimal ParsePrice(string priceStr)
        {
            // Remove currency symbol and trim
            priceStr = priceStr.Replace("€", "").Trim();

            // Replace comma with dot for decimal point if needed
            priceStr = priceStr.Replace(',', '.');

            // Parse to decimal
            if (decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
            {
                return price;
            }

            throw new FormatException($"Impossibile convertire '{priceStr}' in un prezzo valido.");
        }
    }
}
