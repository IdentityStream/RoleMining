using RoleMining.Library.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static RoleMining.Library.Clustering.Kcluster;

namespace RoleMining.Library.Clustering
{
    public class Cluster3
    {
        public static List<RoleCluster> ClusterMethod3(IEnumerable<UserAccess> userAccesses)
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

            var clusters = uniqueAccesses.Select(access => new RoleCluster
            {
                AccessesInCluster = new List<string> { access },
                UsersInCluster = new List<string>(),
                AccessIndexes = new List<int> { uniqueAccessesIndexDict[access] }
            }).ToList();

            foreach (var user in userAccessVectors)
            {
                foreach (var cluster in clusters)
                {
                    if (user.AccessVector[cluster.AccessIndexes[0]])
                    {
                        cluster.UsersInCluster.Add(user.UserID);
                    }
                }
            }

            bool clustersChanged;
            do
            {
                clustersChanged = false;
                int i = 0;
                while (i < clusters.Count - 1)
                {
                    var cluster = clusters[i];
                    for (int j = i + 1; j < clusters.Count; j++)
                    {
                        var clusterToCompare = clusters[j];
                        bool combine = true;

                        var commonUsers = cluster.UsersInCluster.Intersect(clusterToCompare.UsersInCluster).ToList();
                        if (commonUsers.Count == 0) continue; // Skip if no common users

                        if (commonUsers.Count != cluster.UsersInCluster.Count || commonUsers.Count != clusterToCompare.UsersInCluster.Count)
                        {
                            combine = false;
                        }
                        if (commonUsers.Count == cluster.UsersInCluster.Count && commonUsers.Count == clusterToCompare.UsersInCluster.Count)
                        {
                            foreach (var user in commonUsers)
                            {
                                var userVector = userAccessVectors.Find(uA => uA.UserID == user);
                                if (cluster.AccessIndexes.Any(aI => !userVector.AccessVector[aI]) ||
                                    clusterToCompare.AccessIndexes.Any(aIdx => !userVector.AccessVector[aIdx]))
                                {
                                    combine = false;
                                    break; // Break if not all accesses are shared between all users in cluster
                                }
                            }
                        }

                        // Check if all users share all accesses in both clusters
                        foreach (var user in commonUsers)
                        {
                            var userVector = userAccessVectors.Find(uA => uA.UserID == user);
                            if (cluster.AccessIndexes.Any(aI => !userVector.AccessVector[aI]) ||
                                clusterToCompare.AccessIndexes.Any(aIdx => !userVector.AccessVector[aIdx]))
                            {
                                combine = false;
                                break;
                            }
                        }

                        if (combine)
                        {
                            // Merge clusters
                            cluster.AccessesInCluster.AddRange(clusterToCompare.AccessesInCluster);
                            cluster.UsersInCluster.AddRange(clusterToCompare.UsersInCluster);
                            cluster.UsersInCluster = cluster.UsersInCluster.Distinct().ToList();
                            cluster.AccessIndexes.AddRange(clusterToCompare.AccessIndexes);
                            clusters.RemoveAt(j); // Remove merged cluster
                            clustersChanged = true;
                        }
                    }
                    i++;
                }
            } while (clustersChanged);

            return clusters;
        }
        public class RoleCluster
        {
            public List<string> AccessesInCluster { get; set; }
            public List<string> UsersInCluster { get; set; }
            public List<int> AccessIndexes { get; set; }
            public int ClusterAccessesAndUserCount => AccessesInCluster.Count + UsersInCluster.Count;
        }
    }
}
