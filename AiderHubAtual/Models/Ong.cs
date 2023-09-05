using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AiderHubAtual.Models
{
    [Table("ong")]
    public class Ong
    {
        [Key]
        [Column("id_ong")]
        public int Id { get; set; }
        [Column("razao_social")]
        public string RazaoSocial { get; set; }
        [Column("nome_fantasia")]
        public string NomeFantasia { get; set; }
        [Column("cnpj")]
        public string Cnpj { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("senha")]
        public string Senha { get; set; }
        [Column("assinatura_digital")]
        public byte[] AssinaturaDigital { get; set; }
        [Column("telefone")]
        public string Telefone { get; set; }
        [Column("endereco")]
        public string Endereco { get; set; }
        [Column("foto_logo")]
        public string FotoLogo { get; set; }
        [Column("tipo")]
        public string Tipo { get; set; }

        public Ong() { }

        public Ong(int id_ong, string razao_social, string nome_fantasia, string cnpj, string email, string senha, byte[] assinatura_digital, string telefone, string endereco, string foto_logo, string tipo)
        {
            Id = id_ong;
            RazaoSocial = razao_social;
            NomeFantasia = nome_fantasia;
            Cnpj = cnpj;
            Email = email;
            Senha = senha;
            AssinaturaDigital = assinatura_digital;
            Telefone = telefone;
            Endereco = endereco;
            FotoLogo = foto_logo;
            Tipo = tipo;
        }
    }
}
