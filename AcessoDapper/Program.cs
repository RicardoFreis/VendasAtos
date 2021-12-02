using System;
using System.Collections.Generic;
using System.Linq;


namespace AcessoDapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Desenvolver a entrada de dados ...");



            ////Listar pedido
            //DAOPedido daoPedido = new DAOPedido();
            //List<Pedido> listaPedido = new List<Pedido>()
            //{
            // new Pedido() { Id = 1},
            // new Pedido() { Id = 2}
            //};
            //listaPedido = daoPedido.RecuperarPedidosCompletos(listaPedido);



            ////Recupera pedidos simples
            //List<Pedido> listaPedidos = daoPedido.RecuperarPedidos();

            ////Recupera pedidos com 1 join
            //listaPedidos = daoPedido.RecuperarPedidosEnderecos();
            //listaPedidos = daoPedido.RecuperarPedidosProdutos();

            ////Recupera pedidos com 2 joins
            //listaPedidos = daoPedido.RecuperarPedidosProdutosEndereco();



            ////Inserindo produto
            //Pedido alu = new Pedido();
            //alu.Nome = "Homem Arana";

            //var listaProduto = new List<Produto>
            //              {
            //                  { new Produto { Descricao  = "Cerveja" } },
            //                  { new Produto { Descricao  = "Guaraná" } },
            //                  { new Produto { Descricao  = "Tubaína" } }
            //              };
            //alu.listaProdutos = listaProduto;
            //alu.Cliente = new Cliente()
            //{
            //    Nome = "Antônio",
            //    Cpf = "123.232.434/09",
            //};

            //alu = daoPedido.InserirPedido(alu);
            //alu = daoPedido.InserirPedidoProduto(alu);
            //alu = daoPedido.InserirPedidoProdutoEndereco(alu);



            //alu.Nome = "José";
            //alu = daoPedido.AtualizarPedido(alu);

            //var linhas = daoPedido.ExecutarProcedureRemovePedido(alu);
            //var produtos = daoPedido.ExecutarProcedureConsultaProdutos(alu);

            Console.WriteLine("Final");
            Console.ReadKey();
        }
    }
}
