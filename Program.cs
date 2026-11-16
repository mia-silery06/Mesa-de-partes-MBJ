using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
class Program
{


    static void Main(string[] args)
    {
        bool salir = false;
        while (!salir)
        {
            string opcion = mostrarMenuPrincipal();
            if (opcion == "1") 
            {Console.WriteLine("Modulo Iniciar Sesión, en creación");
            Console.ReadLine();}
            else if (opcion == "2") 
            {Console.WriteLine("Modulo Registrarse, en creación");
            Console.ReadLine();} // función de registrarCuenta();
            else if (opcion == "3")
            {
                Console.WriteLine("Gracias por usar la Mesa de Partes Digital. ¡Hasta luego!");
                Console.WriteLine("Presione enter para salir...");
                Console.ReadLine();
                salir = true;
            }
        }
    }
    static string mostrarMenuPrincipal()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Bienvenido a la Mesa de Partes Digital");
            Console.WriteLine("Seleccione una opción:");
            Console.WriteLine("1. Iniciar sesión");
            Console.WriteLine("2. Registrarse");
            Console.WriteLine("3. Salir");
            Console.Write("Ingrese su elección: ");
            string opcion = (Console.ReadLine() ?? "").Trim();

            if (opcion == "1" || opcion == "2" || opcion == "3") return opcion;

            Console.WriteLine("Opción inválida. Por favor ingrese una opción válida.");
            Console.WriteLine("Presione enter para volver al menú principal...");
            Console.ReadLine();
        }
    }
}