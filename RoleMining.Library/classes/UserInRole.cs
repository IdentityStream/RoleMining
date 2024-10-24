﻿namespace RoleMining.Library.Classes
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


        // FUNCTIONS USED BY HASHSET TO COMPARE OBJECTS

        /// <summary>
        /// Function to return the hash code for the UserInRole object, combination of UserID and RoleID.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return $"{UserID} - {RoleID}".GetHashCode();
        }

        /// <summary>
        /// Function to check if this UserInRole object is equal to another object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is UserInRole other)
            {
                return RoleID == other.RoleID && UserID == other.UserID;
            }
            return false;
        }
    }
}