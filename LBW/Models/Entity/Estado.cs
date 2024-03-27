namespace LBW.Models.Entity
{
    public partial class Estado
    {
        public Estado() {

            SubProductos = new HashSet<SubProducto>();

        }
        public int EstadoID { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<SubProducto> SubProductos { get; set; }
    }
}
