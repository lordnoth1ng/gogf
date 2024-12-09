using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadStatuses();
        }

        private void LoadStatuses()
        {
            cmbStatus.Items.Add("В ожидании");
            cmbStatus.Items.Add("В работе");
            cmbStatus.Items.Add("Выполнено");
            cmbStatus.SelectedIndex = 0; // Установка значения по умолчанию
        }


        private void btnAddRequest_Click(object sender, EventArgs e)
        {
            // Проверка введенных данных
            if (string.IsNullOrEmpty(txtEquipment.Text) || string.IsNullOrEmpty(txtFaultType.Text) ||
                string.IsNullOrEmpty(txtProblemDescription.Text) || string.IsNullOrEmpty(txtClient.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            // Создаем строку подключения
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // SQL-запрос на вставку данных (без RequestID)
                string query = "INSERT INTO RepairRequests (Equipment, FaultType, ProblemDescription, Client, Stat) " +
                               "VALUES (@Equipment, @FaultType, @ProblemDescription, @Client, @Stat)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Equipment", txtEquipment.Text);
                    command.Parameters.AddWithValue("@FaultType", txtFaultType.Text);
                    command.Parameters.AddWithValue("@ProblemDescription", txtProblemDescription.Text);
                    command.Parameters.AddWithValue("@Client", txtClient.Text);

                    // Получение значения статуса
                    string selectedStatus = cmbStatus.SelectedItem.ToString();

                    // Проверка на допустимое значение статуса
                    if (selectedStatus != "В ожидании" && selectedStatus != "В работе" && selectedStatus != "Выполнено")
                    {
                        MessageBox.Show("Недопустимое значение статуса.");
                        return;
                    }

                    command.Parameters.AddWithValue("@Stat", selectedStatus);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Заявка успешно добавлена.");
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show("Ошибка при добавлении заявки: " + sqlEx.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла непредвиденная ошибка: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}
