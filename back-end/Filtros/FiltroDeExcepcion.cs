using Microsoft.AspNetCore.Mvc.Filters;

namespace back_end.Filtros
{
    public class FiltroDeExcepcion: ExceptionFilterAttribute
    {
        //private readonly ILogger<MiFiltroDeAccion> _logger;
        public FiltroDeExcepcion()
        {
          //  _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
           // _logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
