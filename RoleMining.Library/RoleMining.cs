
namespace RoleMining.Library
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    public class RoleMining
    {
        /// <summary>
        /// Mines roles based on user accesses and users in roles
        /// </summary>
        /// <param name="userAccesses">A IEnumerable of <seealso cref="UserAccess">UserAccess</seealso> objects</param>
        /// <param name="userInRoles"></param>
        /// <returns></returns>
        public static List<RatioAccessLevel> MineRoles(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
        {
            InputValidator.CheckIfEmpty(userAccesses, nameof(userAccesses));
            InputValidator.CheckIfEmpty(userInRoles, nameof(userInRoles));

            // Input testDataAccess and testDataUserInRole from RoleMining.Console/Program.cs
            // Return a RatioAccessLevel object

            Console.WriteLine("Mining roles...");

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
            var userAccessDict = distinctUserAccesses.ToDictionary(uA => $"{uA.UserID} - {uA.RoleID} - {uA.AccessLevelID}"); // Create a dictionary of user accesses
            var userInRoleDict = distinctUserInRoles.ToDictionary(uIR => $"{uIR.UserID} - {uIR.RoleID}"); // Create a dictionary of user in roles

            var specialAccesses = userAccessDict.Where(uA => uA.Value.IsExtraAccess); // Finding all extra accesses
            var distinctSpecialAccesses = ReturnDistinct(specialAccesses, uA => $"{uA.RoleID} - {uA.AccessLevelID}"); // Finding distinct extra accesses (Hopefully)
            // DistinctSpecialAccesses does only check for distinct accesses, not accesses per role. This means that if you have two different roles with the
            // same accesses, the last one will dissappear 
            // Hopefully this will not turn into another bug, trying out with adding the RoleID to the selector

            // Create the list of roles and how many users are in each role
            var roleSummaries = userInRoleDict.GroupBy(uIR => uIR.Value.RoleID)
                .Select(group => new UserRoleSummary
                {
                    Role = group.Key,
                    TotalUsers = group.Count()
                }).ToList();


            // Create the ratio per access level for all distinct special accesses
            var ratioList = distinctSpecialAccesses.Select(distinctSpecialAccess =>
            {
                var usersWithAccessAsExtra = specialAccesses.Count(uA => uA.Value.AccessLevelID == distinctSpecialAccess.Value.AccessLevelID && uA.Value.RoleID == distinctSpecialAccess.Value.RoleID);
                var typeRole = distinctSpecialAccess.Value.RoleID;

                var roleSummary = roleSummaries.FirstOrDefault(role => role.Role == $"{distinctSpecialAccess.Value.RoleID}");

                return new RatioAccessLevel
                {
                    Role = typeRole,
                    Access = distinctSpecialAccess.Value.AccessLevelID,
                    Ratio = roleSummary != null ? (double)usersWithAccessAsExtra / roleSummary.TotalUsers : 1,
                    UsersWithAccessAsExtra = usersWithAccessAsExtra,
                    TotalUsers = roleSummary != null ? roleSummary.TotalUsers : 1
                };
            }).ToList();

            return ratioList;
        }



            public static IEnumerable<KeyValuePair<string, UserAccess>> ReturnDistinct(
            IEnumerable<KeyValuePair<string, UserAccess>> source,
            Func<UserAccess, string> selector)
        {
            var seen = new HashSet<string>(); // Use the type that matches the selector's return type
            var result = new List<KeyValuePair<string, UserAccess>>();

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
    }
}
