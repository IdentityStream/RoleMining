using FluentValidation;
using FluentValidation.Results;
using RoleMining.Library.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoleMining.Library.Algorithms
{
    /// <summary>
    /// Class that contains the method <see cref="JaccardIndex.JaccardIndices"/> that calculates the Jaccard Index of extra accesses to roles.
    /// </summary>
    public class JaccardIndex
    {
        /// <summary>
        /// Finding the Jaccard Index of extra accesses to roles. A high Jaccard index means that the access fits the role.
        /// </summary>
        /// <param name="userAccesses">A IEnumerable of <see cref="UserAccess"/>, where all accesses are extra accesses</param>
        /// <param name="userInRoles">A IEnumerable of <see cref="UserInRole"/></param>
        /// <returns>An ordered list of <see cref="Jaccard"/></returns>
        public static List<Jaccard> JaccardIndices(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
        {
            InputValidator.Validate(userAccesses, nameof(userAccesses));
            InputValidator.Validate(userInRoles, nameof(userInRoles));

            // Bad data handling
            // To be implented

            // Access -> Users
            var accessesWithListOfUsers = userAccesses.GroupBy(uA => uA.AccessID)
                .ToDictionary(group => group.Key, group => new HashSet<string>(group.Select(uA => uA.UserID).ToList()));

            // Role -> Users
            var usersInRolesDictionary = userInRoles.GroupBy(uIR => uIR.RoleID)
                .ToDictionary(group => group.Key, group => new HashSet<string>(group.Select(uIR => uIR.UserID).ToList()));

            // User -> Roles
            var userToRoles = userInRoles.GroupBy(uIR => uIR.UserID)
                .ToDictionary(group => group.Key, group => new HashSet<string>(group.Select(uIR => uIR.RoleID).ToList()));

            // Access -> Roles
            var rolesThatContainsUsersWithThisAccess = new Dictionary<string, HashSet<string>>();

            foreach (var access in accessesWithListOfUsers)
            {
                foreach (var userID in access.Value)
                {
                    // In the case where a user has no roles, we ignore it
                    if (!userToRoles.ContainsKey(userID))
                    {
                        continue;
                    }
                    foreach (var role in userToRoles[userID])
                    {
                        if (!rolesThatContainsUsersWithThisAccess.ContainsKey(access.Key))
                        {
                            rolesThatContainsUsersWithThisAccess[access.Key] = new HashSet<string>();
                        }

                        rolesThatContainsUsersWithThisAccess[access.Key].Add(role);
                    }
                }
            }


            var jaccardIndices = new List<Jaccard>();

            // We find every extra access and role combination
            foreach (var accessWithListOfUsers in accessesWithListOfUsers)
            {
                var accessID = accessWithListOfUsers.Key;
                // In the case where a user has no roles, we ignore it
                if (!rolesThatContainsUsersWithThisAccess.ContainsKey(accessID))
                {
                    continue;
                }
                foreach (var roleID in rolesThatContainsUsersWithThisAccess[accessID])
                {
                    var usersWithExtraAccess = accessWithListOfUsers.Value;
                    var usersWithRole = usersInRolesDictionary[roleID];
                    int usersWithRoleAndExtraAccess = 0;

                    // Count how many users in the role has the access
                    foreach (var user in usersWithRole)
                    {
                        if (usersWithExtraAccess.Contains(user))
                        {
                            usersWithRoleAndExtraAccess++;
                        }
                    }

                    var union = usersWithRole.Count() + usersWithExtraAccess.Count() - usersWithRoleAndExtraAccess;
                    var jaccardIndex = (double)usersWithRoleAndExtraAccess / union; // Similarity between the access and the role, high similarity = high Jaccard index

                    jaccardIndices.Add(new Jaccard
                    {
                        JaccardIndex = jaccardIndex,
                        RoleID = roleID,
                        AccessID = accessID,
                        UsersWithAccessAndRole = usersWithRoleAndExtraAccess,
                        UsersWithoutAccessWithRole = usersWithRole.Count() - usersWithRoleAndExtraAccess
                    });
                }
            }
            return jaccardIndices.OrderByDescending(j => j.JaccardIndex).ThenByDescending(j => j.UsersWithAccessAndRole).ToList();
        }
    }
}
