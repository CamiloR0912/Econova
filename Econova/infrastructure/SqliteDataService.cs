using Econova.Models;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Econova.Infrastructure
{
    public sealed class SqliteDataService
    {
        private static readonly Lazy<SqliteDataService> _instance =
            new Lazy<SqliteDataService>(() => new SqliteDataService());

        public static SqliteDataService Instance => _instance.Value;

        private readonly string _connectionString;

        private SqliteDataService()
        {
            Batteries_V2.Init();
            string databasePath = DatabaseFileProvider.GetDatabasePath();
            _connectionString = $"Data Source={databasePath};";
        }

        public void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                const string createClientes = @"
                CREATE TABLE IF NOT EXISTS Clientes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombres TEXT NOT NULL,
                    Apellidos TEXT NOT NULL,
                    Cedula TEXT NOT NULL UNIQUE,
                    Email TEXT,
                    Telefono TEXT,
                    Direccion TEXT
                );";

                const string createSalas = @"
                CREATE TABLE IF NOT EXISTS Salas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre TEXT NOT NULL UNIQUE,
                    Capacidad INTEGER NOT NULL
                );";

                const string createReservas = @"
                CREATE TABLE IF NOT EXISTS Reservas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SalaId INTEGER NOT NULL,
                    ClienteId INTEGER NOT NULL,
                    FechaEntrada TEXT NOT NULL,
                    FechaSalida TEXT NOT NULL,
                    FOREIGN KEY (SalaId) REFERENCES Salas(Id),
                    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
                    );";

                using (var cmd = new SqliteCommand(createClientes, connection))
                    cmd.ExecuteNonQuery();
                using (var cmd = new SqliteCommand(createSalas, connection))
                    cmd.ExecuteNonQuery();
                using (var cmd = new SqliteCommand(createReservas, connection))
                    cmd.ExecuteNonQuery();
            }
        }

        public bool AgregarCliente(Cliente cliente, out string error)
        {
            error = null;
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    const string sql = @"
                    INSERT INTO Clientes (Nombres, Apellidos, Cedula, Email, Telefono, Direccion)
                    VALUES (@Nombres, @Apellidos, @Cedula, @Email, @Telefono, @Direccion);";

                    using (var cmd = new SqliteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Nombres", cliente.Nombres);
                        cmd.Parameters.AddWithValue("@Apellidos", cliente.Apellidos);
                        cmd.Parameters.AddWithValue("@Cedula", cliente.Cedula);
                        cmd.Parameters.AddWithValue("@Email", cliente.Email ?? string.Empty);
                        cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono ?? string.Empty);
                        cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion ?? string.Empty);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SqliteException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public List<Cliente> ObtenerClientes()
        {
            var clientes = new List<Cliente>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                const string sql = @"SELECT Id, Nombres, Apellidos, Cedula, Email, Telefono, Direccion
                                     FROM Clientes ORDER BY Id DESC;";
                using (var cmd = new SqliteCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    int numero = 1;
                    while (reader.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Numero = numero++,
                            Nombres = Convert.ToString(reader["Nombres"]),
                            Apellidos = Convert.ToString(reader["Apellidos"]),
                            Cedula = Convert.ToString(reader["Cedula"]),
                            Email = Convert.ToString(reader["Email"]),
                            Telefono = Convert.ToString(reader["Telefono"]),
                            Direccion = Convert.ToString(reader["Direccion"])
                        });
                    }
                }
            }

            return clientes;
        }

        public bool EliminarCliente(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Primero eliminar las reservas del cliente
                        const string eliminarReservas = "DELETE FROM Reservas WHERE ClienteId = @Id;";
                        using (var cmd = new SqliteCommand(eliminarReservas, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }

                        // Luego eliminar el cliente
                        const string eliminarCliente = "DELETE FROM Clientes WHERE Id = @Id;";
                        using (var cmd = new SqliteCommand(eliminarCliente, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            var filas = cmd.ExecuteNonQuery();
                            transaction.Commit();
                            return filas > 0;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public Cliente BuscarClientePorCedula(string cedula)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                const string sql = @"SELECT Id, Nombres, Apellidos, Cedula, Email, Telefono, Direccion
                                     FROM Clientes
                                     WHERE Cedula = @Cedula
                                     LIMIT 1;";
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Cedula", cedula);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        return new Cliente
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombres = Convert.ToString(reader["Nombres"]),
                            Apellidos = Convert.ToString(reader["Apellidos"]),
                            Cedula = Convert.ToString(reader["Cedula"]),
                            Email = Convert.ToString(reader["Email"]),
                            Telefono = Convert.ToString(reader["Telefono"]),
                            Direccion = Convert.ToString(reader["Direccion"])
                        };
                    }
                }
            }
        }

        public List<string> ObtenerCedulasPorPrefijo(string prefijo, int limite = 8)
        {
            var cedulas = new List<string>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                const string sql = @"SELECT Cedula
                                     FROM Clientes
                                     WHERE Cedula LIKE @Prefijo
                                     ORDER BY Cedula
                                     LIMIT @Limite;";
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Prefijo", prefijo + "%");
                    cmd.Parameters.AddWithValue("@Limite", limite);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            cedulas.Add(Convert.ToString(reader["Cedula"]));
                    }
                }
            }
            return cedulas;
        }

        public bool AgregarSala(string nombre, int capacidad, out string error)
        {
            error = null;
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    const string sql = "INSERT INTO Salas (Nombre, Capacidad) VALUES (@Nombre, @Capacidad);";
                    using (var cmd = new SqliteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@Capacidad", capacidad);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SqliteException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public List<Sala> ObtenerSalas()
        {
            var salas = new List<Sala>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                const string sql = "SELECT Id, Nombre, Capacidad FROM Salas ORDER BY Id DESC;";
                using (var cmd = new SqliteCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    int numero = 1;
                    while (reader.Read())
                    {
                        salas.Add(new Sala
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Numero = numero++,
                            Nombre = Convert.ToString(reader["Nombre"]),
                            Capacidad = Convert.ToInt32(reader["Capacidad"])
                        });
                    }
                }
            }
            return salas;
        }

        public bool EliminarSala(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Primero eliminar las reservas que usan esta sala
                        const string eliminarReservas = "DELETE FROM Reservas WHERE SalaId = @Id;";
                        using (var cmd = new SqliteCommand(eliminarReservas, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }

                        // Luego eliminar la sala
                        const string eliminarSala = "DELETE FROM Salas WHERE Id = @Id;";
                        using (var cmd = new SqliteCommand(eliminarSala, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            var filas = cmd.ExecuteNonQuery();
                            transaction.Commit();
                            return filas > 0;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool AgregarReserva(int salaId, int clienteId, DateTime fechaEntrada, DateTime fechaSalida, out string error)
        {
            error = null;
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    const string sql = @"
                    INSERT INTO Reservas (SalaId, ClienteId, FechaEntrada, FechaSalida)
                    VALUES (@SalaId, @ClienteId, @FechaEntrada, @FechaSalida);";

                    using (var cmd = new SqliteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@SalaId", salaId);
                        cmd.Parameters.AddWithValue("@ClienteId", clienteId);
                        cmd.Parameters.AddWithValue("@FechaEntrada", fechaEntrada.ToString("o", CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@FechaSalida", fechaSalida.ToString("o", CultureInfo.InvariantCulture));
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SqliteException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public List<Reserva> ObtenerReservas()
        {
            var reservas = new List<Reserva>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                const string sql = @"
                SELECT r.Id,
                       s.Nombre AS Sala,
                       c.Nombres || ' ' || c.Apellidos AS Cliente,
                       c.Cedula AS Cedula,
                       r.FechaEntrada,
                       r.FechaSalida
                FROM Reservas r
                INNER JOIN Salas s ON s.Id = r.SalaId
                INNER JOIN Clientes c ON c.Id = r.ClienteId
                ORDER BY r.FechaEntrada DESC;";

                using (var cmd = new SqliteCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    int numero = 1;
                    while (reader.Read())
                    {
                        var fechaEntrada = DateTime.Parse(Convert.ToString(reader["FechaEntrada"]), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        var fechaSalida = DateTime.Parse(Convert.ToString(reader["FechaSalida"]), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                        reservas.Add(new Reserva
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Numero = numero++,
                            Sala = Convert.ToString(reader["Sala"]),
                            Cliente = Convert.ToString(reader["Cliente"]),
                            Cedula = Convert.ToString(reader["Cedula"]),
                            FechaEntradaDt = fechaEntrada,
                            FechaSalidaDt = fechaSalida,
                            FechaEntrada = fechaEntrada.ToString("dd/MM/yyyy"),
                            HoraEntrada = fechaEntrada.ToString("hh:mm tt"),
                            FechaSalida = fechaSalida.ToString("dd/MM/yyyy"),
                            HoraSalida = fechaSalida.ToString("hh:mm tt")
                        });
                    }
                }
            }
            return reservas;
        }

        public bool EliminarReserva(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                const string sql = "DELETE FROM Reservas WHERE Id = @Id;";
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ActualizarCliente(Cliente cliente, out string error)
        {
            error = null;
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    const string sql = @"
                    UPDATE Clientes 
                    SET Nombres = @Nombres,
                        Apellidos = @Apellidos,
                        Cedula = @Cedula,
                        Telefono = @Telefono,
                        Email = @Email,
                        Direccion = @Direccion
                    WHERE Id = @Id;";
                    using (var cmd = new SqliteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Nombres", cliente.Nombres);
                        cmd.Parameters.AddWithValue("@Apellidos", cliente.Apellidos);
                        cmd.Parameters.AddWithValue("@Cedula", cliente.Cedula);
                        cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono ?? string.Empty);
                        cmd.Parameters.AddWithValue("@Email", cliente.Email ?? string.Empty);
                        cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion ?? string.Empty);
                        cmd.Parameters.AddWithValue("@Id", cliente.Id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqliteException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        // RF-02: Verifica si ya existe otro cliente con la misma cédula (excluyendo el cliente actual)
        public bool ExisteCedulaParaOtroCliente(string cedula, int clienteIdExcluir)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                const string sql = @"SELECT COUNT(*) FROM Clientes 
                                     WHERE Cedula = @Cedula AND Id != @Id;";
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Cedula", cedula);
                    cmd.Parameters.AddWithValue("@Id", clienteIdExcluir);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        // RF-03/RF-04: Verifica si existe traslape de horarios para una sala
        public bool ExisteTraslapeReserva(int salaId, DateTime fechaEntrada, DateTime fechaSalida, int? reservaIdExcluir = null)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string sql = @"
                    SELECT COUNT(*) FROM Reservas
                    WHERE SalaId = @SalaId
                      AND FechaEntrada < @FechaSalida
                      AND FechaSalida > @FechaEntrada";

                if (reservaIdExcluir.HasValue)
                    sql += " AND Id != @ExcluirId";

                sql += ";";

                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@SalaId", salaId);
                    cmd.Parameters.AddWithValue("@FechaEntrada", fechaEntrada.ToString("o", CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@FechaSalida", fechaSalida.ToString("o", CultureInfo.InvariantCulture));
                    if (reservaIdExcluir.HasValue)
                        cmd.Parameters.AddWithValue("@ExcluirId", reservaIdExcluir.Value);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        // RF-04: Actualizar reserva existente en BD
        public bool ActualizarReserva(int reservaId, int salaId, int clienteId, DateTime fechaEntrada, DateTime fechaSalida, out string error)
        {
            error = null;
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    const string sql = @"
                    UPDATE Reservas 
                    SET SalaId = @SalaId,
                        ClienteId = @ClienteId,
                        FechaEntrada = @FechaEntrada,
                        FechaSalida = @FechaSalida
                    WHERE Id = @Id;";
                    using (var cmd = new SqliteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@SalaId", salaId);
                        cmd.Parameters.AddWithValue("@ClienteId", clienteId);
                        cmd.Parameters.AddWithValue("@FechaEntrada", fechaEntrada.ToString("o", CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@FechaSalida", fechaSalida.ToString("o", CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@Id", reservaId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqliteException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        // RF-06: Actualizar sala existente
        public bool ActualizarSala(int id, string nombre, int capacidad, out string error)
        {
            error = null;
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    const string sql = @"UPDATE Salas SET Nombre = @Nombre, Capacidad = @Capacidad WHERE Id = @Id;";
                    using (var cmd = new SqliteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@Capacidad", capacidad);
                        cmd.Parameters.AddWithValue("@Id", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqliteException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        // RF-07: Obtener reservas activas de una sala específica (futuras o en curso)
        public List<Reserva> ObtenerReservasActivasPorSala(int salaId)
        {
            var reservas = new List<Reserva>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string ahora = DateTime.Now.ToString("o", CultureInfo.InvariantCulture);
                const string sql = @"
                SELECT r.Id,
                       s.Nombre AS Sala,
                       c.Nombres || ' ' || c.Apellidos AS Cliente,
                       c.Cedula AS Cedula,
                       r.FechaEntrada,
                       r.FechaSalida
                FROM Reservas r
                INNER JOIN Salas s ON s.Id = r.SalaId
                INNER JOIN Clientes c ON c.Id = r.ClienteId
                WHERE r.SalaId = @SalaId
                  AND r.FechaSalida > @Ahora
                ORDER BY r.FechaEntrada ASC;";

                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@SalaId", salaId);
                    cmd.Parameters.AddWithValue("@Ahora", ahora);
                    using (var reader = cmd.ExecuteReader())
                    {
                        int numero = 1;
                        while (reader.Read())
                        {
                            var fechaEntrada = DateTime.Parse(Convert.ToString(reader["FechaEntrada"]), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var fechaSalida = DateTime.Parse(Convert.ToString(reader["FechaSalida"]), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                            reservas.Add(new Reserva
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Numero = numero++,
                                Sala = Convert.ToString(reader["Sala"]),
                                Cliente = Convert.ToString(reader["Cliente"]),
                                Cedula = Convert.ToString(reader["Cedula"]),
                                FechaEntradaDt = fechaEntrada,
                                FechaSalidaDt = fechaSalida,
                                FechaEntrada = fechaEntrada.ToString("dd/MM/yyyy"),
                                HoraEntrada = fechaEntrada.ToString("hh:mm tt"),
                                FechaSalida = fechaSalida.ToString("dd/MM/yyyy"),
                                HoraSalida = fechaSalida.ToString("hh:mm tt")
                            });
                        }
                    }
                }
            }
            return reservas;
        }

        // RF-07: Verificar si una sala está ocupada en este momento
        public bool SalaOcupadaAhora(int salaId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string ahora = DateTime.Now.ToString("o", CultureInfo.InvariantCulture);
                const string sql = @"
                    SELECT COUNT(*) FROM Reservas
                    WHERE SalaId = @SalaId
                      AND FechaEntrada <= @Ahora
                      AND FechaSalida > @Ahora;";
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@SalaId", salaId);
                    cmd.Parameters.AddWithValue("@Ahora", ahora);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        // RF-08: Obtener historial de visitas (reservas pasadas)
        public List<Reserva> ObtenerHistorialVisitas()
        {
            var reservas = new List<Reserva>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string ahora = DateTime.Now.ToString("o", CultureInfo.InvariantCulture);
                const string sql = @"
                SELECT r.Id,
                       s.Nombre AS Sala,
                       c.Nombres || ' ' || c.Apellidos AS Cliente,
                       c.Cedula AS Cedula,
                       r.FechaEntrada,
                       r.FechaSalida
                FROM Reservas r
                INNER JOIN Salas s ON s.Id = r.SalaId
                INNER JOIN Clientes c ON c.Id = r.ClienteId
                WHERE r.FechaSalida <= @Ahora
                ORDER BY r.FechaEntrada DESC;";

                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Ahora", ahora);
                    using (var reader = cmd.ExecuteReader())
                    {
                        int numero = 1;
                        while (reader.Read())
                        {
                            var fechaEntrada = DateTime.Parse(Convert.ToString(reader["FechaEntrada"]), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            var fechaSalida = DateTime.Parse(Convert.ToString(reader["FechaSalida"]), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                            reservas.Add(new Reserva
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Numero = numero++,
                                Sala = Convert.ToString(reader["Sala"]),
                                Cliente = Convert.ToString(reader["Cliente"]),
                                Cedula = Convert.ToString(reader["Cedula"]),
                                FechaEntradaDt = fechaEntrada,
                                FechaSalidaDt = fechaSalida,
                                FechaEntrada = fechaEntrada.ToString("dd/MM/yyyy"),
                                HoraEntrada = fechaEntrada.ToString("hh:mm tt"),
                                FechaSalida = fechaSalida.ToString("dd/MM/yyyy"),
                                HoraSalida = fechaSalida.ToString("hh:mm tt")
                            });
                        }
                    }
                }
            }
            return reservas;
        }

        // RF-04: Obtener datos internos de una reserva por Id (SalaId, ClienteId)
        public (int SalaId, int ClienteId) ObtenerIdsReserva(int reservaId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                const string sql = "SELECT SalaId, ClienteId FROM Reservas WHERE Id = @Id;";
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", reservaId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return (Convert.ToInt32(reader["SalaId"]), Convert.ToInt32(reader["ClienteId"]));
                    }
                }
            }
            return (0, 0);
        }
    }
}
