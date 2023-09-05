using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AiderHubAtual.Models
{
    [Table("evento")]
    public class Evento
    {
        [Key]
        [Column("id_evento")]
        public int Id_Evento { get; set; }
        [Column("data_hora")]
        public DateTime data_Hora { get; set; }
        [Column("endereco")]
        public string Endereco { get; set; }
        [Column("carga_horario")]
        public TimeSpan Carga_Horario { get; set; }
        [Column("descricao")]
        public string Descricao { get; set; }
        [Column("responsavel")]
        public string Responsavel { get; set; }
        [Column("status")]
        public bool Status { get; set; }
        [Column("id_ong")]
        public int IdOng { get; set; }

        public Evento() { }

        public Evento(int id_evento, DateTime data_hora, string endereco, TimeSpan carga_horario, string descricao, string responsavel, bool status, int id_ong)
        {
            Id_Evento = id_evento;
            data_Hora = data_hora;
            Endereco = endereco;
            Carga_Horario = carga_horario;
            Descricao = descricao;
            Responsavel = responsavel;
            Status = status;
            IdOng = id_ong;
        }
    }
}
