namespace MySuperDuperPetProject.Middlewares
{
    public static class ApplicationBuilderMiddlewareExtensions
    {
        public static IApplicationBuilder AddCheckUserSessionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CheckUserSessionMiddleware>();
        }
        public static IApplicationBuilder AddClaimsCheckMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ClaimsCheckMiddleware>();
        }
    }
}
