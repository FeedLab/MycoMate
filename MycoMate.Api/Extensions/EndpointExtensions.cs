using MycoMate.Api.Endpoints;

namespace MycoMate.Api.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();
        app.MapIngredientEndpoints();
        app.MapProjectEndpoints();
        app.MapSubstrateRecipeEndpoints();

        return app;
    }
}
