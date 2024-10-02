using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RoleMining.Library.Classes
{
    internal class InputValidator
    {
        /// <summary>
        /// Validate input using FluentValidation. Automatically finds the required validator for the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void Validate<T>(T input, string paramName)
        {
            if (input == null)
            {
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null");
            }

            var validator = GetValidator<T>();
            var result = validator.Validate(input);

            if (!result.IsValid)
            {
                var errorMessage = string.Join(Environment.NewLine, result.Errors);
                throw new ArgumentException($"Validation failed: {Environment.NewLine}{errorMessage}", new ValidationException(result.Errors));
            }
        }

        private static IValidator<T> GetValidator<T>()
        {
            var validatorType = typeof(AbstractValidator<T>);

            var concreteValidatorType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => validatorType.IsAssignableFrom(t) && !t.IsAbstract);

            if (concreteValidatorType == null)
            {
                throw new ArgumentException($"No concrete validator found for type {typeof(T).Name}");
            }

            return (IValidator<T>)Activator.CreateInstance(concreteValidatorType);
        }
    }
}
