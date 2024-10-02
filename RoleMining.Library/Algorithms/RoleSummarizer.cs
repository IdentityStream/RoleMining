using FluentValidation.Results;
using FluentValidation;
using RoleMining.Library.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoleMining.Library.Algorithms
{
    /// <summary>
    /// Class to summarize roles based on user accesses and users in roles.
    /// </summary>
    public class RoleSummarizer
    {
        /// <summary>
        /// Summarize roles and accesses based on user accesses and users in roles.
        /// </summary>
        /// <param name="userAccesses">A IEnumerable of <see cref="UserAccess"/> objects</param>
        /// <param name="userInRoles">A IEnumerable of <see cref="UserInRole"/> objects</param>
        /// <returns></returns>
        public static List<AccessInRoleSummarized> SummarizeRoles(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
        {
            InputValidator.Validate(userAccesses, nameof(userAccesses));
            InputValidator.Validate(userInRoles, nameof(userInRoles));

            // Remove duplicates and tranform to set
            var distinctUserAccesses = new HashSet<UserAccess>();
            var distinctUserInRoles = new HashSet<UserInRole>();

            foreach (var uA in userAccesses)
            {
                distinctUserAccesses.Add(uA);
            }
            foreach (var uIR in userInRoles)
            {
                distinctUserInRoles.Add(uIR);
            }


            // Convert to dictionaries
            var userAccessDict = distinctUserAccesses.ToDictionary(uA => $"{uA.UserID} - {uA.AccessID}");  // Create a dictionary of user accesses
            var userInRoleDict = distinctUserInRoles.ToDictionary(uIR => $"{uIR.UserID} - {uIR.RoleID}");   // Create a dictionary of user in roles

            // Create the list of roles and how many users are in each role
            var roleSummaries = userInRoleDict.GroupBy(uIR => uIR.Value.RoleID)
                .Select(group => new Summary
                {
                    ID = group.Key,
                    Total = group.Count()
                }).ToList();

            var accessSummaries = userAccessDict.GroupBy(uA => uA.Value.AccessID)
                .Select(group => new Summary
                {
                    ID = group.Key,
                    Total = group.Count()
                }).ToList();
            
            var ratioList = accessSummaries.Select(aS =>
            {
                // Get all extra accesses with the current access ID
                var extraAccesses = userAccessDict.Values.Where(uA => uA.AccessID == aS.ID);

                // Determine unique roles associated with these extra accesses
                var rolesWithAccess = extraAccesses.Select(uA =>
                {
                    return userInRoleDict.Values.Where(uIR => uIR.UserID == uA.UserID).Select(uIR => uIR.RoleID).Distinct();
                }).SelectMany(r => r).Distinct();

                // Create RatioAccessLevel for each role with the current access
                return rolesWithAccess.Select(roleID =>
                {
                    var usersInRole = userInRoleDict.Values.Where(uIR => uIR.RoleID == roleID).Select(uIR => uIR.UserID).Distinct();
                    var usersWithAccessInRole = extraAccesses.Count(uA => usersInRole.Contains(uA.UserID));

                    return new AccessInRoleSummarized
                    {
                        RoleID = roleID,
                        AccessID = aS.ID,
                        Ratio = usersInRole.Count() == 0 ? 0 : (double)usersWithAccessInRole / usersInRole.Count(),
                        TotalUsers = usersInRole.Count(),
                        UsersWithAccessAsExtra = usersWithAccessInRole
                    };
                }).ToList();
            }).SelectMany(result => result).ToList(); ;

            return ratioList;

        }


        /// <summary>
        /// Function to remove duplicates from a dictionary based on a selector function.
        /// </summary>
        /// <typeparam name="T">Arbitary object.</typeparam>
        /// <param name="source">Dictionary with values of type T</param>
        /// <param name="selector">Key, or part of key for the dictionary.</param>
        /// <returns>List of KeyValuePair with string and T.</returns>
        public static IEnumerable<KeyValuePair<string, T>> ReturnDistinct<T>(
        IEnumerable<KeyValuePair<string, T>> source,
        Func<T, string> selector)
        {
            var seen = new HashSet<string>(); // Use the type that matches the selector's return type
            var result = new List<KeyValuePair<string, T>>();

            foreach (var kvp in source)
            {
                var selectedElement = selector(kvp.Value); // Apply the selector on the value
                if (seen.Add(selectedElement)) // Add to the HashSet if it's a new element
                {
                    result.Add(kvp); // Add the entire KeyValuePair to the result list
                }
            }
            return result;
        }

        internal class Summary
        {
            public string ID { get; set; }
            public int Total { get; set; }
        }
    }
}
