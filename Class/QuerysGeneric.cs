using LA_VENDUTA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LA_VENDUTA.Class
{
    public static class QuerysGeneric
    {
        public static Random random = new Random();
        public static IEnumerable<tipe> Get_N_Obj_Ordenado_Por<tipe, tipeOfOrder>(IQueryable<tipe> conjunto, int? n, Func<tipe, int> expression, bool descending = false)
        {

            if (descending)
            {
                if (n == null)
                {
                    return conjunto.OrderBy(expression).Reverse();
                }
                else
                {
                    return conjunto.OrderBy(expression).Reverse().Take((int)n);
                }
            }
            else
            {
                if (n == null)
                {
                    return conjunto.OrderBy(expression);
                }
                else
                {
                    return conjunto.OrderBy(expression).Take((int)n);
                }
            }
        }
        public static IEnumerable<tipe> Get_N_Obj_Mientras<tipe>(IQueryable<tipe> conjunto, int? n, Func<tipe, bool> expression)
        {
            if (n == null)
            {
                return conjunto.Where(expression);
            }
            else
            {
                return conjunto.Where(expression).Take((int)n);
            }
        }
        public static Vendedor GetVendedor(string UserId, IQueryable<Vendedor> conjunto)
        {
            var vendedores = Get_N_Obj_Mientras(conjunto, 1, m => m.User == UserId);
            if (vendedores == null || vendedores.Count() == 0) { return null; }
            else return vendedores.ToList()[0];
        }

        public static Comprador GetComprador(string UserId, IQueryable<Comprador> conjunto)
        {
            var vendedores = Get_N_Obj_Mientras(conjunto, 1, m => m.User == UserId);
            if (vendedores == null || vendedores.Count() == 0) { return null; }
            else return vendedores.ToList()[0];
        }
        public static IEnumerable<TarjetaDeCredito> GetTarjetas(IEnumerable<UsuarioConTarjeta> usuarioConTarjetas)
        {
            foreach (var tarjetas in usuarioConTarjetas)
            {
                yield return tarjetas.TarjetadeCredito;
            }
        }
    }
}