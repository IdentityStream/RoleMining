using RoleMining.Library.Classes;
using RoleMining.Library.Algorithms;
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
        public static List<AccessInRoleSummarized> MineRoles(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
        {
            return RoleSummarizer.SummariseRoles(userAccesses, userInRoles);
        }

        /// <summary>
        /// Finding the Jaccard Index of extra accesses to roles. A high Jaccard index means that the access is similar to the role. 
        /// (E.g. 1.0 means the extra access is exclusive to role and all of the users in the role has the access)
        /// </summary>
        /// <param name="userAccesses">List of users and their accesses</param>
        /// <param name="userInRoles">List of users and their roles</param>
        /// <returns></returns>
        public static List<Jaccard> JaccardIndices(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
        {
            return JaccardIndex.JaccardIndices(userAccesses, userInRoles);
        }

        /// <summary>
        /// Finding the weight based on amount of users with and without access in role
        /// </summary>
        /// <param name="amountOfUsersInRole"></param>
        /// <param name="amountOfUsersWithAccess"></param>
        /// <returns></returns>
        public static double CalculateJaccardWeight(int amountOfUsersInRole, int amountOfUsersWithAccess)
        {
            return WeightedJaccardIndex.CalculateJaccardWeight(amountOfUsersInRole, amountOfUsersWithAccess);
        }


    }
}
