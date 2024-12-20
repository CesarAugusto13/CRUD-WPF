using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static TesteJunior.MainWindow;

namespace TesteJunior
{

    public partial class Cadastro : Window
    {
        private string connectionString = "Server=localhost;Database=testdev;Uid=root;Pwd=root;";
        private Produto _produto;

        public Cadastro(Produto produto = null)
        {
            InitializeComponent();
            CarregarGrupos();

            _produto = produto;
            if (_produto != null)
            {
                txtNomeProduto.Text = _produto.Descricao.ToString();
                txtPrecoCusto.Text = _produto.PrecoCusto.ToString();
                txtPrecoVenda.Text = _produto.PrecoVenda.ToString();
                cmbGrupo.Text = _produto.NomeGrupo.ToString();
                chkAtivo.IsChecked = _produto.Ativo;
            }
        }

        private void CarregarGrupos()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT nome FROM produto_grupo", conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        cmbGrupo.Items.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar grupos: " + ex.Message);
            }
        }
        private void btnCadastrar_Click(object sender, RoutedEventArgs e)
        {
            string nomeProduto = txtNomeProduto.Text;
            decimal precoCusto = Convert.ToDecimal(txtPrecoCusto.Text);
            decimal precoVenda = Convert.ToDecimal(txtPrecoVenda.Text);
            string grupoSelecionado = cmbGrupo.SelectedItem.ToString();
            bool ativo = chkAtivo.IsChecked.Value;
            DateTime dataHoraCadastro = DateTime.Now;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "INSERT INTO produto (Descricao, PrecoCusto, PrecoVenda, codGrupo, Ativo, DataHoraCadastro) " +
                        "VALUES (@Descricao, @PrecoCusto, @PrecoVenda, @codGrupo, @Ativo, @DataHoraCadastro)", conn);

                    cmd.Parameters.AddWithValue("@Descricao", nomeProduto);
                    cmd.Parameters.AddWithValue("@PrecoCusto", precoCusto);
                    cmd.Parameters.AddWithValue("@PrecoVenda", precoVenda);
                    switch (grupoSelecionado)
                    {
                        case "GERAL":
                            grupoSelecionado = "1";
                            break;
                        case "AÇOUGUE":
                            grupoSelecionado = "2";
                            break;
                        case "HORTIFRUTI":
                            grupoSelecionado = "3";
                            break;
                        case "PADARIA":
                            grupoSelecionado = "4";
                            break;
                        case "GELADEIRA":
                            grupoSelecionado = "5";
                            break;
                    }

                    cmd.Parameters.AddWithValue("@codGrupo", grupoSelecionado);
                    cmd.Parameters.AddWithValue("@Ativo", ativo);
                    cmd.Parameters.AddWithValue("@DataHoraCadastro", dataHoraCadastro);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Produto cadastrado com sucesso!");
                }

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar produto: " + ex.Message);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            string nomeProduto = txtNomeProduto.Text;
            decimal precoCusto = Convert.ToDecimal(txtPrecoCusto.Text);
            decimal precoVenda = Convert.ToDecimal(txtPrecoVenda.Text);
            string grupoSelecionado = cmbGrupo.SelectedItem.ToString();
            bool ativo = chkAtivo.IsChecked.HasValue && chkAtivo.IsChecked.Value;
            int cod = _produto.Cod;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "UPDATE produto " +
                        "SET Descricao = @Descricao, PrecoCusto = @PrecoCusto, PrecoVenda = @PrecoVenda, codGrupo = @codGrupo, Ativo = @Ativo " +
                        "WHERE cod = @cod", conn);

                    cmd.Parameters.AddWithValue("@Descricao", nomeProduto);
                    cmd.Parameters.AddWithValue("@PrecoCusto", precoCusto);
                    cmd.Parameters.AddWithValue("@PrecoVenda", precoVenda);

                    int codGrupo;
                    switch (grupoSelecionado)
                    {
                        case "GERAL":
                            codGrupo = 1;
                            break;
                        case "AÇOUGUE":
                            codGrupo = 2;
                            break;
                        case "HORTIFRUTI":
                            codGrupo = 3;
                            break;
                        case "PADARIA":
                            codGrupo = 4;
                            break;
                        case "GELADEIRA":
                            codGrupo = 5;
                            break;
                        default:
                            codGrupo = 0;
                            break;
                    }

                    cmd.Parameters.AddWithValue("@codGrupo", codGrupo);
                    cmd.Parameters.AddWithValue("@Ativo", ativo);
                    cmd.Parameters.AddWithValue("@cod", cod);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Produto editado com sucesso!");
                }

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao editar produto: " + ex.Message);
            }
        }
    } 
}
