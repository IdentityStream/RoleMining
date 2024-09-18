using RoleMining.Library.Classes;
using System.Collections.Generic;
using System.Linq;

namespace RoleMining.Library.Algorithms
{
    /// <summary>
    /// 
    /// </summary>
    public class JaccardIndex
    {
        /// <summary>
        /// Finding the Jaccard Index of extra accesses to roles. A high Jaccard index means that the access fits the role.
        /// </summary>
        /// <param name="userAccesses"></param>
        /// <param name="userInRoles"></param>
        /// <returns></returns>
        public static List<Jaccard> JaccardIndices(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
        {
            // Matching extra access to existing roles based on how many of the users in the role has the access
            InputValidator.CheckIfEmpty(userAccesses, nameof(userAccesses));
            InputValidator.CheckIfEmpty(userInRoles, nameof(userInRoles));

            // Bad data handling
            // To be implented

            // Access -> Users
            var accsessesWithListOfUsers = userAccesses.GroupBy(uA => uA.AccessID)
                .ToDictionary(group => group.Key, group => new HashSet<string>(group.Select(uA => uA.UserID).ToList()));

            // Role -> Users
            var usersInRolesDictionary = userInRoles.GroupBy(uIR => uIR.RoleID)
                .ToDictionary(group => group.Key, group => new HashSet<string>(group.Select(uIR => uIR.UserID).ToList()));

            // User -> Roles
            var userToRoles = userInRoles.GroupBy(uIR => uIR.UserID)
                .ToDictionary(group => group.Key, group => new HashSet<string>(group.Select(uIR => uIR.RoleID).ToList()));

            // Access -> Roles
            var rolesThatContainsUsersWithThisAccess = new Dictionary<string, HashSet<string>>();

            foreach (var access in accsessesWithListOfUsers)
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
            foreach (var accessWithListOfUsers in accsessesWithListOfUsers)
            {
                // In the case where a user has no roles, we ignore it
                if (!rolesThatContainsUsersWithThisAccess.ContainsKey(accessWithListOfUsers.Key))
                {
                    continue;
                }
                foreach (var roleID in rolesThatContainsUsersWithThisAccess[accessWithListOfUsers.Key])
                {
                    int usersWithRoleAndExtraAccess = 0;// Amount of users with the same role and same access (as extra). List<string> intersect List<string>, the first list is users in the role and the second list is users with the access

                    // Count how many users in the role has the access
                    foreach (var user in usersInRolesDictionary[roleID])
                    {
                        if (accessWithListOfUsers.Value.Contains(user))
                        {
                            usersWithRoleAndExtraAccess++;
                        }
                    }

                    // Amount of users from role + Amount of users from access
                    var union = accessWithListOfUsers.Value.Count() + (usersInRolesDictionary[roleID].Count() - usersWithRoleAndExtraAccess);
                    var jaccardIndex = (double)usersWithRoleAndExtraAccess / union; // Similarity between the access and the role, high similarity = high Jaccard index

                    jaccardIndices.Add(new Jaccard
                    {
                        JaccardIndex = jaccardIndex,
                        WeightedJaccardIndex = 0,
                        RoleID = roleID,
                        AccessID = accessWithListOfUsers.Key,
                        UsersWithAccessAndRole = usersWithRoleAndExtraAccess,
                        UsersWithoutAccessWithRole = usersInRolesDictionary[roleID].Count() - usersWithRoleAndExtraAccess
                    });
                }
            }
            return jaccardIndices.OrderByDescending(j => j.JaccardIndex).ThenByDescending(j => j.UsersWithAccessAndRole).ToList();
        }
    }
}
