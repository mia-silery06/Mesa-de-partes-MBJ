using System;
using System.IO;

class Program
{
    static string archivoRuta = "usuarios.txt";

    static void Main(string[] args)
    {
        bool salir = false;
        while (!salir)
        {
            Console.Clear();
            Console.WriteLine("==========================================================");
            Console.WriteLine("Bienvenido a la mesa de partes del Módulo básico de SJL");
            Console.WriteLine("==========================================================");
            Console.WriteLine("1. Iniciar sesión");
            Console.WriteLine("2. Registrarse (Crear Cuenta)");
            Console.WriteLine("3. Salir del programa");
            Console.Write("Seleccione una opción marcando un número: ");
            string opcion = Console.ReadLine()!;

            if (opcion == "1") IniciarSesion();
            else if (opcion == "2") RegistrarCuenta();
            else if (opcion == "3") { Console.WriteLine("\n¡Hasta luego!"); salir = true; }
            else { Console.WriteLine("\nOpción no válida."); Console.ReadLine(); }
        }
    }

    static void RegistrarCuenta()
    {
        Console.Clear();
        Console.WriteLine("=== CREAR CUENTA NUEVA ===");
        Console.Write("Ingrese su nuevo nombre de usuario: ");
        string nuevoUsuario = Console.ReadLine()!;
        Console.Write("Ingrese su nueva contraseña: ");
        string nuevaClave = Console.ReadLine()!;

        if (string.IsNullOrWhiteSpace(nuevoUsuario) || string.IsNullOrWhiteSpace(nuevaClave))
        {
            Console.WriteLine("\nError: El usuario o la contraseña no pueden estar vacíos.");
            Console.ReadLine();
            return;
        }

        using (StreamWriter sw = new StreamWriter(archivoRuta, true))
        {
            sw.WriteLine($"{nuevoUsuario},{nuevaClave}");
        }
        Console.WriteLine($"\n¡Usuario '{nuevoUsuario}' guardado permanentemente en el archivo!");
        Console.ReadLine();
    }

    static void IniciarSesion()
    {
        Console.Clear();
        Console.WriteLine("=== INICIAR SESIÓN ===");
        Console.Write("Usuario: ");
        string usuarioInput = Console.ReadLine()!;
        Console.Write("Contraseña: ");
        string claveInput = Console.ReadLine()!;

        if (!File.Exists(archivoRuta))
        {
            Console.WriteLine("\nError: No hay cuentas registradas en el sistema todavía.");
            Console.ReadLine();
            return;
        }

        string[] lineas = File.ReadAllLines(archivoRuta);
        bool loginExitoso = false;

        foreach (string linea in lineas)
        {
            string[] partes = linea.Split(',');
            string usuarioGuardado = partes[0];
            string claveGuardada = partes[1];

            if (usuarioInput == usuarioGuardado && claveInput == claveGuardada)
            {
                loginExitoso = true;
                break;
            }
        }

        if (loginExitoso)
        {
            Console.WriteLine($"\n¡Bienvenido, {usuarioInput}!");
            Console.WriteLine("==========================================================");
            Console.WriteLine("MENÚ DE LA MESA DE PARTES (Opción 1 Activa)");
            Console.WriteLine("==========================================================");
            Console.WriteLine("1. Registrar nuevo expediente (Activo)");
            Console.WriteLine("2. Consultar expediente (Próximamente)");
            Console.WriteLine("3. Ordenar expedientes (Próximamente)");
            Console.WriteLine("\nPresione ENTER para cerrar sesión.");
            Console.ReadLine();
        }
        else
        {
            Console.WriteLine("\nError: Usuario o contraseña incorrectos.");
            Console.ReadLine();
        }
    }
}