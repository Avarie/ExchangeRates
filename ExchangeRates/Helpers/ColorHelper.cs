using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeRates.Helpers
{
    public static class ColorHelper
    {
        private static List<string> AddColors(params string[] arr) => arr.OrderBy(a => Guid.NewGuid()).ToList();

        public static readonly Dictionary<ChartColor, List<string>> Colors = new Dictionary<ChartColor, List<string>>
        {
            { ChartColor.Red, AddColors("lightsalmon", "salmon", "darksalmon", "lightcoral", "indianred", "crimson", "firebrick", "red", "darkred", "coral", "tomato", "orangered", "gold", "orange", "darkorange") },
            { ChartColor.Green, AddColors("lawngreen","chartreuse","limegreen","lime","forestgreen","green","darkgreen","greenyellow","yellowgreen","springgreen","mediumspringgreen","lightgreen","palegreen","darkseagreen","mediumseagreen","seagreen","olive","darkolivegreen","olivedrab") }
        };

        public static string GetColor(ChartColor color, int index)
        {
            if (index > Colors[color].Count - 1)
            {
                index = index % (Colors[color].Count - 1);
            }

            return Colors[color][index];
        }
    }

    public enum ChartColor
    {
        Red, 
        Green
    }
}
