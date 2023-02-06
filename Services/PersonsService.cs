using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using WpfRefactorFramework.Models;

namespace WpfRefactorFramework.Services
{
    public static class PersonsService
    {
        
        private const string StringConnection = "data source=192.168.250.25;initial catalog=univer;persist security info=True;user id=Pavel;password=~Pss~JrY;MultipleActiveResultSets=True;";


        public static async Task InsertMove(int idPropusk, string typeStatus, DateTime dateTime)
        {
            using var con = new SqlConnection(StringConnection);
            con.Open();

            const string sql = @"INSERT INTO Move(idPersonRooms , DateTimeMove , TypeStatus) values(@idPersonRooms , @DateTimeMove , @TypeStatus)";
            //const string sql = @"Update pers.""Family"" SET created_at = @createAt where id_person = @idPerson";
            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("idPersonRooms", idPropusk);
            cmd.Parameters.AddWithValue("DateTimeMove", dateTime);
            cmd.Parameters.AddWithValue("TypeStatus", typeStatus);

            await cmd.ExecuteNonQueryAsync();
            con.Close();
            Console.WriteLine("row inserts");
        }

        public static async Task UpdateStatus(string status, int idPerson)
        {

            using var connection = new SqlConnection(StringConnection);
            connection.Open();

            var sql = $"UPDATE PROPUSK SET status = '{status}' where idPersonRooms = {idPerson}";
            using var command = new SqlCommand(sql, connection);
            //command.ExecuteNonQuery();
            await command.ExecuteNonQueryAsync();

            connection.Close();

        }

        public static async Task<int> GetInnerPersons()
        {
            var sql = $@"select count(pr.id)   
                        from PersonRooms as pr
                        inner join Propusk P on pr.id = P.idPersonRooms
                        inner join DepartamentsHostel DH on pr.idDepartametHostel = DH.id
                        inner join StudentsGroup SG on pr.idStudGroup = SG.id
                        inner join Students S on SG.idStudent = S.id
                        inner join Rooms R2 on DH.idRooms = R2.id
                        inner join Section S2 on R2.idSection = S2.id
                        inner join Hostel H on S2.idHostel = H.id
                        inner join Buildings B on H.idBuildings = B.id
                        where B.name = 'Общежитие №7' and  P.blocked <> 'T' and P.status = 'T' ";
            using var conn = new SqlConnection(StringConnection);
            conn.Open();
            using var command = new SqlCommand(sql, conn);
            var reader = await command.ExecuteReaderAsync();

            return reader.Read() ? reader.GetInt32(0) : 0;
        }
        public static async Task<int> GetCountAllPersons()
        {
            var sql = $@"select count(pr.id)   
                        from PersonRooms as pr
                        inner join Propusk P on pr.id = P.idPersonRooms
                        inner join DepartamentsHostel DH on pr.idDepartametHostel = DH.id
                        inner join StudentsGroup SG on pr.idStudGroup = SG.id
                        inner join Students S on SG.idStudent = S.id
                        inner join Rooms R2 on DH.idRooms = R2.id
                        inner join Section S2 on R2.idSection = S2.id
                        inner join Hostel H on S2.idHostel = H.id
                        inner join Buildings B on H.idBuildings = B.id
                        where B.name = 'Общежитие №7' and  P.blocked <> 'T'";
            using var conn = new SqlConnection(StringConnection);
            conn.Open();
            using var command = new SqlCommand(sql, conn);
            var reader = await command.ExecuteReaderAsync();

            return reader.Read() ? reader.GetInt32(0) : 0;
        }
        
        public static async Task<int> GetOutherPersons()
        {
            var sql = $@"select count(pr.id)   
                        from PersonRooms as pr
                        inner join Propusk P on pr.id = P.idPersonRooms
                        inner join DepartamentsHostel DH on pr.idDepartametHostel = DH.id
                        inner join StudentsGroup SG on pr.idStudGroup = SG.id
                        inner join Students S on SG.idStudent = S.id
                        inner join Rooms R2 on DH.idRooms = R2.id
                        inner join Section S2 on R2.idSection = S2.id
                        inner join Hostel H on S2.idHostel = H.id
                        inner join Buildings B on H.idBuildings = B.id
                        where B.name = 'Общежитие №7' and  P.blocked <> 'T' and P.status = 'F' ";
            using var conn = new SqlConnection(StringConnection);
            conn.Open();
            using var command = new SqlCommand(sql, conn);
            var reader = await command.ExecuteReaderAsync();

            return reader.Read() ? reader.GetInt32(0) : 0;
        } 
        
        public static async Task<IEnumerable<Person>> GetPersonList()
        {
            var persons = new List<Person>();
            var sql = $@"select pr.id,  P.id as Idpropusk, S.surname, S.name as secondName, S.patronymic, B.name as build, S2.nameSection, R2.numberRoom, P.numberPropusk ,pr.status ,S.photo , P.status as propuskStatus
                        from PersonRooms as pr
                        inner join Propusk P on pr.id = P.idPersonRooms
                        inner join DepartamentsHostel DH on pr.idDepartametHostel = DH.id
                        inner join StudentsGroup SG on pr.idStudGroup = SG.id
                        inner join Students S on SG.idStudent = S.id
                        inner join Rooms R2 on DH.idRooms = R2.id
                        inner join Section S2 on R2.idSection = S2.id
                        inner join Hostel H on S2.idHostel = H.id
                        inner join Buildings B on H.idBuildings = B.id
                        where B.name = 'Общежитие №7'";

            using var conn = new SqlConnection(StringConnection);
            conn.Open();
            using var command = new SqlCommand(sql, conn);
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                persons.Add(new Person
                {
                    IdPropusk = Convert.ToInt32(reader["Idpropusk"].ToString()),
                    Id = Convert.ToInt32(reader["id"].ToString()),
                    FirstName = reader["surname"].ToString(),
                    SecondName = reader["secondName"].ToString(),
                    LastName = reader["patronymic"].ToString(),
                    Building = reader["build"].ToString(),
                    Section = reader["nameSection"].ToString(),
                    Room = reader["numberRoom"].ToString(),
                    StatusPerson = reader["status"].ToString(),
                    Avatar = reader["photo"] as byte[],
                    StatusPropusk = reader["propuskStatus"].ToString(),
                });
            }
            reader.Close();
            conn.Close();

            return persons;

        }
         
        public static async Task<Person> GetPerson(string code)
        {
            var person = new Person();
            var sql = $@"select top 1 pr.id,  P.id as Idpropusk, S.surname, S.name as secondName, S.patronymic, B.name as build, S2.nameSection, R2.numberRoom, P.numberPropusk ,pr.status ,S.photo , P.status as propuskStatus
                        from PersonRooms as pr
                        inner join Propusk P on pr.id = P.idPersonRooms
                        inner join DepartamentsHostel DH on pr.idDepartametHostel = DH.id
                        inner join StudentsGroup SG on pr.idStudGroup = SG.id
                        inner join Students S on SG.idStudent = S.id
                        inner join Rooms R2 on DH.idRooms = R2.id
                        inner join Section S2 on R2.idSection = S2.id
                        inner join Hostel H on S2.idHostel = H.id
                        inner join Buildings B on H.idBuildings = B.id
                        where B.name = 'Общежитие №7' and P.numberPropusk = '{code}'";

            using var conn = new SqlConnection(StringConnection);
            conn.Open();
            using var command = new SqlCommand(sql, conn);
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                person = new Person
                {
                    IdPropusk = Convert.ToInt32(reader["Idpropusk"].ToString()),
                    Id = Convert.ToInt32(reader["id"].ToString()),
                    FirstName = reader["surname"].ToString(),
                    SecondName = reader["secondName"].ToString(),
                    LastName = reader["patronymic"].ToString(),
                    Building = reader["build"].ToString(),
                    Section = reader["nameSection"].ToString(),
                    Room = reader["numberRoom"].ToString(),
                    StatusPerson = reader["status"].ToString(),
                    Avatar = reader["photo"] as byte[],
                    StatusPropusk = reader["propuskStatus"].ToString(),
                };

            }
            reader.Close();
            conn.Close();

            return person;

        }
        public static async Task<IEnumerable<Move>> GetMove()
        {
            var array = new List<Move>();
            const string sql = @"select top 200 S.surname, S.name as secondName, S.patronymic , M.DateTimeMove , M.TypeStatus , B.name as build, S2.nameSection, R2.numberRoom 
                    from PersonRooms as pr
                    inner join Propusk P on pr.id = P.idPersonRooms
                    inner join DepartamentsHostel DH on pr.idDepartametHostel = DH.id
                    inner join Move M on P.id = M.idPersonRooms
                    inner join StudentsGroup SG on pr.idStudGroup = SG.id
                    inner join Students S on SG.idStudent = S.id
                    inner join Rooms R2 on DH.idRooms = R2.id
                    inner join Section S2 on R2.idSection = S2.id
                    inner join Hostel H on S2.idHostel = H.id
                    inner join Buildings B on H.idBuildings = B.id
                    where B.name = 'Общежитие №7'
                    order by  M.DateTimeMove desc";
            using var conn = new SqlConnection(StringConnection);
            conn.Open();
            using var command = new SqlCommand(sql, conn);
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                array.Add(new Move
                {
                    FirstName = reader["surname"].ToString(),
                    SecondName = reader["secondName"].ToString(),
                    LastName = reader["patronymic"].ToString(),
                    LastTime = DateTime.Parse(reader["DateTimeMove"].ToString()),
                    Status = reader["TypeStatus"].ToString(),
                    Building = reader["build"].ToString(),
                    Section = reader["nameSection"].ToString(),
                    Room = reader["numberRoom"].ToString(),
                });

            }
            reader.Close();
            conn.Close();
            return array;
        }
    }
}
