//#define LoadNewData
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
using MathHelpers;
using InMemoryDatabase;
using System.Globalization;
using SaucenaoApi;

namespace Test
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var saucenao = new Saucenao();

            var data = await saucenao.GetSauceAsync("https://media.discordapp.net/attachments/759124398325366784/927643925471178792/unknown.png");
        }
    }
}
