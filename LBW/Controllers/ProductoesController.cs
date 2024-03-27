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
    public class ProductoesController : Controller
    {
        private LbwContext _context;

        public ProductoesController(LbwContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var productos = _context.Productos.Select(i => new {
                i.ProductoID,
                i.Nombre,
                i.CategoriaID
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "ProductoID" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(productos, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Producto();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Productos.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.ProductoID });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Productos.FirstOrDefaultAsync(item => item.ProductoID == key);
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
            var model = await _context.Productos.FirstOrDefaultAsync(item => item.ProductoID == key);

            _context.Productos.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> CategoriasLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Categorias
                         orderby i.Nombre
                         select new {
                             Value = i.CategoriaID,
                             Text = i.Nombre
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(Producto model, IDictionary values) {
            string PRODUCTO_ID = nameof(Producto.ProductoID);
            string NOMBRE = nameof(Producto.Nombre);
            string CATEGORIA_ID = nameof(Producto.CategoriaID);

            if(values.Contains(PRODUCTO_ID)) {
                model.ProductoID = Convert.ToInt32(values[PRODUCTO_ID]);
            }

            if(values.Contains(NOMBRE)) {
                model.Nombre = Convert.ToString(values[NOMBRE]);
            }

            if(values.Contains(CATEGORIA_ID)) {
                model.CategoriaID = Convert.ToInt32(values[CATEGORIA_ID]);
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