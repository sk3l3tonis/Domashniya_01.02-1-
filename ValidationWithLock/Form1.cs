using Npgsql;

namespace ValidationWithLock
{
    public partial class Form1 : Form
    {
        private int loginAttempts = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Авторизация прошла успешно!");
                this.Close();
            }
            else
            {
                loginAttempts++;

                if (loginAttempts >= 3)
                {
                    MessageBox.Show("Вы превысили лимит попыток входа. Попробуйте снова через 20 секунд.");
                    btnLogin.Enabled = false;
                    Thread.Sleep(20000); // Задержка на 20 секунд
                    btnLogin.Enabled = true;
                    loginAttempts = 0;
                }
                else
                {
                    MessageBox.Show("Неверные имя пользователя или пароль. Попробуйте еще раз.");
                }
            }
        }
        private bool AuthenticateUser(string username, string password)
        {
            string connectionString = "Server=localhost;Port=5432;Database=auth;User Id=postgres;Password=1212321;";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM users WHERE Username = @username AND Password = @password";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }
    }
}

    