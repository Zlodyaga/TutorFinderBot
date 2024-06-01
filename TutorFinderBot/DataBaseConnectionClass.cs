using MySql.Data.MySqlClient;
using TutorFinderBot.Data_classes;

namespace TutorFinderBot
{
    public static class DataBaseConnectionClass
    {
        public static MySqlConnection connection = new MySqlConnection("server=localhost;port=3307;username=root;password=root;database=findteachers");

        public static void OpenConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }

        public static void CloseConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }

        public static MySqlConnection GetConnection()
        {
            return connection;
        }


        public static List<IUser> GetUsersAll()
        {
            List<IUser> users = new List<IUser>();

            OpenConnection();

            string query = "SELECT * FROM users;";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32("id_user");
                string username = reader.GetString("username");
                string lastName = reader.GetString("last_name");
                string firstName = reader.GetString("first_name");
                string email = reader.GetString("email");
                string phone = reader.GetString("phone");
                string sex = reader.GetString("sex");
                DateTime dateOfBirth = reader.GetDateTime("date_of_birth");

                switch (reader.GetString("role_in_system"))
                {
                    case "admin":
                        users.Add(new Admin(id, username, lastName, firstName, email, phone, dateOfBirth, sex));
                        break;
                    case "teacher":
                        users.Add(new Teacher(id, username, lastName, firstName, email, phone, dateOfBirth, sex));
                        break;
                    case "student":
                        users.Add(new Student(id, username, lastName, firstName, email, phone, dateOfBirth, sex));
                        break;
                    default:
                        break;
                }
            }

            reader.Close();
            CloseConnection();

            return users;
        }

        public static List<Vacancy> GetVacancies()
        {
            List<Vacancy> vacancies = new List<Vacancy>();

            OpenConnection();

            string query = "SELECT * FROM vacancies;";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id_vacancy = reader.GetInt32("id_vacancy");
                int id_user = reader.GetInt32("id_user");
                double hourly_rate = reader.GetDouble("hourly_rate");
                string subject = reader.GetString("subject_of_studying");
                string status = reader.GetString("status_vacancy");

                vacancies.Add(new Vacancy(id_vacancy, id_user, hourly_rate, subject, status));
            }

            reader.Close();
            CloseConnection();

            return vacancies;
        }

        public static List<Vacancy> GetActualVacancies() => GetVacancies().Where(v => v.Status == "Розміщено").ToList();
    }
}
