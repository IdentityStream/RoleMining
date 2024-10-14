using Microsoft.Extensions.DependencyInjection;
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
        /// <param name="services">Your ServiceCollection</param>
        /// <param name="algorithm">RecommenderAlgorithm to be added to the ServiceCollection</param>
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
    /// <summary>
    /// Algoritms that can be used for recommending accesses to be baked into roles.
    /// </summary>
    public enum RecommenderAlgorithm
    {
        /// <summary>
        /// See <see cref="JaccardIndex.CalculateScores"/>
        /// </summary>
        JaccardIndex,
    }
}
