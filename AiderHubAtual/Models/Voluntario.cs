using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiderHubAtual.Models
{
    [Table("voluntario")]
    public class Voluntario
    {
        [Key]
        [Column("id_voluntario")]
        public int Id { get; set; }
        [Column("nome")]
        public string Nome { get; set; }
        [Column("foto_logo")]
        public string Foto { get; set; }
        [Column("data_nascimento")]
        public DateTime DataNascimento { get; set; }
        [Column("cpf")]
        public string Cpf { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("senha")]
        public string Senha { get; set; }
        [Column("telefone")]
        public string Telefone { get; set; }
        [Column("endereco")]
        public string Endereco { get; set; }
        [Column("formacao")]
        public string Formacao { get; set; }
        [Column("sobre")]
        public string Sobre { get; set; }
        [Column("interesses")]
        public string Interesses { get; set; }
        [Column("tipo")]
        public string Tipo { get; set; }
        public Voluntario() { }

        public Voluntario(int id_voluntario, string nome, string foto_logo, DateTime data_nascimento, string cpf, string email, string senha, string endereco, string formacao, string sobre, string interesses,  string telefone, string tipo)
        {
            Id = id_voluntario;
            Nome = nome;
            Foto = foto_logo;
            DataNascimento = data_nascimento;
            Cpf = cpf;
            Email = email;
            Senha = senha;
            Telefone = telefone;
            Endereco = endereco;
            Formacao = formacao;
            Sobre = sobre;
            Interesses = interesses;
            Tipo = tipo;
        }
    }
}
