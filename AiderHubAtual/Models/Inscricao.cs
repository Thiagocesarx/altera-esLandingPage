using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiderHubAtual.Models
{
    [Table("inscricao")]
    public class Inscricao
    {
        [Key]
        [Column("id_inscricao")]
        public int Id { get; set; }
        [Column("id_evento")]
        public int idEvento { get; set; }
        [Column("id_voluntario")]
        public int idVoluntario { get; set; }
        [Column("status")]
        public bool Status { get; set; }
        [Column("tipo")]
        public string Tipo { get; set; }
        [Column("confirmacao")]
        public bool Confirmacao { get; set; }

        [Column("datainscricao")]
        public DateTime DataInscricao { get; set; }

        public Inscricao() { }

        public Inscricao(int id_inscricao, int id_evento, int id_voluntario, bool status, string tipo, bool confirmacao, DateTime datainscricao)
        {
            Id = id_inscricao;
            idEvento = id_evento;
            idVoluntario = id_voluntario;
            Status = status;
            Tipo = tipo;
            Confirmacao = confirmacao;
            DataInscricao = datainscricao;
        }
    }
}
