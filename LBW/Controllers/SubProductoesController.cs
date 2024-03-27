using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LBW.Models.Entity;

namespace LBW.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SubProductoesController : Controller
    {
        private LbwContext _context;

        public SubProductoesController(LbwContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var subproductos = _context.SubProductos.Select(i => new {
                i.SubProductoID,
                i.Nombre,
                i.ProductoID,
                i.EstadoID
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "SubProductoID" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(subproductos, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new SubProducto();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.SubProductos.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.SubProductoID });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.SubProductos.FirstOrDefaultAsync(item => item.SubProductoID == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int key) {
            var model = await _context.SubProductos.FirstOrDefaultAsync(item => item.SubProductoID == key);

            _context.SubProductos.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> ProductosLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from producto in _context.Productos
                         join categoria in _context.Categorias on producto.CategoriaID equals categoria.CategoriaID
                         orderby producto.Nombre
                         select new
                         {
                             Value = producto.ProductoID,
                             Text = categoria.Nombre + " - " + producto.Nombre
                         };

            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }


        [HttpGet]
        public async Task<IActionResult> EstadosLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Estados
                         orderby i.Nombre
                         select new {
                             Value = i.EstadoID,
                             Text = i.Nombre
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(SubProducto model, IDictionary values) {
            string SUB_PRODUCTO_ID = nameof(SubProducto.SubProductoID);
            string NOMBRE = nameof(SubProducto.Nombre);
            string PRODUCTO_ID = nameof(SubProducto.ProductoID);
            string ESTADO_ID = nameof(SubProducto.EstadoID);

            if(values.Contains(SUB_PRODUCTO_ID)) {
                model.SubProductoID = Convert.ToInt32(values[SUB_PRODUCTO_ID]);
            }

            if(values.Contains(NOMBRE)) {
                model.Nombre = Convert.ToString(values[NOMBRE]);
            }

            if(values.Contains(PRODUCTO_ID)) {
                model.ProductoID = Convert.ToInt32(values[PRODUCTO_ID]);
            }

            if(values.Contains(ESTADO_ID)) {
                model.EstadoID = Convert.ToInt32(values[ESTADO_ID]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}