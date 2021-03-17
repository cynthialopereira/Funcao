using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAtividadeEntrevista.Models
{
    /// <summary>
    /// Classe de Modelo de Cliente
    /// </summary>
    public class BeneficiarioModel
    {
        public long Posicao { get; set; }

        //-1 - Excluir
        //1 - Incluir
        public long Acao { get; set; }


        public long Id { get; set; }

        public long IdCliente { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        public string Nome { get; set; }

        /// <summary>
        /// Cpf
        /// </summary>
        [Required]
        [MaxLength(14)]
        public string Cpf { get; set; }


    }
}