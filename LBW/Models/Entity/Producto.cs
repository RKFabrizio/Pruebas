namespace LBW.Models.Entity
{
    public partial class Producto
    {
        public Producto()
        {
            SubProductos = new HashSet<SubProducto>();
        }
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public int CategoriaID { get; set; }


        public virtual Categoria IdCatNavigation { get; set; }
        
        public virtual ICollection<SubProducto> SubProductos { get; set; }

    }
}
