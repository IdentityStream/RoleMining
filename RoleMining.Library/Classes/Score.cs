﻿using RoleMining.Library.Algorithms;
using System.Collections.Generic;
namespace RoleMining.Library.Classes
{
    /// <summary>
    /// Return object for <see cref="IAccessInRoleRecommender.CalculateScores"/>
    /// </summary>
    public class Score
    {
        /// <summary>
        /// Jacard Index of the access to role
        /// </summary>
        public double AccessToRoleScore { get; set; }

        /// <summary>
        /// The role ID, unique identifier for each role.
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// The accessID, unique identifier for each access. (e.g. "GitHub - Admin")
        /// </summary>
        public string AccessID { get; set; }

        /// <summary>
        /// Amount of users with the role and the access
        /// </summary>
        public int UsersWithAccessAndRoleCount { get; set; }

        /// <summary>
        /// Amount of users with the access, but not with the role
        /// </summary>
        public int UsersWithAccessWithoutRoleCount { get; set; }

        /// <summary>
        /// Amount of users with the role, but not with the access
        /// </summary>
        public int UsersWithoutAccessWithRoleCount { get; set; }

        /// <summary>
        /// List of userIDs with the role
        /// </summary>
        public List<string> UsersWithAccessAndRole { get; set; }

        /// <summary>
        /// List of userIDs with the access, but not with the role
        /// </summary>
        public List<string> UsersWithAccessWithoutRole { get; set; }

        /// <summary>
        /// List of UserIDs with the role, but not with the access
        /// </summary>
        public List<string> UsersWithoutAccessWithRole { get; set; }
    }
}
