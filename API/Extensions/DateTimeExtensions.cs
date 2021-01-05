using System;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dod) 
        {
            var today = DateTime.Today;
            var age = today.Year - dod.Year;

            if (dod.Date > today.AddYears(-age)) age--;

            return age;
        }
    }
}