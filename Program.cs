using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static string archivoRuta = "usuarios.txt";  // Formato usuarios.txt : nombre,apellido,dni,hashClave
    static string rutaTramites = "tramites.txt"; // Formato tramites.txt : codigo,dni,tipo,detalle1,detalle2,fecha

    const int MAX_TRAMITES = 100;
    static string[] codigos = new string[MAX_TRAMITES];
    static string[] tipos = new string[MAX_TRAMITES];
    static string[] detalles1 = new string[MAX_TRAMITES];
    static string[] detalles2 = new string[MAX_TRAMITES];
    static string[] fechas = new string[MAX_TRAMITES];
    static int cantidad = 0;

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

    // MENÚ PRINCIPAL

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

    // REGISTRO

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

    // MENÚ DE USUARIO

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
                case "1": registrarTramite(dni); break;
                case "2": verTramites(dni); break;
                case "3": buscarTramites(dni); break;
                case "4": ordenarTramites(dni); break;
                case "5": cancelarTramite(dni); break;
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

    // REGISTRAR TRÁMITE 

    static void registrarTramite(string dni)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("==========================================================");
            Console.WriteLine("BIENVENIDO AL MÓDULO DE REGISTRO DE TRÁMITES");
            Console.WriteLine("==========================================================");
            Console.WriteLine("Elija el tipo de trámite que desea registrar:");
            Console.WriteLine("1. Demanda de Alimentos");
            Console.WriteLine("2. Rectificación de Partida");
            Console.WriteLine("3. Despido Arbitrario");
            Console.WriteLine("4. Volver al menú anterior");
            Console.Write("\nIngrese su elección: ");
            string eleccion = (Console.ReadLine() ?? "").Trim();

            if (eleccion == "1")
            {
                Console.Clear();
                Console.WriteLine("=== REGISTRO: DEMANDA DE ALIMENTOS ===");
                string demandado = pedirTexto("Ingrese el nombre completo del demandado: ", "El nombre del demandado", permitirNumeros: false, formatoNombre: true);
                decimal monto = pedirMonto();
                string montoTexto = monto.ToString("F2", CultureInfo.InvariantCulture);

                guardarTramite(dni, "Demanda de Alimentos", $"Demandado: {demandado}", $"Monto: S/. {montoTexto}");
                return;
            }
            else if (eleccion == "2")
            {
                Console.Clear();
                Console.WriteLine("=== REGISTRO: RECTIFICACIÓN DE PARTIDA ===");
                string tipoPartida = pedirTipoPartida();
                string error = pedirTexto("Ingrese la descripción del error a rectificar: ", "La descripción");

                guardarTramite(dni, "Rectificación de Partida", $"Partida: {tipoPartida}", $"Error: {error}");
                return;
            }
            else if (eleccion == "3")
            {
                Console.Clear();
                Console.WriteLine("=== REGISTRO: DESPIDO ARBITRARIO ===");
                string empresa = pedirTexto("Ingrese el nombre de la empresa empleadora: ", "El nombre de la empresa");
                string motivo = pedirTexto("Ingrese el motivo o detalle del despido: ", "El detalle del despido");

                guardarTramite(dni, "Despido Arbitrario", $"Empresa: {empresa}", $"Motivo: {motivo}");
                return;
            }
            else if (eleccion == "4")
            {
                return;
            }
            else
            {
                Console.WriteLine("\nOpción inválida. Intente de nuevo (presione enter).");
                Console.ReadLine();
            }
        }
    }

    static void guardarTramite(string dni, string tipo, string detalle1, string detalle2)
    {
        string codigo = "EXP-" + DateTime.Now.ToString("yyyyMMdd-HHmmssfff");
        string fecha = DateTime.Now.ToString("yyyy-MM-dd");

        using (StreamWriter escritor = new StreamWriter(rutaTramites, true))
        {
            escritor.WriteLine($"{codigo},{dni},{tipo},{detalle1},{detalle2},{fecha}");
        }

        Console.WriteLine("\n==========================================================");
        Console.WriteLine("¡TRÁMITE REGISTRADO CON ÉXITO!");
        Console.WriteLine($"Su Código de Expediente es: {codigo}");
        Console.WriteLine("Guarde este código para hacer seguimiento o búsquedas.");
        Console.WriteLine("==========================================================");
        Console.WriteLine("Presione enter para volver al menú...");
        Console.ReadLine();
    }

    //  ARREGLOS: CARGAR Y MOSTRAR 

    static void cargarTramites(string dni)
    {
        cantidad = 0;
        if (!File.Exists(rutaTramites)) return;

        string[] lineas = File.ReadAllLines(rutaTramites);
        for (int i = 0; i < lineas.Length; i++)
        {
            string[] p = lineas[i].Split(',');
            if (p.Length >= 6 && p[1] == dni && cantidad < MAX_TRAMITES)
            {
                codigos[cantidad] = p[0];
                tipos[cantidad] = p[2];
                detalles1[cantidad] = p[3];
                detalles2[cantidad] = p[4];
                fechas[cantidad] = p[5];
                cantidad++;
            }
        }
    }

    static void mostrarTramite(int i)
    {
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine($"Código : {codigos[i]}");
        Console.WriteLine($"Tipo   : {tipos[i]}");
        Console.WriteLine($"Datos  : {detalles1[i]} | {detalles2[i]}");
        Console.WriteLine($"Fecha  : {fechas[i]}");
    }

    static void mostrarLista()
    {
        if (cantidad == 0)
        {
            Console.WriteLine("No se encontraron trámites.");
            return;
        }

        for (int i = 0; i < cantidad; i++)
        {
            mostrarTramite(i);
        }
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine($"Total: {cantidad} trámite(s)");
    }

    // VER / BUSCAR / ORDENAR / CANCELAR 

    static void verTramites(string dni)
    {
        Console.Clear();
        Console.WriteLine("=== MIS TRÁMITES ===");
        cargarTramites(dni);
        mostrarLista();
        pausa();
    }

    static void buscarTramites(string dni)
    {
        Console.Clear();
        Console.WriteLine("=== BUSCAR TRÁMITE ===");
        string texto = pedirTexto("Ingrese el código de expediente o el tipo de trámite: ", "La búsqueda");

        cargarTramites(dni);

        int encontrados = 0;
        Console.WriteLine();
        for (int i = 0; i < cantidad; i++)
        {
            if (codigos[i].Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                tipos[i].Contains(texto, StringComparison.OrdinalIgnoreCase))
            {
                mostrarTramite(i);
                encontrados++;
            }
        }

        if (encontrados == 0)
            Console.WriteLine("No se encontraron trámites con esa búsqueda.");
        else
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine($"Coincidencias: {encontrados}");
        }
        pausa();
    }

    static void ordenarTramites(string dni)
    {
        Console.Clear();
        Console.WriteLine("=== ORDENAR TRÁMITES ===");
        Console.WriteLine("1. Por fecha (más recientes primero)");
        Console.WriteLine("2. Por fecha (más antiguos primero)");
        Console.WriteLine("3. Por tipo de trámite (A-Z)");
        Console.Write("\nSeleccione una opción: ");
        string opcion = (Console.ReadLine() ?? "").Trim();

        if (opcion != "1" && opcion != "2" && opcion != "3")
        {
            Console.WriteLine("\nOpción inválida. (presione enter)");
            Console.ReadLine();
            return;
        }

        cargarTramites(dni);
        ordenarBurbuja(int.Parse(opcion));

        Console.WriteLine();
        mostrarLista();
        pausa();
    }

    static void ordenarBurbuja(int criterio)
    {
        for (int i = 0; i < cantidad - 1; i++)
        {
            for (int j = 0; j < cantidad - 1 - i; j++)
            {
                if (debeIntercambiar(j, j + 1, criterio))
                {
                    intercambiar(j, j + 1);
                }
            }
        }
    }

    static bool debeIntercambiar(int a, int b, int criterio)
    {
        int comparacion;

        if (criterio == 3)
        {
            comparacion = string.Compare(tipos[a], tipos[b], StringComparison.OrdinalIgnoreCase);
            if (comparacion == 0)
                comparacion = string.Compare(codigos[a], codigos[b], StringComparison.Ordinal);
        }
        else
        {
            comparacion = string.Compare(codigos[a], codigos[b], StringComparison.Ordinal);
        }

        if (criterio == 1) return comparacion < 0;
        return comparacion > 0;
    }

    static void intercambiar(int a, int b)
    {
        string temp;

        temp = codigos[a]; codigos[a] = codigos[b]; codigos[b] = temp;
        temp = tipos[a]; tipos[a] = tipos[b]; tipos[b] = temp;
        temp = detalles1[a]; detalles1[a] = detalles1[b]; detalles1[b] = temp;
        temp = detalles2[a]; detalles2[a] = detalles2[b]; detalles2[b] = temp;
        temp = fechas[a]; fechas[a] = fechas[b]; fechas[b] = temp;
    }

    static int buscarPosicion(string codigo)
    {
        for (int i = 0; i < cantidad; i++)
        {
            if (codigos[i].Equals(codigo, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    static void cancelarTramite(string dni)
    {
        Console.Clear();
        Console.WriteLine("=== CANCELAR TRÁMITE ===");

        cargarTramites(dni);
        if (cantidad == 0)
        {
            Console.WriteLine("\nNo tiene trámites registrados.");
            pausa();
            return;
        }

        mostrarLista();
        string codigo = pedirTexto("\nIngrese el código del trámite a cancelar: ", "El código");

        int pos = buscarPosicion(codigo);
        if (pos == -1)
        {
            Console.WriteLine("\nNo se encontró un trámite suyo con ese código.");
            pausa();
            return;
        }

        Console.Write($"¿Seguro que desea cancelar {codigos[pos]} ({tipos[pos]})? (s/n): ");
        if ((Console.ReadLine() ?? "").Trim().ToLower() != "s")
        {
            Console.WriteLine("\nCancelación abortada.");
            pausa();
            return;
        }

        string[] lineas = File.ReadAllLines(rutaTramites);
        using (StreamWriter escritor = new StreamWriter(rutaTramites, false))
        {
            for (int i = 0; i < lineas.Length; i++)
            {
                string[] p = lineas[i].Split(',');
                bool esLaQueSeCancela = p.Length >= 2 && p[0] == codigos[pos] && p[1] == dni;
                if (!esLaQueSeCancela)
                {
                    escritor.WriteLine(lineas[i]);
                }
            }
        }

        Console.WriteLine("\nTrámite cancelado correctamente.");
        pausa();
    }

    static void pausa()
    {
        Console.WriteLine("\nPresione enter para volver al menú...");
        Console.ReadLine();
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

    static string validarClave(string clave, string dni)
    {
        if (clave.Length < 6) return "La contraseña debe tener al menos 6 caracteres.";
        if (tieneEspacios(clave)) return "La contraseña no puede contener espacios.";
        if (!tieneLetra(clave)) return "La contraseña debe tener al menos una letra.";
        if (!tieneDigito(clave)) return "La contraseña debe tener al menos un número.";
        if (clave == dni) return "La contraseña no puede ser igual a su DNI.";
        return "";
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

    static decimal pedirMonto()
    {
        const decimal minimo = 10.00m;
        const decimal maximo = 50000.00m;

        while (true)
        {
            Console.Write("Ingrese el monto mensual solicitado (S/.): ");
            string texto = (Console.ReadLine() ?? "").Trim().Replace(',', '.');

            if (string.IsNullOrWhiteSpace(texto))
                Console.WriteLine("El monto no puede estar vacío.");
            else if (!decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal monto))
                Console.WriteLine("Error: Debe ingresar un número válido.");
            else if (monto < minimo || monto > maximo)
                Console.WriteLine($"Error: El monto debe estar entre {minimo:F2} y {maximo:F2}.");
            else if (Math.Round(monto, 2) != monto)
                Console.WriteLine("Error: El monto no puede tener más de 2 decimales.");
            else
                return monto;

            Console.WriteLine("(presione enter para volver a intentarlo)");
            Console.ReadLine();
        }
    }

    static string pedirTipoPartida()
    {
        while (true)
        {
            Console.Write("Ingrese el tipo de partida (Nacimiento / Matrimonio / Defunción): ");
            string tipo = (Console.ReadLine() ?? "").Trim().ToLower();

            if (tipo == "nacimiento") return "Nacimiento";
            if (tipo == "matrimonio") return "Matrimonio";
            if (tipo == "defunción" || tipo == "defuncion") return "Defunción";

            Console.WriteLine("Tipo inválido. Debe ser Nacimiento, Matrimonio o Defunción. (presione enter)");
            Console.ReadLine();
        }
    }

    // SEGURIDAD 

    static string leerClaveOculta()
    {
        StringBuilder sb = new StringBuilder();
        while (true)
        {
            ConsoleKeyInfo tecla = Console.ReadKey(true);
            if (tecla.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return sb.ToString();
            }
            if (tecla.Key == ConsoleKey.Backspace)
            {
                if (sb.Length > 0)
                {
                    sb.Length--;
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(tecla.KeyChar))
            {
                sb.Append(tecla.KeyChar);
                Console.Write("*");
            }
        }
    }

    static string hashClave(string clave)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(clave));
        return Convert.ToHexString(bytes);
    }
}