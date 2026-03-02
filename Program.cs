using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Trabalho_Prático;

namespace Trabalho_Prático
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int nCursos, nCandi, codC;
            string linha;
            Dictionary<int, Cursos> cursos = new Dictionary<int, Cursos>();

            StreamReader arq = new StreamReader("entrada1.txt", Encoding.UTF8);
            linha = arq.ReadLine();
            string[] num = linha.Split(';');
            nCursos = int.Parse(num[0]);
            nCandi = int.Parse(num[1]);
            Candidato[] total = new Candidato[nCandi];
            for (int j = 0; j < nCursos; j++)
            {
                linha = arq.ReadLine();
                string[] cu = linha.Split(';');
                codC = int.Parse(cu[0]);
                Cursos curso = new Cursos()
                {
                    nomcurso = cu[1],
                    quantVagas = int.Parse(cu[2]),
                };
                cursos.Add(codC, curso);
            }

            int i = 0;
            do
            {
                linha = arq.ReadLine();
                if (linha == null) break;
                string[] can = linha.Split(';');
                Candidato candidatos = new Candidato()
                {
                    nome = can[0],
                    notaR = double.Parse(can[1]),
                    notaM = double.Parse(can[2]),
                    notaL = double.Parse(can[3]),
                    cod1 = int.Parse(can[4]),
                    cod2 = int.Parse(can[5])
                };

                candidatos.nota = (candidatos.notaR + candidatos.notaM + candidatos.notaL) / 3;
                total[i] = candidatos;
                i++;
            } while (linha != null);

            arq.Close();

            Quicksort(total, 0, nCandi - 1);

            for (int k = 0; k < total.Length; k++)
            {
                Candidato candidato = total[k];
                Cursos cur1 = cursos[candidato.cod1];
                Cursos cur2 = cursos[candidato.cod2];

                if (cur1.selecionados.Count < cur1.quantVagas)
                {
                    cur1.selecionados.Add(candidato);
                }
                else if (cur2.selecionados.Count < cur2.quantVagas)
                {
                    cur2.selecionados.Add(candidato);
                    cur1.espera.Inserir(candidato);
                }
                else
                {
                    cur1.espera.Inserir(candidato);
                    cur2.espera.Inserir(candidato);
                }
            }

            StreamWriter saida = new StreamWriter("saida.txt");
            foreach (KeyValuePair<int, Cursos> par in cursos)
            {
                Cursos c = par.Value;

                double corte = 0;
                if (c.selecionados.Count > 0)
                {
                    corte = c.selecionados[c.selecionados.Count - 1].nota;
                }

                saida.WriteLine(c.nomcurso + " " + corte.ToString("F2"));
                saida.WriteLine("Selecionados");

                for (int s = 0; s < c.selecionados.Count; s++)
                {
                    Candidato cand = c.selecionados[s];
                    saida.WriteLine(cand.nome + " " + cand.nota.ToString("F2"));
                }

                List<Candidato> fila = new List<Candidato>();
                while (!c.espera.Vazia())
                {
                    fila.Add(c.espera.Remover());
                }

                saida.WriteLine("Fila de Espera");
                for (int f = 0; f < fila.Count; f++)
                {
                    Candidato cand = fila[f];
                    saida.WriteLine(cand.nome + " " + cand.nota.ToString("F2"));
                }

                saida.WriteLine();
            }
            saida.Close();
            Console.ReadLine();
        }

        static void Quicksort(Candidato[] array, int esq, int dir)
        {
            int i = esq, j = dir;
            Candidato pivo = array[(esq + dir) / 2];

            while (i <= j)
            {
                while (Maior(array[i], pivo))
                    i++;
                while (Menor(array[j], pivo))
                    j--;

                if (i <= j)
                {
                    Candidato temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    i++;
                    j--;
                }
            }

            if (esq < j)
                Quicksort(array, esq, j);
            if (i < dir)
                Quicksort(array, i, dir);
        }
        static bool Maior(Candidato a, Candidato b)
        {
            if (a.nota > b.nota)
                return true;
            if (a.nota == b.nota && a.notaR > b.notaR)
                return true;
            return false;
        }

        static bool Menor(Candidato a, Candidato b)
        {
            if (a.nota < b.nota)
                return true;
            if (a.nota == b.nota && a.notaR < b.notaR)
                return true;
            return false;
        }


    }
    class Candidato
    {
        public string nome;
        public double notaR;
        public double notaM;
        public double notaL;
        public int cod1;
        public int cod2;
        public double nota;

        public override string ToString()
        {
            return $"{nome} - Média: {nota:F2} - Redação: {notaR}";
        }
        public double Nota
        {
            get { return nota; }
            set { nota = value; }
        }
        public int Cod2
        {
            get { return cod2; }
            set { cod2 = value; }
        }
        public int Cod1
        {
            get { return cod1; }
            set { cod1 = value; }
        }
        public double NotaL
        {
            get { return notaL; }
            set { notaL = value; }
        }
        public double NotaM
        {
            get { return notaM; }
            set { notaM = value; }
        }
        public double NotaR
        {
            get { return notaR; }
            set { notaR = value; }
        }
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

    }
    class Cursos
    {
        public string nomcurso;
        public int quantVagas;
        public List<Candidato> selecionados = new List<Candidato>();
        public List<Candidato> seleTotal = new List<Candidato>();
        public Fila espera = new Fila();

        public int QuantVagas
        {
            get { return quantVagas; }
            set { quantVagas = value; }
        }
        public string NomeC
        {
            get { return nomcurso; }
            set { nomcurso = value; }
        }
    }
    class Celula
    {
        private Candidato elemento;
        private Celula prox;
        public Celula(Candidato elemento)
        {
            this.elemento = elemento;
            this.prox = null;
        }
        public Celula()
        {
            this.elemento = null;
            this.prox = null;
        }
        public Celula Prox
        {
            get { return prox; }
            set { prox = value; }
        }
        public Candidato Elemento
        {
            get { return elemento; }
            set { elemento = value; }
        }
    }
}

class Fila
{
    private Celula primeiro, ultimo;
    public Fila()
    {
        primeiro = new Celula();
        ultimo = primeiro;
    }
    public void Inserir(Candidato x)
    {
        ultimo.Prox = new Celula(x);
        ultimo = ultimo.Prox;
    }
    public Candidato Remover()
    {
        if (primeiro == ultimo)
            throw new Exception("Erro!");
        Celula tmp = primeiro;
        primeiro = primeiro.Prox;
        Candidato elemento = primeiro.Elemento;
        tmp.Prox = null;
        tmp = null;
        return elemento;
    }
    public bool Vazia()
    {
        return primeiro == ultimo;
    }
}