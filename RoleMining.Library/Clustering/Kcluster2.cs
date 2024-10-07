using RoleMining.Library.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoleMining.Library.Clustering
{
    public class Kcluster2
    {
        public static List<UserCluster> Cluster(IEnumerable<UserAccess> userAccesses, List<string> usersToStartClusters, int similarityThreshold = 1)
        {
            InputValidator.CheckIfEmpty(userAccesses, nameof(userAccesses));

            // Distinct AccessIDs
            var uniqueAccesses = userAccesses.Select(uA => uA.AccessID).Distinct().ToList();
            // Dictionary for the index of each access, for faster lookup
            var uniqueAccessesIndexDict = uniqueAccesses.Select((access, index) => new { access, index })
                .ToDictionary(x => x.access, x => x.index);

            // Distinct UserIDs
            var uniqueUsers = userAccesses.Select(uA => uA.UserID).Distinct().ToList();

            // Dictionary with userID as keys, and a IEnumerable of accessIDs as values in a HashSet
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

            // Manually cluster
            // As of now, first creates a cluster around the first user. If another user is outside the
            // similarity threshold, a new cluster is created. We check the user at each cluster
            var clusters = new List<UserCluster>();



            foreach (var user in userAccessVectors)
            {
                bool addedToCluster = false;

                foreach (var cluster in clusters)
                {
                    // Compare to the first user in the cluster
                    if (HammingDistance(user.AccessVector, cluster.AccessVector) < similarityThreshold)
                    {

                        cluster.Users.Add(user);
                        cluster.UsersInCluster.Add(user.UserID);

                        addedToCluster = true;
                        break;
                    }
                }

                if (!addedToCluster)
                {
                    var accessesInCluster = uniqueAccesses.Where((access, index) => user.AccessVector[index])
                                              .ToList();
                    // Create a new cluster if no similar cluster is found
                    var userList = new List<UserAccessVector> { user };
                    clusters.Add(new UserCluster
                    {
                        Accesses = accessesInCluster,
                        AmountOfAccesses = accessesInCluster.Count,
                        UsersInCluster = userList.Select(uL => uL.UserID).ToList(),
                        Users = userList,
                        AccessVector = user.AccessVector
                    });
                }
            }

            return clusters;
        }

        public static int HammingDistance(BitArray a, BitArray b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException("The two BitArrays must be of the same length.");
            }

            // XOR operation to find differing bits
            BitArray xorResult = a.Xor(b);

            // Count the number of bits true
            int distance = 0;
            foreach (bool bit in xorResult)
            {
                if (bit)
                {
                    distance++;
                }
            }

            return distance;
        }
    }

    public class UserAccessVector
    {
        public string UserID { get; set; }
        public BitArray AccessVector { get; set; } // BitArray for binary role presence
    }

    public class UserCluster
    {
        public List<string> Accesses { get; set; }
        public int AmountOfAccesses { get; set; }
        public List<string> UsersInCluster { get; set; }

        public List<UserAccessVector> Users { get; set; }
        public BitArray AccessVector { get; set; }

    }
}
