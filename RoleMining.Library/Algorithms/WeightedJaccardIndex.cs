using RoleMining.Library.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoleMining.Library.Algorithms
{
    /// <summary>
    /// Class for calculating the weighted Jaccard index
    /// </summary>
    public class WeightedJaccardIndex
    { 
        /// <summary>
        /// Finding the Jaccard Index, and the weighted Jaccard Index of extra accesses to roles. A high Jaccard index means that the access is similar to the role. (E.g. 1.0 = 100% of users in role has this extra access)
        /// </summary>
        /// <param name="userAccesses">List of users and their accesses</param>
        /// <param name="userInRoles">List of users and their roles</param>
        /// <returns></returns>
        public static List<Jaccard> JaccardIndices(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
        {
            InputValidator.CheckIfEmpty(userAccesses, nameof(userAccesses));
            InputValidator.CheckIfEmpty(userInRoles, nameof(userInRoles));

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

            foreach (var userAccess in distinctUserAccesses)
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
                    var intersection = roleToUsers.Value.Intersect(accessToUsers.Value).Count(); // Amount of users with the same role and same access. List<string> intersect List<string>, the first list is users in the role and the second list is users with the access
                    var union = roleToUsers.Value.Count() - intersection + accessToUsers.Value.Count();                        // Amount of users from role + Amount of users from access
                    var jaccardIndex = (double)intersection / union;                             // Similarity between the access and the role, high similarity = high Jaccard index

                    var amountOfUsersInRole = roleToUsers.Value.Count;

                    var weight = CalculateJaccardWeight(amountOfUsersInRole, intersection);

                    jaccardIndices.Add(new Jaccard
                    {
                        JaccardIndex = jaccardIndex,
                        WeightedJaccardIndex = jaccardIndex * weight,
                        RoleID = roleToUsers.Key,
                        AccessID = accessToUsers.Key,
                        UsersWithAccessAndRole = intersection,
                        UsersWithoutAccessWithRole = roleToUsers.Value.Count() - intersection
                    });
                }
            }

            return jaccardIndices;
        }

        /// <summary>
        /// Calculates the weight used in the weighted Jaccard index
        /// </summary>
        /// <param name="amountOfUsersInRole"></param>
        /// <param name="amountOfUsersWithAccess"></param>
        /// <returns></returns>
        public static double CalculateJaccardWeight(int amountOfUsersInRole, int amountOfUsersWithAccess)
        {
            // Penalty for adding users that do not have the access
            // The penalty is calculated by taking the value of the difference between the amount of users in the role and the intersection, divided by the amount of users in the role
            // Example: 10 users in the role, 1 users with the access
            // Result: 10 - 1 / 10 = 0.9, 1-0.9 = 0.1. This means that the Jaccard index will be multiplied by 0.1
            var penalty = (amountOfUsersInRole - amountOfUsersWithAccess) / (double)amountOfUsersInRole;

            // Reward based on extra accesses removed
            // The reward is based on how many accesses are removed
            // Need to find the way to implement this. I might use the logorithm of the amount of users with the access. So at base value i would do *10
            // 
            var reward = Math.Log10(amountOfUsersWithAccess + 1);

            var weight = (1 - penalty) * reward;
            return weight;
        }
    }
}
