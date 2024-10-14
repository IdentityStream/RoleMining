﻿using Microsoft.Extensions.DependencyInjection;
using RoleMining.Library.Algorithms;

namespace RoleMining.Library
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> to add RoleMining services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Method to add RoleMining algorithm services to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="algorithm"></param>
        /// <param name=""></param>
        public static void AddRoleMining(
            this IServiceCollection services,
            RecommenderAlgorithm algorithm = RecommenderAlgorithm.JaccardIndex
            )
        {
            switch (algorithm)
            {
                case RecommenderAlgorithm.JaccardIndex:
                    services.AddTransient<IAccessInRoleRecommender, JaccardIndex>();
                    break;
            }
        }
    }
    public enum RecommenderAlgorithm
    {
        JaccardIndex,
    }
}
