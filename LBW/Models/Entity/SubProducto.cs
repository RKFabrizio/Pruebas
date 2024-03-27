namespace LBW.Models.Entity
{
    public partial class SubProducto
    {
        public int SubProductoID { get; set; }
        public string Nombre { get; set; }
        public int ProductoID { get; set; }
        public int EstadoID { get; set; }
        public virtual Producto IdProNavigation { get; set; }
        public virtual Estado IdEstNavigation {  get; set; }
    }
}
