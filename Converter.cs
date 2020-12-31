using System;
using System.Collections.Generic;
using System.Text;

namespace ResUtils
{
    public class Converter
    {
        public static string Date_To_String(DateTime dateTime)
        {
            return $"{dateTime.Day}/{dateTime.Month}/{dateTime.Year} - {dateTime.Hour}:{(dateTime.Minute < 10 ? "0" + dateTime.Minute : dateTime.Minute)}";
        }
    }
}
