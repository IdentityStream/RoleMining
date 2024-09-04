
namespace RoleMining.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Main class for RoleMining.Library. Contains the method to mine roles.
    /// </summary>
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
            var userAccessDict = distinctUserAccesses.ToDictionary(uA => $"{uA.UserID} - {uA.RoleID} - {uA.AccessID}"); // Create a dictionary of user accesses
            var userInRoleDict = distinctUserInRoles.ToDictionary(uIR => $"{uIR.UserID} - {uIR.RoleID}"); // Create a dictionary of user in roles

            var specialAccesses = userAccessDict.Where(uA => uA.Value.IsExtraAccess); // Finding all extra accesses
            var distinctSpecialAccesses = ReturnDistinct(specialAccesses, uA => $"{uA.RoleID} - {uA.AccessID}"); // Finding distinct extra accesses (Hopefully)
            // DistinctSpecialAccesses does only check for distinct accesses, not accesses per role. This means that if you have two different roles with the
            // same accesses, the last one will dissappear 
            // Hopefully this will not turn into another bug, trying out with adding the RoleID to the selector

            // Create the list of roles and how many users are in each role
            var roleSummaries = userInRoleDict.GroupBy(uIR => uIR.Value.RoleID)
                .Select(group => new UserRoleSummary
                {
                    RoleID = group.Key,
                    TotalUsers = group.Count()
                }).ToList();


            // Create the ratio per access level for all distinct special accesses
            var ratioList = distinctSpecialAccesses.Select(distinctSpecialAccess =>
            {
                var usersWithAccessAsExtra = specialAccesses.Count(uA => uA.Value.AccessID == distinctSpecialAccess.Value.AccessID && uA.Value.RoleID == distinctSpecialAccess.Value.RoleID);
                var typeRole = distinctSpecialAccess.Value.RoleID;

                var roleSummary = roleSummaries.FirstOrDefault(role => role.RoleID == $"{distinctSpecialAccess.Value.RoleID}");

                return new RatioAccessLevel
                {
                    RoleID = typeRole,
                    AccessID = distinctSpecialAccess.Value.AccessID,
                    Ratio = roleSummary != null ? (double)usersWithAccessAsExtra / roleSummary.TotalUsers : 1,
                    UsersWithAccessAsExtra = usersWithAccessAsExtra,
                    TotalUsers = roleSummary != null ? roleSummary.TotalUsers : 1
                };
            }).ToList();

            return ratioList;
        }




        // Function for recommending new standard accesses to a role. Done through comparing users that have extra access, and users in role
        public static List<Jaccard> JaccardIndices(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
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


            // Create a dictionary to map RoleID to a list of UserIDs
            var rolesToUsers = new Dictionary<string, List<string>>();

            foreach (var userInRole in distinctUserInRoles)
            {
                if (!rolesToUsers.ContainsKey(userInRole.RoleID))
                {
                    rolesToUsers[userInRole.RoleID] = new List<string>();
                }
                rolesToUsers[userInRole.RoleID].Add(userInRole.UserID);
            }

            // Create a dictionary to map AccessID to a list of UserIDs
            var accesssToUsers = new Dictionary<string, List<string>>();

            foreach (var userAccess in distinctUserAccesses.Where(ua => ua.IsExtraAccess))
            {
                if (!accesssToUsers.ContainsKey(userAccess.AccessID))
                {
                    accesssToUsers[userAccess.AccessID] = new List<string>();
                }
                accesssToUsers[userAccess.AccessID].Add(userAccess.UserID);
            }

            // Create a list of Jaccard indices
            var jaccardIndices = new List<Jaccard>();

            // Calculate Jaccard index for each extra access to each role
            foreach (var accessToUsers in accesssToUsers) // Each access as a key, with the value as a list of users
            {
                foreach (var roleToUsers in rolesToUsers) // Each role as a key, with the value as a list of users
                {
                    var intersection = roleToUsers.Value.Intersect(accessToUsers.Value).Count(); // Amount of users with the same role and same access
                    var union = roleToUsers.Value.Union(accessToUsers.Value).Count();            // Amount of users from role + Amount of users from access
                    var jaccardIndex = (double)intersection / union;                             // Similarity between the access and the role, high similarity = high Jaccard index

                    var amountOfUsersInRole = roleToUsers.Value.Count();
                    var amountOfUsersWithAccess = intersection;


                    // Penalty for adding users that do not have the access
                    // The penalty is calculated by taking the value of the difference between the amount of users in the role and the intersection, divided by the amount of users in the role
                    // Example: 10 users in the role, 1 users with the access
                    // Result: 10 - 1 / 10 = 0.9, 1-0.9 = 0.1. This means that the Jaccard index will be multiplied by 0.1
                    var penalty = amountOfUsersInRole - intersection / (double)amountOfUsersInRole;

                    // Reward based on extra accesses removed
                    // The reward is based on how many accesses are removed
                    // Example: Removed 10 extra accesses
                    // 
                    var reward = intersection;

                    var weight = penalty * reward;

                    jaccardIndices.Add(new Jaccard                                               
                    {
                        JaccardIndex = jaccardIndex,
                        WeightedJaccardIndex = jaccardIndex * weight,
                        RoleID = roleToUsers.Key,
                        AccessID = accessToUsers.Key,
                        UsersWithAccess = intersection,
                        UsersWithoutAccess = roleToUsers.Value.Count - intersection
                    });
                }
            }

            return jaccardIndices;
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
    }
}
