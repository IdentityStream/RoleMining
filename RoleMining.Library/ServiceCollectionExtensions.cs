using Microsoft.Extensions.DependencyInjection;
using RoleMining.Library.Algorithms;

namespace RoleMining.Library
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRoleMining(
            this IServiceCollection services,
            RecommenderAlgorithm algorithm = RecommenderAlgorithm.JaccardIndex,
            RoleBuilderAlgorithm roleBuilderAlgorithm = RoleBuilderAlgorithm.Kcluster)
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
