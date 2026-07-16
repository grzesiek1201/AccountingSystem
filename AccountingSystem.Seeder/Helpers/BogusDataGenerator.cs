using System.Text;

namespace AccountingSystem.Seeder.Helpers
{
    public static class BogusDataGenerator
    {
        private static readonly Random _random = new();


        public static decimal GetRandomPrice(decimal min = 10, decimal max = 5000)
        {
            return Math.Round(
                (decimal)_random.NextDouble() * (max - min) + min,
                2);
        }


        public static int GetRandomQuantity(int min = 1, int max = 20)
        {
            return _random.Next(min, max + 1);
        }


        public static DateTime GetRandomDate(
            int daysBack = 365)
        {
            return DateTime.Now
                .AddDays(-_random.Next(0, daysBack));
        }


        public static bool GetRandomBoolean(
            int probability = 50)
        {
            return _random.Next(0, 100) < probability;
        }


        public static T GetRandomElement<T>(IList<T> list)
        {
            return list[_random.Next(list.Count)];
        }


        public static string GetRandomNip()
        {
            return string.Join("",
                Enumerable.Range(0, 10)
                .Select(x => _random.Next(0, 10)));
        }


        public static string GetRandomEmail(string companyName)
        {
            var cleanName = companyName
                .ToLower()
                .Replace(" ", "")
                .Replace(".", "");

            return $"kontakt@{cleanName}.pl";
        }


        public static string GetRandomProductCode(
            string prefix,
            int number)
        {
            return $"{prefix}-{number:D3}";
        }


        public static string GetRandomCompanyName()
        {
            var names = new[]
            {
                "ABC Solutions",
                "Nova Tech",
                "Pro Business",
                "Future Systems",
                "Max Trade",
                "Global Service"
            };

            return GetRandomElement(names);
        }


        public static string GetRandomCity()
        {
            var cities = new[]
            {
                "Lublin",
                "Warszawa",
                "Kraków",
                "Poznań",
                "Wrocław",
                "Gdańsk"
            };

            return GetRandomElement(cities);
        }


        public static string GetRandomStreet()
        {
            var streets = new[]
            {
                "Lipowa",
                "Polna",
                "Ogrodowa",
                "Słoneczna",
                "Przemysłowa"
            };

            return $"ul. {GetRandomElement(streets)} {_random.Next(1, 100)}";
        }


        public static string GetRandomZipCode()
        {
            return $"{_random.Next(10, 99)}-{_random.Next(100, 999)}";
        }
    }
}