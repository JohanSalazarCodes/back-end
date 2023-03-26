using back_end.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace back_end.Entidades
{
    public class Genero
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 10)]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        public List<PeliculasGeneros> PeliculasGeneros { get; set; }

        //[Range(18, 120)]
        //public int Edad { get; set; }

        //[CreditCard]
        //public string TarjetaDeCredito { get; set; }

        //[Url]
        //public string URL { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //   if (!string.IsNullOrEmpty(Nombre))
        //    {
        //        var primeraLEtra = Nombre[0].ToString();

        //        if (primeraLEtra != primeraLEtra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra debe ser mayúscula",
        //                new string[] {nameof(Nombre)});
        //        }
        //    }
        //}
    }
}
