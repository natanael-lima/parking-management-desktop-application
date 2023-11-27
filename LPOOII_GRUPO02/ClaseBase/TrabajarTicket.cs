﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;
namespace ClaseBase
{
    public class TrabajarTicket
    {
        public static Ticket altaTicket(Ticket ticket)
        {
            using (SqlConnection cn = new SqlConnection(ClaseBase.Properties.Settings.Default.playaConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Ticket(cli_Id, tv_Id, sec_Id, t_Duracion,t_FechaHoraEnt, t_Patente, t_Tarifa,t_Total) values (@cli_Id,@tv_Id,@sec_Id,@t_Duracion,@t_FechaHoraEntrada, @t_Patente, @t_Tarifa,@t_Total); SELECT SCOPE_IDENTITY();", cn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@cli_Id", ticket.Cli_Id.Cli_Id);
                    cmd.Parameters.AddWithValue("@tv_Id", ticket.Tv_Id.Tv_Id);
                    cmd.Parameters.AddWithValue("@sec_Id", ticket.Sec_Id.Sec_Codigo);
                    cmd.Parameters.AddWithValue("@t_Duracion", ticket.T_Duracion);
                    cmd.Parameters.AddWithValue("@t_FechaHoraEntrada", ticket.T_FechaHoraEnt);
                    cmd.Parameters.AddWithValue("@t_Patente", ticket.T_Patente);
                    cmd.Parameters.AddWithValue("@t_Tarifa", ticket.T_Tarifa);
                    cmd.Parameters.AddWithValue("@t_Total", ticket.T_Total);

                    // Ejecutar la inserción y recuperar el ID generado
                    int idGenerado = Convert.ToInt32(cmd.ExecuteScalar());

                    // Asignar el ID generado al objeto Ticket antes de retornarlo
                    ticket.T_Id = idGenerado;
                }
            }

            // Retornar el objeto Ticket con el ID generado
            return ticket;
        }
        public static Ticket traerTicket(Sector sector)
        {
            Ticket ticket = null; 
            using (SqlConnection cn = new SqlConnection(ClaseBase.Properties.Settings.Default.playaConnectionString))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT TOP 1 Ticket.* FROM Ticket INNER JOIN Sector ON Ticket.sec_Id = "+ sector.Sec_Codigo + " WHERE Sector.sec_Identificador = '" + sector.Sec_Identificador + "' ORDER BY t_Id DESC" ;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ticket = new Ticket();
                            // Mapear las columnas según la estructura de tu tabla Sector
                            ticket.T_Id = Convert.ToInt32(reader["t_Id"]);
                            Cliente cli = new Cliente();
                            cli.Cli_Id = Convert.ToInt32(reader["cli_Id"]);
                            foreach (Cliente clii in TrabajarClientes.traer_clientes())
                            {
                                if (cli.Cli_Id == clii.Cli_Id)
                                {
                                    cli = clii;
                                    break;
                                }
                            }
                            ticket.Cli_Id = cli;
                            TipoVehiculo tp = new TipoVehiculo();
                            tp.Tv_Id = Convert.ToInt32(reader["tv_Id"]);
                            foreach (TipoVehiculo tpp in TrabajarTipoVehiculos.traer_tipos_vehiculos())
                            {
                                if (tp.Tv_Id == tpp.Tv_Id)
                                {
                                    tp = tpp;
                                    break;
                                }
                            }
                            ticket.Tv_Id = tp;
                            Sector sec = new Sector();
                            sec.Sec_Codigo = Convert.ToInt32(reader["sec_Id"]);
                            foreach (Sector secc in TrabajarSector.traerSectores())
                            {
                                if (sec.Sec_Codigo==secc.Sec_Codigo)
                                {
                                    sec = secc;
                                    break;
                                }
                            }
                            ticket.Sec_Id = sec;  
                            ticket.T_Duracion = Convert.ToDouble(reader["t_Duracion"]);
                            ticket.T_FechaHoraEnt = Convert.ToDateTime(reader["t_FechaHoraEnt"]);
                            ticket.T_Patente = Convert.ToString(reader["t_Patente"]);
                                ticket.T_Tarifa = Convert.ToDecimal(reader["t_Tarifa"]);
                            ticket.T_Total = Convert.ToDecimal(reader["t_Total"]);
                            // Otros atributos

                        }
                    }
                }
            }
            return ticket;
        }
        public static void updateTiket(Ticket tiket)
        {
            using (SqlConnection cn = new SqlConnection(ClaseBase.Properties.Settings.Default.playaConnectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE Ticket SET t_FechaHoraSal = @fecha WHERE t_Id = @id", cn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@fecha", tiket.T_FechaHoraSal);
                    cmd.Parameters.AddWithValue("@id", tiket.T_Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        

    }
}
