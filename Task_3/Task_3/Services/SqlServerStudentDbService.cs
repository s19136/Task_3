using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Task_3.DTO.Request;

namespace Task_3.Services
{
    public class SqlServerStudentDbService : IStudentServiceDb
    {
        public string EnrollStudent(EnrollStudentRequest request)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19136;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();

                com.CommandText = "Select * From Studies Where Name=@Name";
                com.Parameters.AddWithValue("Name", request.Studies);
                com.Transaction = tran;

                var dr = com.ExecuteReader();
                if (!dr.Read()) //Check if studies exists
                {
                    dr.Close();
                    tran.Rollback();
                    return "No such studies";
                }
                var IdStudy = (int)dr["IdStudy"];

                var IdEnrollment = 1;
                com.CommandText = "Select * From Enrollment, Studies Where Semester=1 And Enrollment.IdStudy = Studies.IdStudy and Name=@Name";
                dr.Close();
                dr = com.ExecuteReader();
                if (!dr.Read()) // Check if Enrollment with semester = 1 exists for these studies
                {
                    com.CommandText = "Select max(IdEnrollment) as MaxId From Enrollment";
                    dr.Close();
                    dr = com.ExecuteReader();
                    dr.Read();
                    IdEnrollment = (int)dr["MaxId"] + 1; //take IdEnrollment that we created
                    com.CommandText = "insert into Enrollment(IdEnrollment, IdStudy, Semester, StartDate) values " +
                        "(@IdEnrollment, @IdStudy, @Semester, @StartDate)";
                    com.Parameters.AddWithValue("IdEnrollment", IdEnrollment);
                    com.Parameters.AddWithValue("IdStudy", IdStudy);
                    com.Parameters.AddWithValue("Semester", 1);
                    com.Parameters.AddWithValue("StartDate", DateTime.Now.ToString());
                    dr.Close();
                    com.ExecuteNonQuery();
                }
                else
                {
                    IdEnrollment = (int)dr["IdEnrollment"]; //take existing IdEnrollment to insert in Student later
                }

                com.CommandText = "Select * From Student Where IndexNumber=@IndexNumber";
                com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                dr.Close();
                dr = com.ExecuteReader();
                if (dr.Read()) //Check if there is already student with this index number
                {
                    dr.Close();
                    tran.Rollback();
                    return "There already is student with this index";
                }

                //Insert student
                com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES " +
                    "(@IndexNumber, @FirstName, @LastName, @BirthDate, @NewIdEnrollment)";
                com.Parameters.AddWithValue("FirstName", request.FirstName);
                com.Parameters.AddWithValue("LastName", request.LastName);
                com.Parameters.AddWithValue("BirthDate", request.BirthDate);
                com.Parameters.AddWithValue("NewIdEnrollment", IdEnrollment);
                dr.Close();
                com.ExecuteNonQuery();

                //tran.Rollback();
                tran.Commit();

                com.CommandText = "Select * From Enrollment " +
                    "Where IdEnrollment = @NewIdEnrollment";
                dr.Close();
                dr = com.ExecuteReader();
                dr.Read();
                return dr["IdEnrollment"].ToString() + " " + dr["IdStudy"].ToString() + " "
                    + dr["Semester"].ToString() + " " + dr["StartDate"].ToString();
            }

        }

        public string Promote(PromoteStudentRequest request)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19136;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();

                com.CommandText = "Select IdEnrollment From Enrollment, Studies " +
                    "Where Enrollment.IdStudy = Studies.IdStudy and Studies.Name = @Name and Enrollment.Semester = @Semester";
                com.Parameters.AddWithValue("Name", request.Studies);
                com.Parameters.AddWithValue("Semester", Int64.Parse(request.Semester));
                com.Transaction = tran;

                var dr = com.ExecuteReader();
                if (!dr.Read()) //Check if studies exists
                {
                    dr.Close();
                    tran.Rollback();
                    return "No such record in Enrollment";
                }

                com.CommandText = "EXEC Promote @Name, @Semester";
                dr.Close();
                com.ExecuteNonQuery();

                tran.Commit();

                com.CommandText = "Select * From Enrollment, Studies " +
                    "Where Enrollment.IdStudy = Studies.IdStudy and Studies.Name = @Name and Enrollment.Semester = @NewSemester";
                com.Parameters.AddWithValue("NewSemester", Int64.Parse(request.Semester)+1);
                dr.Close();
                dr = com.ExecuteReader();
                dr.Read();
                return dr["IdEnrollment"].ToString() + " " + dr["IdStudy"].ToString() + " " 
                    + dr["Semester"].ToString() + " " + dr["StartDate"].ToString();
            }
        }
    }
}
