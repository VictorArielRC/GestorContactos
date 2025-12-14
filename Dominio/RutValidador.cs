using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Dominio
{
    public static class RutValidador
    {
        public static bool EsValido(string rut)
        {
            if (string.IsNullOrEmpty(rut)) return false;

            rut = rut.Replace(".", "").Replace("-", "").ToUpper();
            if (!Regex.IsMatch(rut, @"^\d{7,8}[0-9K]$"))
                return false;
            string cuerpo = rut[..^1];
            char dv = rut[^1];

            int suma = 0;
            int multiplicador = 2;

            for (int i = cuerpo.Length - 1; i >= 0; i--)
            {
                suma += (cuerpo[i] - '0') * multiplicador;
                multiplicador = multiplicador == 7 ? 2 : multiplicador + 1;
            }

            int resto = 11 - (suma % 11);
            char dvCalculado = resto switch
            {
                11 => '0',
                10 => 'K',
                _ => (char)(resto + '0')
            };

            return dv == dvCalculado;
        }
    }
}
