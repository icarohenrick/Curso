using Microsoft.EntityFrameworkCore;

namespace Curso.Domain
{
    public class Documento
    {
        private string _cpf;

        public int Id { get; set; }

        public void SetCPF(string cpf)
        {
            if(string.IsNullOrWhiteSpace(cpf))
            {
                throw new System.Exception("CPF Inválido");
            }

            _cpf = cpf;
        }

        [BackingField(nameof(_cpf))]
        public string CPF => _cpf;
        public string GetCPF() => _cpf;
    }
}