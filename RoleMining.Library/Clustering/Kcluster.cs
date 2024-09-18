using RoleMining.Library.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace RoleMining.Library.Clustering
{
    public class Kcluster
    {
        public static List<UserCluster> Cluster(IEnumerable<UserAccess> userAccesses, int similarityThreshold = 1)
        {
            InputValidator.CheckIfEmpty(userAccesses, nameof(userAccesses));

            var userAccessVectors = new List<UserAccessVector>();

            var uniqueAccesses = userAccesses.Select(uA => uA.AccessID).Distinct().ToList();
            var uniqueUsers = userAccesses.Select(uA => uA.UserID).Distinct().ToList();


            foreach (var user in uniqueUsers)
            {
                var userVector = new UserAccessVector{UserID = user, AccessVector = new BitArray(uniqueAccesses.Count())};

                foreach (var access in uniqueAccesses)
                {
                    if (userAccesses.Any(uA => uA.UserID == user && uA.AccessID == access))
                    {
                        userVector.AccessVector[uniqueAccesses.IndexOf(access)] = true;
                    }
                    else
                    {
                        userVector.AccessVector[uniqueAccesses.IndexOf(access)] = false;
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
                    if  (HammingDistance(user.AccessVector, cluster.AccessVector) < similarityThreshold)
                    {

                        cluster.Users.Add(user);
                        cluster.UsersInCluster.Add(user.UserID);

                        addedToCluster = true;
                        break;
                    }
                }

                if (!addedToCluster)
                {
                        var accessesInCluster = new List<string>();
                        var amountOfAccessesInCluster = 0;
                        foreach (var access in uniqueAccesses)
                        {
                            if (user.AccessVector[uniqueAccesses.IndexOf(access)] == true)
                            {
                                accessesInCluster.Add(access);
                                amountOfAccessesInCluster++;
                            }
                        }
                    // Create a new cluster if no similar cluster is found
                    var userList = new List<UserAccessVector> { user };
                    clusters.Add(new UserCluster
                    {
                        Accesses = accessesInCluster,
                        AmountOfAccesses = amountOfAccessesInCluster,
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

        var distance = 0;
        for (var i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
            {
                distance++;
            }
        }
        return distance;
    }
    public class UserAccessVector
    {
        public string UserID { get; set; }
        public BitArray AccessVector { get; set; } // BitArray for binary role presence
    }
    public class UserCluster
    {
        public List<string> Accesses { get; set; }
        public int AmountOfAccesses  { get; set; }
        public List<string> UsersInCluster { get; set; }

        public List<UserAccessVector> Users { get; set; }
        public BitArray AccessVector { get; set; }

    }
    }
}
