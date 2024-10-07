using RoleMining.Library.Classes;
using System.Collections.Generic;

namespace RoleMining.Library.Algorithms
{
    /// <summary>
    /// Represents an interface for recommending access in a role.
    /// </summary>
    public interface IAccessInRoleRecommender
    {
        /// <summary>
        /// Calculates how well an extra access fits a role.
        /// </summary>
        /// <param name="userAccesses">The collection of user accesses.</param>
        /// <param name="userInRoles">The collection of user in roles.</param>
        /// <returns>A list of Jaccard objects representing the scores.</returns>
        List<Score> CalculateScores(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles);
    }
}
