using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Simple.Wpf.DataGrid.Models;

namespace Simple.Wpf.DataGrid.Services
{
    public static class TabularDataGenerator
    {
        private static readonly Random HowManyColumnsRandom;
        private static readonly Random HowManyRowsRandom;
        private static readonly Random TypesRandom;
        private static readonly Random StringLengthRandom;
        private static readonly Random StringCharRandom;
        private static readonly Random DoubleRandom;
        private static readonly Random IntegerRandom;
        private static readonly Random UpdatesRandom;

        private static readonly IDictionary<Type, Func<object>> Types;

        static TabularDataGenerator()
        {
            HowManyColumnsRandom = new Random(Convert.ToInt32(Regex.Match(Guid.NewGuid()
                    .ToString(), @"\d+")
                .Value));
            HowManyRowsRandom = new Random(Convert.ToInt32(Regex.Match(Guid.NewGuid()
                    .ToString(), @"\d+")
                .Value));
            TypesRandom = new Random(Convert.ToInt32(Regex.Match(Guid.NewGuid()
                    .ToString(), @"\d+")
                .Value));
            StringLengthRandom = new Random(Convert.ToInt32(Regex.Match(Guid.NewGuid()
                    .ToString(), @"\d+")
                .Value));
            StringCharRandom = new Random(Convert.ToInt32(Regex.Match(Guid.NewGuid()
                    .ToString(), @"\d+")
                .Value));
            DoubleRandom = new Random(Convert.ToInt32(Regex.Match(Guid.NewGuid()
                    .ToString(), @"\d+")
                .Value));
            IntegerRandom = new Random(Convert.ToInt32(Regex.Match(Guid.NewGuid()
                    .ToString(), @"\d+")
                .Value));
            UpdatesRandom = new Random(Convert.ToInt32(Regex.Match(Guid.NewGuid()
                    .ToString(), @"\d+")
                .Value));

            Types = new Dictionary<Type, Func<object>>
            {
                {typeof(string), GenerateString},
                {typeof(double), GenerateDouble},
                {typeof(int), GenerateInteger},
                {typeof(DateTime), GenerateDateTime}
            };
        }

        private static int HowManyColumns => HowManyColumnsRandom.Next(1, 30);

        private static int HowManyRows => HowManyRowsRandom.Next(1, 20000);

        public static IEnumerable<DynamicData> CreateInitialSnapshot()
        {
            var columnCount = HowManyColumns;
            var rowCount = HowManyRows;

            //var columnCount = 5;
            //var rowCount = 50;

            var rows = new List<DynamicData>(rowCount);

            for (var i = 0; i < columnCount; i++)
            {
                var iAsString = i.ToString();
                var name = string.Empty;
                name = string.Format(i < 10 ? "col_0{0}" : "col_{0}", iAsString);

                var type = GenerateType();

                for (var j = 0; j < rowCount; j++)
                    if (i == 0)
                    {
                        var row = new DynamicData(columnCount);
                        row.Update(Constants.UI.Grids.PredefinedColumns.Id, j.ToString());
                        row.Update(name, Types[type]());

                        rows.Add(row);
                    }
                    else
                    {
                        rows[j]
                            .Update(name, Types[type]());
                    }
            }

            return rows;
        }

        private static Type GenerateType()
        {
            return Types.Keys
                .Skip(TypesRandom.Next(0, Types.Count))
                .First();
        }

        private static object GenerateDateTime()
        {
            return DateTime.Now.AddDays(IntegerRandom.Next(-4000, 9000))
                .Date;
        }

        private static object GenerateInteger()
        {
            return IntegerRandom.Next(-100000, 100000);
        }

        private static object GenerateDouble()
        {
            return DoubleRandom.NextDouble();
        }

        private static object GenerateString()
        {
            var length = StringLengthRandom.Next(4, 20);
            var sb = new StringBuilder(length);

            for (var i = 0; i < length; i++) sb.Append((char) StringCharRandom.Next(65, 91));

            return sb.ToString();
        }

        public static IEnumerable<DynamicData> CreateUpdates(IEnumerable<DynamicData> data)
        {
            var dataArray = data.ToArray();

            var numberOfUpdates = UpdatesRandom.Next(0, Convert.ToInt32(dataArray.Length * 0.1));

            var updatesHash = new HashSet<string>();

            var updates = new List<DynamicData>(numberOfUpdates);
            while (updates.Count < numberOfUpdates)
            {
                // throw this value away...
                UpdatesRandom.Next(updatesHash.Count, dataArray.Length);

                var update = dataArray[UpdatesRandom.Next(0, dataArray.Length)];
                if (!updatesHash.Contains(update.Id))
                {
                    updates.Add(update.Clone());
                    updatesHash.Add(update.Id);
                }
            }

            updates.ForEach(x =>
            {
                var propertyName = x.Properties.ToArray()[UpdatesRandom.Next(1, x.Properties.Count())];
                var property = x[propertyName];
                var propertyType = property.GetType();

                if (propertyType == typeof(int))
                {
                    var tenPercent = Convert.ToInt32((int) property * UpdatesRandom.NextDouble());
                    var delta = UpdatesRandom.Next(0, 100000) < 60000 ? -1 * tenPercent : tenPercent;

                    x.Update(propertyName, (int) property + delta);
                }
                else if (propertyType == typeof(double))
                {
                    var delta = UpdatesRandom.Next(0, 1000) < 500
                        ? -1 * UpdatesRandom.NextDouble()
                        : 1 * UpdatesRandom.NextDouble();

                    x.Update(propertyName, (double) property + delta);
                }
            });

            return updates;
        }
    }
}