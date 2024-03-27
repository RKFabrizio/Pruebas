namespace LBW.Models.Entity
{
    public partial class Categoria
    {
        public Categoria()
        {
            Productos = new HashSet<Producto>();
        }
        public int CategoriaID { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
