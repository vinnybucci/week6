using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using CampgroundReservations.Models;

namespace CampgroundReservations.DAO
{
    public class ReservationSqlDAO : IReservationDAO
    {
        private string connectionString;

        public ReservationSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public int CreateReservation(int siteId, string name, DateTime fromDate, DateTime toDate)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into reservation (site_id, name, from_date, to_date) VALUES (@site_id, @name, @from_date, @to_date); Select scope_identity(); ", connection);
                    sqlCommand.Parameters.AddWithValue("@site_id", siteId);
                    sqlCommand.Parameters.AddWithValue("@name", name);
                    sqlCommand.Parameters.AddWithValue("@from_date", fromDate);
                    sqlCommand.Parameters.AddWithValue("@to_date", toDate);

                    int createdReservation = Convert.ToInt32(sqlCommand.ExecuteScalar());
                    return createdReservation;
                }
            }
            catch (Exception)
            {
                throw;

            }
        }
        public IList<Reservation> UpcomingReservations(int parkId)
        {
            List<Reservation> reservations = new List<Reservation>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Select * from reservation join site on reservation.site_id = site.site_id join campground on campground.campground_id = site.campground_id where park_id = @parkId and from_date between getDate() -1 and getDate() + 30", connection);
                    sqlCommand.Parameters.AddWithValue("@parkId", parkId);
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        reservations.Add(GetReservationFromReader(reader));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return reservations;
        }
        private Reservation GetReservationFromReader(SqlDataReader reader)
        {
            Reservation reservation = new Reservation();
            reservation.ReservationId = Convert.ToInt32(reader["reservation_id"]);
            reservation.SiteId = Convert.ToInt32(reader["site_id"]);
            reservation.Name = Convert.ToString(reader["name"]);
            reservation.FromDate = Convert.ToDateTime(reader["from_date"]);
            reservation.ToDate = Convert.ToDateTime(reader["to_date"]);
            reservation.CreateDate = Convert.ToDateTime(reader["create_date"]);

            return reservation;
        }
    }
}
