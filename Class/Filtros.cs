using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LA_VENDUTA.Class
{
    public class Filtros
    {
        public int Filtro1 { get; set; }
        public int Filtro2 { get; set; }
         
        public int Filtro3 { get; set; }

        public Filtros(int filtro1, int filtro2,int filtro3)
        {
            Filtro1 = filtro1;
            Filtro2 = filtro2;
            Filtro3 = filtro3;
        }
    }
}