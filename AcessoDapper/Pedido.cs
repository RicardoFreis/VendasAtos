using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcessoDapper
{
    internal class Pedido
    {

        public int Id { get; set; }
        public string Nome { get; set; }
        //public string Telefone { get; set; }
        public Cliente Cliente { get; set; }
        public List<Produto> listaProdutos { get; set; }

        public Pedido()
        {
            this.listaProdutos = new List<Produto>();
        }



    }
}
