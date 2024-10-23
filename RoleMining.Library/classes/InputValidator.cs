using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RoleMining.Library
{
    internal class InputValidator
    {
        public static void CheckIfEmpty(IEnumerable<object> input, string paramName)
        {
            if (input == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (input is IEnumerable enumerable && !enumerable.GetEnumerator().MoveNext())
            {
                throw new ArgumentException("Input cannot be an empty collection.", paramName);
            }
        }
    }
}
