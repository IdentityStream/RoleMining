using System;
using System.Collections.Generic;

namespace RoleMining.Library
{
    /// <summary>
    /// Combination of a user and a role
    /// </summary>
    public class UserInRole
    {
        /// <summary>
        /// The role ID, unique identifier for each role.
        /// </summary>
        public string RoleID { get; set; }
        /// <summary>
        /// Unique identifier for each user.
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// Function to check if this UserInRole object is equal to another object.
        /// </summary>
        /// <param name="obj">Object to be compared, usually another <seealso cref="UserInRole">UserInRole</seealso> object.</param>
        /// <returns>A bool representing if they are equal or not.</returns>
        public override bool Equals(object obj)
        {
            if (obj is UserInRole other)
            {
                return RoleID == other.RoleID && UserID == other.UserID;
            }
            return false;
        }
        /// <summary>
        /// Function to return the hash code for the UserInRole object, combination of UserID and RoleID.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return $"{UserID} - {RoleID}".GetHashCode();
        }
    }
}