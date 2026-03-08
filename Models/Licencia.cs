namespace LicenciaSistemas.Models
{
    public class Licencia
    {
        public int Id { get; set; }

        public string NombreEmpresa { get; set; }

        public string Descripcion { get; set; }

        public string DescripcionBloqueo { get; set; }

        public decimal CuotaPagar { get; set; }

        public decimal CuotaPagada { get; set; }

        public string NumeroHabilitacion { get; set; }

        public bool Habilitado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }
    }
}
