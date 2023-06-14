
using Microsoft.EntityFrameworkCore;
using PruebaFontana_JuanHuamani.Data;
using PruebaFontana_JuanHuamani.Models;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");



async Task<List<Venta>> consultaDetalleDeVentas(int cant)
{
    using var dbVenta = new DbPruebaContext();
    return await dbVenta.Venta.Include(v => v.VentaDetalles)
        .Include("VentaDetalles.IdProductoNavigation")
        .ToListAsync();
}

//Traemos los datos base, 30 días
var datos = await consultaDetalleDeVentas(30);
Console.WriteLine("El total de ventas de los últimos 30 días (monto total y Q de ventas).");

var fecha = datos.Max(m => m.Fecha).AddDays(-30);
var Respt30Dias = datos.Where(m => m.Fecha >= fecha);
var consulta = from item in Respt30Dias
            select new { TotalVentas = item.Total, Quantity = item.VentaDetalles.Sum(m => m.Cantidad) };
Console.WriteLine($"Total de Ventas : {consulta.Sum(x => x.TotalVentas)} Y la cantidad ventas fue : {consulta.Sum(x => x.Quantity)}");
Console.WriteLine("--------------------------------------------------------------------------------------------------------------");


Console.WriteLine("El día y hora en que se realizó la venta con el monto más alto (y cuál es aquel monto).");
var ventaMasAlta = datos.Where(m => m.Total == datos.Max(m => m.Total)).Select(m => new { m.Total, m.Fecha }).FirstOrDefault();
Console.WriteLine($"Fecha de la ventas es : {ventaMasAlta.Fecha} y el monto mas alto  : {ventaMasAlta.Total}");
Console.WriteLine("--------------------------------------------------------------------------------------------------------------");


Console.WriteLine("Indicar cuál es el producto con mayor monto total de ventas.");
using var basededatos = new DbPruebaContext();

var ProductoMayorTotalVentas =
  (from p in basededatos.Productos
   let totalQuantity = (from op in basededatos.VentaDetalles
                        where op.IdProducto == p.IdProducto
                        select op.TotalLinea).Sum()
   where totalQuantity > 0
   orderby totalQuantity descending
   select p).Take(1);
Console.WriteLine($"Respuesta: El producto con mayor monto total en ventas es: {ProductoMayorTotalVentas.FirstOrDefault().Nombre}");
Console.WriteLine("--------------------------------------------------------------------------------------------------------------");



Console.WriteLine("Indicar el local con mayor monto de ventas.");
var localMayorMontoVentas =
  (from l in basededatos.Locals
   let totalQuantity = (from op in basededatos.Venta
                        join vd in basededatos.VentaDetalles on op.IdVenta equals vd.IdVenta
                        where op.IdLocal == l.IdLocal
                        select vd.TotalLinea).Sum()
   where totalQuantity > 0
   orderby totalQuantity descending
   select l).Take(1);
Console.WriteLine($"Respuesta El local con mayor monto de ventas: {localMayorMontoVentas.FirstOrDefault().Nombre}");
Console.WriteLine("--------------------------------------------------------------------------------------------------------------");



Console.WriteLine("¿Cuál es la marca con mayor margen de ganancias?");

var marcaMayorTotalVentas =
    (from m in basededatos.Marcas
     let totalQuantity = (from op in basededatos.VentaDetalles
                          join p in basededatos.Productos on op.IdProducto equals p.IdProducto
                          where p.IdMarca == m.IdMarca
                          select op.IdProducto).Count()
     where totalQuantity > 0
     orderby totalQuantity descending
     select m).Take(1);

Console.WriteLine($"Respuesta: La Marca con el mayor margen de ganancias es : {marcaMayorTotalVentas.FirstOrDefault().Nombre}");
Console.ReadLine();







