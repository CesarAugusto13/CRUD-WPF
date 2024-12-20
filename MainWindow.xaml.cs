using MySql.Data.MySqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TesteJunior
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Server=localhost;Database=testdev;Uid=root;Pwd=root;";

        public class Produto
        {
            public int Cod { get; set; }
            public string Descricao { get; set; }
            public string NomeGrupo { get; set; }
            public decimal PrecoCusto { get; set; }
            public decimal PrecoVenda { get; set; }
            public bool Ativo { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            CarregarProdutos();

        }

        public void CarregarProdutos()
        {
            try
            {
                List<Produto> produtos = new List<Produto>();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT Cod, Descricao, codGrupo, PrecoCusto, PrecoVenda, Ativo " +
                        "FROM produto", conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Produto produto = new Produto
                        {
                            Cod = reader.GetInt32(0),
                            Descricao = reader.GetString(1),
                            NomeGrupo = GetNomeGrupo(reader.GetInt32(2)),
                            PrecoCusto = reader.GetDecimal(3),
                            PrecoVenda = reader.GetDecimal(4),
                            Ativo = reader.GetBoolean(5)
                        };
                        produtos.Add(produto);
                    }
                }

                dgdProdutos.ItemsSource = produtos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar produtos: " + ex.Message);
            }
        }
        private string GetNomeGrupo(int codGrupo)
        {
            switch (codGrupo)
            {
                case 1:
                    return "GERAL";
                case 2:
                    return "AÇOUGUE";
                case 3:
                    return "HORTIFRUTI";
                case 4:
                    return "PADARIA";
                case 5:
                    return "GELADEIRA";
                default:
                    return "Desconhecido";
            }
        }
        private void RemoverProduto(int Cod)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM produto WHERE Cod = @Cod", conn);
                    cmd.Parameters.AddWithValue("@Cod", Cod);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao remover produto: " + ex.Message);
                throw;
            }
        }

        private void btnRedirecionarCadastro_Click(object sender, RoutedEventArgs e)
        {
            Cadastro Cadastro = new Cadastro();
            Cadastro.Show();
            this.Close();
        }

        private void btnRemover_Click(object sender, RoutedEventArgs e)
        {
            Produto produtoSelecionado = dgdProdutos.SelectedItem as Produto;
            if (produtoSelecionado == null)
            {
                MessageBox.Show("Selecione um produto para remover.");
                return;
            }

            if (produtoSelecionado.Ativo)
            {
                MessageBox.Show("Este produto não pode ser removido porque está ativo.");
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Você realmente deseja remover o produto {produtoSelecionado.Descricao}?",
                                                      "Confirmar Remoção", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    RemoverProduto(produtoSelecionado.Cod);
                    MessageBox.Show("Produto removido com sucesso!");
                    CarregarProdutos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao remover produto: " + ex.Message);
                }
            }
        }

        private void btnRedirecionarEditar_Click(object sender, RoutedEventArgs e)
        {
            Produto produtoSelecionado = dgdProdutos.SelectedItem as Produto;

            if (produtoSelecionado != null)
            {
                Cadastro cadastro = new Cadastro(produtoSelecionado);
                cadastro.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Selecione um produto para editar.");
            }
        }


        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}