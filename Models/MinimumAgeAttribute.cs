using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CS3750_PlanetExpressLMS.Models
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        // Variable to hold the minimum age
        int _minimumAge; 

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="minimumAge"></param>
        public MinimumAgeAttribute(int minimumAge)
        {
            // Set the local var age to the new instance of age passed in
            _minimumAge = minimumAge;
        }

        /// <summary>
        /// Override the IsValid Method
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            // Create a datetime object
            DateTime date; 

            // Checks if user's age is less than the current date
            if(DateTime.TryParse(value.ToString(), out date))
            {
                // Valid age
                return date.AddYears(_minimumAge) < DateTime.Now;
            }

            // Age requirement is not met
            return false;
        }
    }
}
