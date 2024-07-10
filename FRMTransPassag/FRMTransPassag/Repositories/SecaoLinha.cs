using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRMTransPassag.Repositories
{
    public class SecaoLinha
    {
        public SecaoLinha(string code = null, string codeLinha = null, string partida = null, string chegada = null)
        {
            if (code != null)
                this.Code = code;
            
            if (codeLinha != null)
                this.CodeLinha = codeLinha;

            if (partida != null)
                this.LocalPartida = partida;

            if (chegada != null)
                this.LocalChegada = chegada;
        }

        public string Code { get; set; }
        public string CodeLinha { get; set; }
        public string LocalPartida { get; set; }
        public string LocalChegada { get; set; }

    }
}
