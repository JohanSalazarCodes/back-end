using Microsoft.EntityFrameworkCore;

namespace back_end.Utilidades
{
    public static class HttContextExtensions
    {
        public async static Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("cantidadTotalRegistros", cantidad.ToString());

        }
    }
}
