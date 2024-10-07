using RoleMining.Library.Classes;
using System.Collections.Generic;

namespace RoleMining.Library.Algorithms
{
    public interface IAccessInRoleRecommender
    {
        List<Jaccard> CalculateScores(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles);
    }
}
