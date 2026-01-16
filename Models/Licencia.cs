namespace LicenciaSistemas.Models
{
    public class Licencia
    {
        public int Id { get; set; }
        public string NombreEmpresa { get; set; }
        public decimal CuotaPagar { get; set; }
        public decimal CuotaPagada { get; set; }
        public string NumeroHabilitacion { get; set; }
        public int Habilitado { get; set; }
    }
}
