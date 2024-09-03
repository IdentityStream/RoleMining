using System;
using System.Collections.Generic;
using System.Text;

namespace RoleMining.Library
{
    /// <summary>
    /// Output for a future access-to-be-added to an existing role
    /// </summary>
    public class RecommendedNewAccessInRole
    {
        /// <summary>
        /// The role ID, unique identifier for each role
        /// </summary>
        public string RoleID { get; set; }
        /// <summary>
        /// The access level, something that is unique to each access and access level. Example: access: "Github", access level: "Admin". The access parameter would then be "Github - Admin"
        /// </summary>
        public string AccessID { get; set; }
        /// <summary>
        /// The amount of users in that role
        /// </summary>
        public int TotalUsers { get; set; }
        /// <summary>
        /// The amount of users that have the access level as extra access compared to how many users there are in the role
        /// </summary>
        public double Ratio { get; set; }
    }
}
