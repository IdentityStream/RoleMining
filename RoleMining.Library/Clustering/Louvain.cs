using RoleMining.Library.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static RoleMining.Library.Clustering.Kcluster;

namespace RoleMining.Library.Clustering
{
    public class Louvain
    {
        public static void Cluster(IEnumerable<UserAccess> userAccesses)
        {
            InputValidator.CheckIfEmpty(userAccesses, nameof(userAccesses));

            var uniqueAccesses = userAccesses.Select(uA => uA.AccessID).Distinct().ToList();
            var uniqueAccessesIndexDict = uniqueAccesses.Select((access, index) => new { access, index })
                .ToDictionary(x => x.access, x => x.index);

            var uniqueUsers = userAccesses.Select(uA => uA.UserID).Distinct().ToList();

            var userAccessDict = userAccesses.GroupBy(uA => uA.UserID)
                .ToDictionary(g => g.Key, g => new HashSet<string>(g.Select(uA => uA.AccessID)));

            var userAccessVectors = new List<UserAccessVector>();

            foreach (var user in uniqueUsers)
            {
                var userVector = new UserAccessVector { UserID = user, AccessVector = new BitArray(uniqueAccesses.Count()) };
                if (userAccessDict.TryGetValue(user, out var userAccessSet))
                {
                    foreach (var access in userAccessSet)
                    {
                        if (uniqueAccessesIndexDict.TryGetValue(access, out var index))
                        {
                            userVector.AccessVector[index] = true;
                        }
                    }
                }
                userAccessVectors.Add(userVector);
            }


        }
    }
}
