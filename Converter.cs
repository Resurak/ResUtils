using System;
using System.Collections.Generic;
using System.Text;

namespace ResUtils
{
    public class Converter
    {
        public static string DateToString(DateTime dateTime, bool addSeconds = false)
        {
            if (addSeconds)
                return $"{dateTime.Day}/{dateTime.Month}/{dateTime.Year} - {dateTime.Hour}:{(dateTime.Minute < 10 ? "0" + dateTime.Minute : dateTime.Minute)}:{(dateTime.Second < 10 ? "0" + dateTime.Second : dateTime.Second)}";
            else return $"{dateTime.Day}/{dateTime.Month}/{dateTime.Year} - {dateTime.Hour}:{(dateTime.Minute < 10 ? "0" + dateTime.Minute : dateTime.Minute)}";
        }
    }
}
