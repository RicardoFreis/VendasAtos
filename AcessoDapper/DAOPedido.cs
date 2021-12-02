using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace AcessoDapper
{
    internal class DAOPedido
    {
        const string CONNECTION_STRING = "data source=localhost;initial catalog=VENDAS;Persist Security Info=True;Connection Timeout=60;User ID=sa;Password=boi228369";

        public List<Pedido> RecuperarPedidos()
        {
            List<Pedido> listaPedidos;

            string sql = "SELECT * from Pedidos"; //NUNCA CONCATENAR STRING AQUI, PELA MOR DE DEUS ....

            try
            {
                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    listaPedidos = conexao.Query<Pedido>(sql).AsList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                //throw;
                return null;
            }

            return listaPedidos;
        }

        public Pedido RecuperarPedido(Pedido alu)
        {

            string sql = "SELECT * from Pedidos Where [id] = @id"; //NUNCA CONCATENAR STRING AQUI, PELA MOR DE DEUS ....

            try
            {
                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    alu = conexao.Query<Pedido>(sql, alu).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                //throw;
                return null;
            }

            return alu;
        }


        public Pedido RecuperarPedidoCompleto(Pedido alu)
        {

            string sql = @"SELECT * from Pedidos
                            INNER JOIN Produto ON Produto.idPedido = Pedidos.id
                            INNER JOIN Endereco ON Endereco.idPedido = Pedidos.id
                            WHERE Pedidos.id = @id";

            try
            {
                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    Pedido aluTemp = null;

                    var items = conexao.Query<Pedido, Produto, Cliente, Pedido>(sql,
                        (pedido, produto, cliente) =>  //Este metodo anonimo é executado para cada linha de retorno da consulta
                        {
                            if (aluTemp == null) //se o aluno ainda não esta instanciado 
                            {
                                pedido.Cliente = cliente;
                                aluTemp = pedido;
                                aluTemp.listaProdutos.Add(produto);
                            }
                            else //se o aluno ja esta na coleção eu so adiciono o telefone
                            {
                                aluTemp.listaProdutos.Add(produto);
                            }

                            return aluTemp;
                        },
                        alu);  // splitOn: "id"


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                //throw;
                return null;
            }

            return alu;
        }


        public List<Pedido> RecuperarPedidosCompletos(List<Pedido> paramListaAlu)
        {
            List<Pedido> listaPedidos;


            string sql = @"SELECT * from Pedidos
                            INNER JOIN Produto ON Produto.idPedido = Pedidos.id
                            INNER JOIN Endereco ON Endereco.idPedido = Pedidos.id
                            WHERE Pedidos.id IN @id";


            var parametros = new { id = new string[paramListaAlu.Count] };



            for (int i = 0; i < paramListaAlu.Count; i++)
            {
                parametros.id[i] = paramListaAlu[i].Id.ToString();
            }

            try
            {
                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    listaPedidos = new List<Pedido>();


                    var items = conexao.Query<Pedido, Produto, Cliente, Pedido>(sql,
                        (aluno, produto, cliente) =>  //Este metodo anonimo é executado para cada linha de retorno da consulta
                        {
                            var alu = listaPedidos.Where(a => a.Id == aluno.Id).FirstOrDefault();
                            if (alu == null) //se o aluno ainda não esta na coleção eu adiciono tudo
                            {
                                aluno.Cliente = cliente;
                                alu = aluno;
                                alu.listaProdutos.Add(produto);
                                listaPedidos.Add(alu);
                            }
                            else //se o aluno ja esta na coleção eu so adiciono o telefone
                            {
                                alu.listaProdutos.Add(produto);
                            }

                            return aluno;
                        }, parametros);  // splitOn: "id"

                    //listaPedidos = items.AsList();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                //throw;
                return null;
            }

            return listaPedidos;
        }


        public Pedido AtualizarPedido(Pedido alu)
        {
            try
            {
                string sqlPedido = $@"UPDATE Pedidos
                                     SET [nome] = @nome
                                        ,[telefone] = @telefone
                                     WHERE id = @id";


                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    var retornoPedido = conexao.Execute(sqlPedido, alu);
                    //alu.IdPedido = Convert.ToInt32(retornoPedido);
                }
            }
            catch (DbException exDb)
            {
                Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                Console.WriteLine("DbException.Source: {0}", exDb.Source);
                Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                Console.WriteLine("DbException.Message: {0}", exDb.Message);
                return null;
            }
            // Handle all other exceptions.
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
                return null;
            }

            return alu;
        }

        public int ExecutarProcedureRemovePedido(Pedido alu)
        {
            int linhasAfetadas = 0;

            try
            {
                string procedure = "[removePedido]";
                var pars = new { idPedido = alu.Id };


                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    var retornoPedido = conexao.Execute(procedure, pars, commandType: CommandType.StoredProcedure);
                    linhasAfetadas = Convert.ToInt32(retornoPedido);
                }
            }
            catch (DbException exDb)
            {
                Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                Console.WriteLine("DbException.Source: {0}", exDb.Source);
                Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                Console.WriteLine("DbException.Message: {0}", exDb.Message);
                return 0;
            }
            // Handle all other exceptions.
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
                return 0;
            }


            return linhasAfetadas;
        }

        public List<Produto> ExecutarProcedureConsultaProdutos(Pedido alu)
        {
            List<Produto> listaProdutos;

            try
            {
                string procedure = "[consultarProduto]";
                var pars = new { idPedido = alu.Id };


                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    var telefones = conexao.Query<Produto>(procedure, pars, commandType: CommandType.StoredProcedure);
                    listaProdutos = telefones.AsList();
                    // listaProdutos = conexao.Query<Produto>(procedure, pars, commandType: CommandType.StoredProcedure).AsList();
                }
            }
            catch (DbException exDb)
            {
                Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                Console.WriteLine("DbException.Source: {0}", exDb.Source);
                Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                Console.WriteLine("DbException.Message: {0}", exDb.Message);
                return null;
            }
            // Handle all other exceptions.
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
                return null;
            }


            return listaProdutos;
        }

        public Pedido InserirPedido(Pedido alu)
        {
            alu.Id = 0;
            try
            {
                string sqlPedido = $@"INSERT INTO Pedidos (nome,telefone)
                                                OUTPUT INSERTED.id 
                                                VALUES (@nome, @telefone)"; //NUCA CONCATENAR STRING AQUI, PELA MOR DE DEUS ....


                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    var retornoPedido = conexao.ExecuteScalar(sqlPedido, alu);
                    alu.Id = Convert.ToInt32(retornoPedido);
                }
            }
            catch (DbException exDb)
            {
                Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                Console.WriteLine("DbException.Source: {0}", exDb.Source);
                Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                Console.WriteLine("DbException.Message: {0}", exDb.Message);
                return alu;
            }
            // Handle all other exceptions.
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
                return alu;
            }


            return alu;
        }

        public Pedido InserirPedidoProduto(Pedido alu)
        {
            alu.Id = 0;
            try
            {
                string sqlPedido = $@"INSERT INTO Pedidos (nome,telefone)
                                                OUTPUT INSERTED.id 
                                                VALUES (@nome, @telefone)";

                string sqlProduto = @"INSERT INTO Produto (numeroProduto, idPedido)
                                       VALUES( @numeroProduto, @idPedido)";



                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    conexao.Open();
                    using (var transaction = conexao.BeginTransaction())
                    {
                        var retornoPedido = conexao.ExecuteScalar(sqlPedido, alu, transaction);
                        alu.Id = Convert.ToInt32(retornoPedido);

                        foreach (var tel in alu.listaProdutos)
                        {
                            tel.IdPedido = alu.Id;
                        }

                        var retornoProduto = conexao.Execute(sqlProduto, alu.listaProdutos, transaction);

                        transaction.Commit();
                    }
                }
            }
            catch (DbException exDb)
            {
                Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                Console.WriteLine("DbException.Source: {0}", exDb.Source);
                Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                Console.WriteLine("DbException.Message: {0}", exDb.Message);
                return alu;
            }
            // Handle all other exceptions.
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
                return alu;
            }


            return alu;
        }

        public Pedido InserirPedidoProdutoEndereco(Pedido alu)
        {
            //O objeto aluno deve chegar aqui completo, com a lista de telefones e com o atributo endereço preenchido

            alu.Id = 0;
            try
            {
                string sqlPedido = $@"INSERT INTO Pedidos (nome,telefone)
                                                OUTPUT INSERTED.id 
                                                VALUES (@nome, @telefone)";

                string sqlProduto = @"INSERT INTO Produto (numeroProduto, idPedido)
                                       VALUES( @numeroProduto, @idPedido)";

                string sqlEndereco = @"INSERT INTO Endereco (logradouro, cep, numero, idPedido)
                                       VALUES( @logradouro, @cep, @numero, @idPedido)";



                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    conexao.Open();
                    using (var transaction = conexao.BeginTransaction())
                    {
                        var retornoPedido = conexao.ExecuteScalar(sqlPedido, alu, transaction);
                        alu.Id = Convert.ToInt32(retornoPedido);

                        foreach (var tel in alu.listaProdutos)
                        {
                            tel.IdPedido = alu.Id;
                        }

                        var retornoProduto = conexao.Execute(sqlProduto, alu.listaProdutos, transaction);


                        alu.Cliente.IdPedido = alu.Id;
                        var retornoEndereco = conexao.Execute(sqlEndereco, alu.Cliente, transaction);

                        transaction.Commit();
                    }
                }
            }
            catch (DbException exDb)
            {
                Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                Console.WriteLine("DbException.Source: {0}", exDb.Source);
                Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                Console.WriteLine("DbException.Message: {0}", exDb.Message);
                return alu;
            }
            // Handle all other exceptions.
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
                return alu;
            }


            return alu;
        }

        public List<Pedido> RecuperarPedidosEnderecos()
        {
            List<Pedido> listaPedidos;



            string sql = @"SELECT * from Pedidos
                           INNER JOIN Endereco ON Endereco.idPedido = Pedidos.id";

            try
            {
                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    var items = conexao.Query<Pedido, Cliente, Pedido>(sql,
                        (aluno, cliente) =>
                        {
                            aluno.Cliente = cliente;
                            return aluno;
                        });

                    listaPedidos = items.AsList();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                //throw;
                return null;
            }

            return listaPedidos;
        }

        public List<Pedido> RecuperarPedidosProdutos()
        {
            List<Pedido> listaPedidos;



            string sql = @"SELECT * from Pedidos
                           INNER JOIN Produto ON Produto.idPedido = Pedidos.id";

            try
            {
                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    listaPedidos = new List<Pedido>();

                    var items = conexao.Query<Pedido, Produto, Pedido>(sql,
                        (aluno, telefone) =>  //Este metodo anonimo é executado para cada linha de retorno da consulta
                        {
                            var alu = listaPedidos.Where(a => a.Id == aluno.Id).FirstOrDefault();
                            if (alu == null) //se o aluno ainda não esta na coleção eu adiciono tudo
                            {
                                alu = aluno;
                                alu.listaProdutos.Add(telefone);
                                listaPedidos.Add(alu);
                            }
                            else //se o aluno ja esta na coleção eu so adiciono o telefone
                            {
                                alu.listaProdutos.Add(telefone);
                            }

                            return aluno;
                        }, splitOn: "Produto");

                    //listaPedidos = items.AsList();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                //throw;
                return null;
            }

            return listaPedidos;
        }

        public List<Pedido> RecuperarPedidosProdutosEndereco()
        {
            List<Pedido> listaPedidos;



            string sql = @"SELECT * from Pedidos
                            INNER JOIN Produto ON Produto.idPedido = Pedidos.id
                            INNER JOIN Endereco ON Endereco.idPedido = Pedidos.id";

            try
            {
                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    listaPedidos = new List<Pedido>();

                    var items = conexao.Query<Pedido, Produto, Cliente, Pedido>(sql,
                        (aluno, produto, cliente) =>  //Este metodo anonimo é executado para cada linha de retorno da consulta
                        {
                            var alu = listaPedidos.Where(a => a.Id == aluno.Id).FirstOrDefault();
                            if (alu == null) //se o aluno ainda não esta na coleção eu adiciono tudo
                            {
                                aluno.Cliente = cliente;
                                alu = aluno;
                                alu.listaProdutos.Add(produto);
                                listaPedidos.Add(alu);
                            }
                            else //se o aluno ja esta na coleção eu so adiciono o telefone
                            {
                                alu.listaProdutos.Add(produto);
                            }

                            return aluno;
                        });  // splitOn: "id"

                    //listaPedidos = items.AsList();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                //throw;
                return null;
            }

            return listaPedidos;
        }




        //Corriga este metodo
        public List<Pedido> RecuperarPedidosProdutosEndereco(List<Pedido> listaPedidosParam)
        {
            List<Pedido> listaPedidos;


            string sql = @"SELECT * from Pedidos
                            INNER JOIN Produto ON Produto.idPedido = Pedidos.id
                            INNER JOIN Endereco ON Endereco.idPedido = Pedidos.id
                            WHERE Pedidos.id IN @id";

            try
            {
                using (SqlConnection conexao = new SqlConnection(CONNECTION_STRING))
                {
                    listaPedidos = new List<Pedido>();

                    var items = conexao.Query<Pedido, Produto, Cliente, Pedido>(sql,
                        (aluno, produto, cliente) =>  //Este metodo anonimo é executado para cada linha de retorno da consulta
                        {
                            var alu = listaPedidos.Where(a => a.Id == aluno.Id).FirstOrDefault();
                            if (alu == null) //se o aluno ainda não esta na coleção eu adiciono tudo
                            {
                                aluno.Cliente = cliente;
                                alu = aluno;
                                alu.listaProdutos.Add(produto);
                                listaPedidos.Add(alu);
                            }
                            else //se o aluno ja esta na coleção eu so adiciono o telefone
                            {
                                alu.listaProdutos.Add(produto);
                            }

                            return aluno;
                        },
                        listaPedidosParam);  // splitOn: "id"
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                //throw;
                return null;
            }

            return listaPedidos;
        }



    }
}


