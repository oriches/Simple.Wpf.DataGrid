using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Simple.Wpf.DataGrid.Models;

namespace Simple.Wpf.DataGrid.Extensions
{
    public static class MemoryExtensions
    {
        private static readonly IDictionary<MemoryUnits, string> UnitsAsString = new Dictionary<MemoryUnits, string>();

        private static readonly IDictionary<MemoryUnits, decimal> UnitsMultiplier =
            new Dictionary<MemoryUnits, decimal>();

        private static readonly Type MemoryUnitsType = typeof(MemoryUnits);

        public static string WorkingSetPrivateAsString(this Memory memory)
        {
            var valueAsString = decimal.Round(memory.WorkingSetPrivate * GetMultiplier(MemoryUnits.Mega), 2)
                .ToString(CultureInfo.InvariantCulture);

            return valueAsString + " " + GetUnitString(MemoryUnits.Mega);
        }

        public static string ManagedAsString(this Memory memory)
        {
            var valueAsString = decimal.Round(memory.Managed * GetMultiplier(MemoryUnits.Mega), 2)
                .ToString(CultureInfo.InvariantCulture);

            return valueAsString + " " + GetUnitString(MemoryUnits.Mega);
        }

        private static decimal GetMultiplier(MemoryUnits units)
        {
            if (UnitsMultiplier.TryGetValue(units, out var unitsMultiplier)) return unitsMultiplier;

            unitsMultiplier = 1 / Convert.ToDecimal((int) units);

            UnitsMultiplier.Add(units, unitsMultiplier);
            return unitsMultiplier;
        }

        private static string GetUnitString(MemoryUnits units)
        {
            if (UnitsAsString.TryGetValue(units, out var unitsString)) return unitsString;

            string unitAsString;
            switch (units)
            {
                case MemoryUnits.Bytes:
                    unitAsString = "Bytes";
                    break;
                case MemoryUnits.Kilo:
                    unitAsString = "Kilo";
                    break;
                case MemoryUnits.Mega:
                    unitAsString = "Mega";
                    break;
                case MemoryUnits.Giga:
                    unitAsString = "Giga";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(units), @"Unknown units of memory!");
            }

            var memInfo = MemoryUnitsType.GetMember(unitAsString);
            var attributes = memInfo[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            unitsString = ((DescriptionAttribute) attributes[0]).Description;

            UnitsAsString.Add(units, unitsString);
            return unitsString;
        }
    }
}