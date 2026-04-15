using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ConsoleApp1
{
    class Program
    {
        static string cadenaConexion = "Server=(localdb)\\MSSQLLocalDB; Database=ControlAlumnoSuCarnet; Integrated Security=True;";
    
    static void Main(string[] args)
        {
            // --- ESTO ES EL MENÚ QUE VES EN LA PANTALLA NEGRA ---
            Console.WriteLine("--- SISTEMA DE CONTROL DE ALUMNOS ---");
            Console.WriteLine("1. Insertar nuevo alumno");
            Console.WriteLine("2. Modificar alumno existente");
            Console.WriteLine("3. Buscar un alumno");
            Console.Write("Elige una opción: ");

            // Guardamos lo que el usuario escribió en el teclado
            string opcion = Console.ReadLine();

            // Dependiendo de lo que elijas (1, 2 o 3), el código salta a la función correcta
            switch (opcion)
            {
                case "1":
                    Console.Write("Escribe el Carnet (ej. CD121515): "); string carnet = Console.ReadLine();
                    Console.Write("Escribe el Nombre Completo: "); string nombre = Console.ReadLine();
                    Insertar(carnet, nombre); // Llama al bloque de "Insertar"
                    break;
                case "2":
                    Console.Write("Carnet del alumno a modificar: "); string cMod = Console.ReadLine();
                    Console.Write("Nuevo Nombre Completo: "); string nMod = Console.ReadLine();
                    Modificar(cMod, nMod); // Llama al bloque de "Modificar"
                    break;
                case "3":
                    Console.Write("Carnet a buscar (ej. CD121515): "); string cBus = Console.ReadLine();
                    Buscar(cBus); // Llama al bloque de "Buscar"
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }

            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey(); // Pausa para que no se cierre la ventana
        }

        // --- FUNCIÓN PARA GUARDAR UN ALUMNO NUEVO ---
        static void Insertar(string carnet, string nombre)
        {
            // 'using' asegura que la puerta del almacén se cierre al terminar, aunque haya un error
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                // La orden SQL que le daremos al almacén (INSERT = Meter datos)
                string query = "INSERT INTO Alumno (Carnet, NombreCompleto) VALUES (@carnet, @nombre)";

                // Preparamos el "pedido" con la orden y la llave de la puerta
                SqlCommand comando = new SqlCommand(query, conexion);

                // Ponemos etiquetas seguras a los datos para que nadie hackee la base de datos
                comando.Parameters.AddWithValue("@carnet", carnet);
                comando.Parameters.AddWithValue("@nombre", nombre);

                conexion.Open(); // ¡Abrimos la puerta!
                comando.ExecuteNonQuery(); // Empujamos el pedido hacia la base de datos
                Console.WriteLine("¡Alumno guardado con éxito!");
            }
        }

        // --- FUNCIÓN PARA CAMBIAR EL NOMBRE DE ALGUIEN ---
        static void Modificar(string carnet, string nuevoNombre)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                // UPDATE = Cambiar algo que ya existe
                string query = "UPDATE Alumno SET NombreCompleto = @nombre WHERE Carnet = @carnet";

                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@nombre", nuevoNombre);
                comando.Parameters.AddWithValue("@carnet", carnet);

                conexion.Open();
                // ExecuteNonQuery devuelve un número: cuántas filas cambió
                int filas = comando.ExecuteNonQuery();

                if (filas > 0) Console.WriteLine("¡Datos actualizados!");
                else Console.WriteLine("No se encontró ese carnet.");
            }
        }

        // --- FUNCIÓN PARA BUSCAR A UN ALUMNO ESPECÍFICO ---
        static void Buscar(string carnet)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                // SELECT = Traer o mirar datos
                string query = "SELECT NombreCompleto FROM Alumno WHERE Carnet = @carnet";

                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@carnet", carnet);

                conexion.Open();

                // SqlDataReader es como una "bolsa" donde el carrito trae lo que encontró
                SqlDataReader lector = comando.ExecuteReader();

                // .Read() intenta sacar algo de la bolsa. Si hay algo, devuelve TRUE
                if (lector.Read())
                {
                    // Le pedimos al lector que nos diga qué hay en la columna "NombreCompleto"
                    Console.WriteLine("Alumno encontrado: " + lector["NombreCompleto"]);
                }
                else
                {
                    Console.WriteLine("No existe un alumno con ese carnet.");
                }
            }
        }
    }
}
