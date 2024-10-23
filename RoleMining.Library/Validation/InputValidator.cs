using FluentValidation;
using System;

namespace RoleMining.Library.Validation
{
    internal static class InputValidator
    {
        /// <summary>
        /// Validate input using FluentValidation. Automatically finds the required validator for the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void ValidateAndThrowArgumentExceptions<T>(this IValidator<T> validator, T input, string paramName)
        {
            if (input == null)
            {
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null");
            }

            var result = validator.Validate(input);

            if (!result.IsValid)
            {
                var errorMessage = string.Join(Environment.NewLine, result.Errors);
                throw new ArgumentException($"Validation failed: {Environment.NewLine}{errorMessage}", paramName, new ValidationException(result.Errors));
            }
        }
    }
}
