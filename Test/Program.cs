using CsvToObjects;
using ReflectionExtensions;
using MoeBooruApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            //var p = new MoeBooruRequester();

            //var request = new MoeBooruAPIRequest()
            //{
            //    MaxResults = 1000,
            //    Random = true,
            //    Rating = MoeBooruRating.Any,
            //    Tags = new List<string>()
            //    {
            //        "catgirl"
            //    }
            //};

            //var cachedRequest = request.GetRequest();

            //var response = await p.GetResponse(cachedRequest);

            //var type = typeof(bool?);

            //object p = type.ConvertToCompatibleType("test");

            var listUsuarios = new List<Usuario>()
            {
                new Usuario()
                {
                    NombrePeroRaroParaEnseñarElEjemplo = "Pepito",
                    Edad = 18,
                    DatoInutil = "asdasdasd"
                },
                new Usuario()
                {
                    NombrePeroRaroParaEnseñarElEjemplo = "Juan",
                    Edad = 20,
                    DatoInutil = "fwefwefwef"
                },
            };

            Migrate(listUsuarios, item => new MigrateQuery()
            {
                UserName = item.NombrePeroRaroParaEnseñarElEjemplo,
                Edad = item.Edad,
            });
        }

        public static void Migrate<T>(List<T> usuarios, Func<T, MigrateQuery> mapeo)
        {
            foreach (var item in usuarios)
            {
                var usuarioMigracion = mapeo(item);

                Console.WriteLine($"Migrado correctamente el usuario {usuarioMigracion.UserName} {usuarioMigracion.Edad}");
            }

        }

        public class Usuario
        {
            public string NombrePeroRaroParaEnseñarElEjemplo { get; set; }
            public int Edad { get; set; }
            public string DatoInutil { get; set; }
        }

        public class MigrateQuery
        {
            public string UserName { get; set; }
            public int Edad { get; set; }
        }
    }
}
