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
        /// <param name="userAccesses">A IEnumerable of <see cref="UserAccess"/>, where all accesses are extra accesses</param>
        /// <param name="userInRoles">A IEnumerable of <see cref="UserInRole"/></param>
        /// <returns></returns>
        public static List<AccessInRoleSummarized> SummarizeRoles(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
        {
            return RoleSummarizer.SummarizeRoles(userAccesses, userInRoles);
        }

        /// <summary>
        /// Finding the Jaccard Index of extra accesses to roles. A high Jaccard index means that the access fits the role.
        /// </summary>
        /// <param name="userAccesses">A IEnumerable of <see cref="UserAccess"/>, where all accesses are extra accesses</param>
        /// <param name="userInRoles">A IEnumerable of <see cref="UserInRole"/></param>
        /// <returns></returns>
        public static List<Jaccard> JaccardIndices(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
        {
            return JaccardIndex.JaccardIndices(userAccesses, userInRoles);
        }
    }
}
