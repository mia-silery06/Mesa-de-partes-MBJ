using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
class Program
{
    static string archivoRuta = "usuarios.txt";  // Formato usuarios.txt : nombre,apellido,dni,hashClave
    static void Main(string[] args)
    {
        bool salir = false;
        while (!salir)
        {
            string opcion = mostrarMenuPrincipal();
            if (opcion == "1") iniciarSesion();
            else if (opcion == "2") registrarCuenta();
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
    static void registrarCuenta()
    {
        Console.Clear();
        Console.WriteLine("=== REGISTRO DE NUEVA CUENTA ===");

        string nombre = pedirTexto("Ingrese su Nombre: ", "El nombre", permitirNumeros: false, formatoNombre: true);
        string apellido = pedirTexto("Ingrese su Apellido: ", "El apellido", permitirNumeros: false, formatoNombre: true);
        string dni = pedirDni();

        if (dniExiste(dni))
        {
            Console.WriteLine("\nYa existe una cuenta registrada con ese DNI.");
            Console.WriteLine("Presione enter para volver al menú principal...");
            Console.ReadLine();
            return;
        }

        string clave = pedirClaveNueva(dni);

        using (StreamWriter escritor = new StreamWriter(archivoRuta, true))
        {
            escritor.WriteLine($"{nombre},{apellido},{dni},{hashClave(clave)}");
        }

        Console.WriteLine("\n¡Registro guardado con éxito! (presione enter para volver al menú principal)");
        Console.ReadLine();
    }

    static bool dniExiste(string dni)
    {
        if (!File.Exists(archivoRuta)) return false;

        foreach (string linea in File.ReadAllLines(archivoRuta))
        {
            string[] partes = linea.Split(',');
            if (partes.Length >= 4 && partes[2] == dni) return true;
        }
        return false;
    }

    // INICIO DE SESIÓN
    static void iniciarSesion()
    {
        Console.Clear();
        Console.WriteLine("=== INICIAR SESIÓN ===");

        if (!File.Exists(archivoRuta))
        {
            Console.WriteLine("\nError: No hay cuentas registradas en el sistema todavía.");
            Console.WriteLine("Presione enter para volver al menú principal...");
            Console.ReadLine();
            return;
        }

        Console.Write("\nIngrese su DNI: ");
        string dniInput = (Console.ReadLine() ?? "").Trim();
        Console.Write("Ingrese su contraseña: ");
        string claveHash = hashClave(leerClaveOculta());

        foreach (string linea in File.ReadAllLines(archivoRuta))
        {
            string[] partes = linea.Split(',');
            if (partes.Length < 4) continue;

            if (dniInput == partes[2] && claveHash == partes[3])
            {
                menuUsuario(partes[2], partes[0], partes[1]);
                return;
            }
        }

        Console.WriteLine("\nError: DNI o contraseña incorrectos.");
        Console.WriteLine("Presione enter para volver al menú principal...");
        Console.ReadLine();
    }

     static void menuUsuario(string dni, string nombre, string apellido)
    {
        bool sesionActiva = true;
        while (sesionActiva)
        {
            Console.Clear();
            Console.WriteLine($"\n¡Bienvenido al sistema, {nombre} {apellido}!");
            Console.WriteLine($"DNI activo: {dni}");
            Console.WriteLine("==========================================================");
            Console.WriteLine("MENÚ DE LA MESA DE PARTES");
            Console.WriteLine("==========================================================");
            Console.WriteLine("1. Registrar nuevo trámite");
            Console.WriteLine("2. Ver mis trámites realizados");
            Console.WriteLine("3. Buscar trámite");
            Console.WriteLine("4. Ordenar trámites");
            Console.WriteLine("5. Cancelar trámite");
            Console.WriteLine("6. Cerrar sesión");
            Console.Write("\nSeleccione una opción: ");
            string opcion = (Console.ReadLine() ?? "").Trim();

            switch (opcion)
            {
                // En creación las funciones de cada opción, por ahora solo se cierra la sesión
                case "1": //registrarTramite
                case "2": //verTramites(dni) 
                case "3": //buscarTramites(dni)
                case "4": //ordenarTramites(dni)
                case "5": //cancelarTramite(dni)
                case "6":
                    Console.WriteLine("\nCerrando sesión... (presione enter)");
                    Console.ReadLine();
                    sesionActiva = false;
                    break;
                default:
                    Console.WriteLine("\nOpción inválida. Intente de nuevo (presione enter).");
                    Console.ReadLine();
                    break;
            }
        }
    }

    //  VALIDACIONES

    static bool tieneDigito(string texto)
    {
        for (int i = 0; i < texto.Length; i++)
        {
            if (char.IsDigit(texto[i])) return true;
        }
        return false;
    }

    static bool tieneLetra(string texto)
    {
        for (int i = 0; i < texto.Length; i++)
        {
            if (char.IsLetter(texto[i])) return true;
        }
        return false;
    }

    static bool tieneEspacios(string texto)
    {
        for (int i = 0; i < texto.Length; i++)
        {
            if (char.IsWhiteSpace(texto[i])) return true;
        }
        return false;
    }

    static bool soloDigitos(string texto)
    {
        for (int i = 0; i < texto.Length; i++)
        {
            if (!char.IsDigit(texto[i])) return false;
        }
        return true;
    }

    static bool soloLetrasYEspacios(string texto)
    {
        for (int i = 0; i < texto.Length; i++)
        {
            if (!char.IsLetter(texto[i]) && texto[i] != ' ') return false;
        }
        return true;
    }

    static string pedirTexto(string mensaje, string nombreCampo, bool permitirNumeros = true, bool formatoNombre = false)
    {
        while (true)
        {
            Console.Write(mensaje);
            string texto = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(texto))
                Console.WriteLine($"{nombreCampo} no puede estar vacío.");
            else if (texto.Contains(','))
                Console.WriteLine($"{nombreCampo} no puede contener comas.");
            else if (!permitirNumeros && tieneDigito(texto))
                Console.WriteLine($"{nombreCampo} no debe contener números.");
            else if (!permitirNumeros && !soloLetrasYEspacios(texto))
                Console.WriteLine($"{nombreCampo} solo debe contener letras (sin símbolos especiales).");
            else
            {
                if (formatoNombre)
                {
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                    texto = textInfo.ToTitleCase(texto.ToLower());
                }
                return texto;
            }

            Console.WriteLine("(presione enter para volver a intentarlo)");
            Console.ReadLine();
        }
    }
    static string pedirDni()
    {
        while (true)
        {
            Console.Write("Ingrese su DNI (8 dígitos): ");
            string dni = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(dni))
                Console.WriteLine("El DNI no puede estar vacío.");
            else if (!soloDigitos(dni))
                Console.WriteLine("El DNI debe contener solo números.");
            else if (dni.Length != 8)
                Console.WriteLine("El DNI debe tener 8 dígitos.");
            else
                return dni;

            Console.WriteLine("(presione enter para volver a intentarlo)");
            Console.ReadLine();
        }
    }

    //En progreso
    static string validarClave(string clave, string dni)
    {
        Console.WriteLine("en progreso");
        return "en progreso";

    }
    static string pedirClaveNueva(string dni)
    {
        while (true)
        {
            Console.Write("Ingrese su contraseña (mín. 6 caracteres, con letras y números, sin espacios): ");
            string clave = leerClaveOculta();

            string error = validarClave(clave, dni);
            if (error != "")
            {
                Console.WriteLine(error + " (presione enter para volver a intentarlo)");
                Console.ReadLine();
                continue;
            }

            Console.Write("Confirme su contraseña: ");
            if (leerClaveOculta() != clave)
            {
                Console.WriteLine("Las contraseñas no coinciden. (presione enter para volver a intentarlo)");
                Console.ReadLine();
                continue;
            }
            return clave;
        }
    }

    //En progreso
    static string leerClaveOculta()
    {
        Console.WriteLine("en progreso");
        return "en progreso";

    }

    static string hashClave(string clave)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(clave));
        return Convert.ToHexString(bytes);
    }
}

