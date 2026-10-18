using System;

class Program
{
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

            if (opcion == "1")
            {
                Console.WriteLine("\n[Próximamente] Módulo de Inicio de Sesión. Presione ENTER.");
                Console.ReadLine();
            }
            else if (opcion == "2")
            {
                Console.WriteLine("\n[Próximamente] Módulo de Registro de Cuenta. Presione ENTER.");
                Console.ReadLine();
            }
            else if (opcion == "3")
            {
                Console.WriteLine("\nGracias por usar el sistema. ¡Hasta luego!");
                salir = true;
            }
            else
            {
                Console.WriteLine("\nOpción no válida. Presione ENTER e intente nuevamente.");
                Console.ReadLine();
            }
        }
    }
}

