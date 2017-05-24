using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Code1
{
    /// <summary>
    /// Para demonstrar o uso de interfaces - depois pode-se alterar para eventos.
    /// </summary>
    interface IControloAlteracoes
    {
        bool TemAlteracoes();
        void ResetTemAlteracoes();
    }
    /// <summary>
    /// Esta classe faz a gestão do tempo e da pontuação do jogo.
    /// </summary>
    class Pontuacao : IControloAlteracoes
    {
        DateTime vaiAcabarEm;
        public int TempoRestante
        {
            get
            {
                return (int)(vaiAcabarEm - DateTime.Now).TotalSeconds;
            }
            set
            {
                vaiAcabarEm = DateTime.Now.AddSeconds(value);
                tempoRestanteAnterior = TempoRestante;
            }
        }
        int pontos;
        public int Pontos
        {
            get
            {
                return pontos;
            }
            set
            {
                pontos = value;
                alterouPontuacao = true;
            }
        }

        int tempoRestanteAnterior;
        bool alterouPontuacao;

        public Pontuacao()
        {
            pontos = 0;
            vaiAcabarEm = DateTime.Now;
            tempoRestanteAnterior = 0;
            alterouPontuacao = true;
        }
        public bool TerminarJogo()
        {
            return Pontos <= Code1.PONTOS_MINIMOS || vaiAcabarEm < DateTime.Now;
        }

        public bool TemAlteracoes()
        {
            return alterouPontuacao || tempoRestanteAnterior != TempoRestante;
        }

        public void ResetTemAlteracoes()
        {
            alterouPontuacao = false;
            tempoRestanteAnterior = TempoRestante;
        }
    }
    /// <summary>
    /// Estas são os comandos possíveis para o utilizador.
    /// Falta escrever no ecrã uma legenda com eles.
    /// </summary>
    enum ComandoEnum
    {
        Nada,
        Terminar,
        MoverCursor,
        ForcarRolamento,
        Pausar, // falta implementar
        Recomecar  // falta implementar
    }
    /// <summary>
    /// Esta classe trata o input do utilizador e controla a posição do cursor.
    /// </summary>
    class Comando : IControloAlteracoes
    {
        public int PosicaoAtual { get; set; }

        bool alterouPosicao;

        public Comando()
        {
            alterouPosicao = true;
            PosicaoAtual = (int)Math.Floor((double)Code1.NUMERO_COLUNAS / 2);
        }
        private void mudaPosicao(int valor)
        {
            PosicaoAtual += valor;
            if (PosicaoAtual < 0)
            {
                PosicaoAtual = Code1.NUMERO_COLUNAS - 1;
            }
            else if (PosicaoAtual == Code1.NUMERO_COLUNAS)
            {
                PosicaoAtual = 0;
            }
            alterouPosicao = true;
        }
        public ComandoEnum Read()
        {
            ComandoEnum ce = ComandoEnum.Nada;
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);
                switch (cki.Key)
                {
                    case ConsoleKey.LeftArrow:
                        ce = ComandoEnum.MoverCursor;
                        mudaPosicao(-1);
                        break;
                    case ConsoleKey.RightArrow:
                        ce = ComandoEnum.MoverCursor;
                        mudaPosicao(+1);
                        break;
                    case ConsoleKey.DownArrow:
                        ce = ComandoEnum.ForcarRolamento;
                        break;
                    case ConsoleKey.Escape:
                        ce = ComandoEnum.Terminar;
                        break;
                }
            }
            return ce;
        }

        public bool TemAlteracoes()
        {
            return alterouPosicao;
        }

        public void ResetTemAlteracoes()
        {
            alterouPosicao = false;
        }
    }
    /// <summary>
    /// Tipos de ações que resultam das "cartas".
    /// </summary>
    enum CartaAcaoEnum
    {
        SomaPontos,
        ContaZero,
        TerminaJogo,
        AumentaTempo
    }
    /// <summary>
    /// Indica as características que cada tipo de carta deve definir.
    /// </summary>
    abstract class CartaTemplate
    {
        public abstract CartaAcaoEnum Acao { get; protected set; }
        public abstract int Pontos { get; protected set; }
        public abstract string Texto { get; protected set; }
        public abstract void AtualizarPontuacao(Pontuacao pontu);
    }
    /// <summary>
    /// Carta vazia, não faz nada nem soma pontos.
    /// </summary>
    class CartaVazia : CartaTemplate
    {
        public override CartaAcaoEnum Acao { get; protected set; }
        public override int Pontos { get; protected set; }
        public override string Texto { get; protected set; }
        public CartaVazia()
        {
            Acao = CartaAcaoEnum.SomaPontos;
            Pontos = 0;
            Texto = "";
        }
        public override void AtualizarPontuacao(Pontuacao pontu)
        {
            // aqui não tem que fazer nada
        }
    }
    /// <summary>
    /// Carta que aumenta ou diminui a pontuação num certo valor.
    /// </summary>
    class CartaPontos : CartaTemplate
    {
        public override CartaAcaoEnum Acao { get; protected set; }
        public override int Pontos { get; protected set; }
        public override string Texto { get; protected set; }
        public CartaPontos(int pontos)
        {
            Acao = CartaAcaoEnum.SomaPontos;
            Pontos = pontos;
            Texto = pontos.ToString();
        }
        public override void AtualizarPontuacao(Pontuacao pontu)
        {
            pontu.Pontos += Pontos;
        }
    }
    /// <summary>
    /// Carta que provoca uma ação com impacto na pontuação e/ou no curso do jogo.
    /// </summary>
    class CartaAcao : CartaTemplate
    {
        public override CartaAcaoEnum Acao { get; protected set; }
        public override int Pontos { get; protected set; }
        public override string Texto { get; protected set; }
        public CartaAcao(CartaAcaoEnum acao, string texto)
        {
            Acao = acao;
            Pontos = 0;
            Texto = texto;
        }
        public override void AtualizarPontuacao(Pontuacao pontu)
        {
            switch (Acao)
            {
                case CartaAcaoEnum.ContaZero:
                    pontu.Pontos = 0;
                    break;
                case CartaAcaoEnum.TerminaJogo:
                    pontu.TempoRestante = 0;
                    break;
                case CartaAcaoEnum.AumentaTempo:
                    pontu.TempoRestante += Code1.BONUS_TEMPO;
                    break;
            }
        }
    }
    /// <summary>
    /// Classe que tem uma lista das cartas possíveis e um método para
    /// sortear aleatoriamente uma carta.
    /// </summary>
    class CartasPossiveis
    {
        CartaTemplate[] cartas;
        Random r;
        private CartasPossiveis()
        {
            r = new Random();
            cartas = new CartaTemplate[20];
            cartas[0] = new CartaPontos(1);
            cartas[1] = new CartaPontos(-1);
            cartas[2] = new CartaPontos(5);
            cartas[3] = new CartaPontos(-5);
            cartas[4] = new CartaPontos(10);
            cartas[5] = new CartaPontos(-10);
            cartas[6] = new CartaPontos(15);
            cartas[7] = new CartaPontos(-15);
            cartas[8] = new CartaPontos(25);
            cartas[9] = new CartaPontos(-25);
            cartas[10] = new CartaVazia();
            cartas[11] = new CartaVazia();
            cartas[12] = new CartaVazia();
            cartas[13] = new CartaAcao(CartaAcaoEnum.ContaZero, "Z");
            cartas[14] = new CartaAcao(CartaAcaoEnum.TerminaJogo, "FIM");
            cartas[15] = new CartaAcao(CartaAcaoEnum.AumentaTempo, "+T");
            cartas[16] = new CartaVazia();
            cartas[17] = new CartaVazia();
            cartas[18] = new CartaVazia();
            cartas[19] = new CartaVazia();
        }
        private CartaTemplate nextRandom()
        {
            return cartas[r.Next(0, 19)];
        }
        static CartasPossiveis unicaInstancia = null;
        public static CartaTemplate GetCartaRandom()
        {
            if (unicaInstancia == null)
            {
                unicaInstancia = new CartasPossiveis();
            }
            return unicaInstancia.nextRandom();
        }
    }
    /// <summary>
    /// Matriz onde se colocam as cartas sorteadas e que se vai alterando
    /// automaticamente, apaga a linha de baixo e acrescenta uma linha em cima.
    /// </summary>
    class MatrizJogo : IControloAlteracoes
    {
        DateTime proximoRolamento;
        CartaTemplate[,] matriz;
        CartaTemplate[] ultimaLinhaDescartada;
        bool alterouMatriz;
        public MatrizJogo()
        {
            alterouMatriz = true;
            ultimaLinhaDescartada = new CartaTemplate[Code1.NUMERO_COLUNAS];
            matriz = new CartaTemplate[Code1.NUMERO_LINHAS, Code1.NUMERO_COLUNAS];
            for (int j = 0; j < Code1.NUMERO_LINHAS; j++)
            {
                for (int k = 0; k < Code1.NUMERO_COLUNAS; k++)
                {
                    matriz[j, k] = CartasPossiveis.GetCartaRandom();
                }
            }
            proximoRolamento = DateTime.Now.AddMilliseconds(Code1.INTERVALO_ROLA);
        }
        public bool RolaParaBaixo(bool forcar)
        {
            if (DateTime.Now > proximoRolamento || forcar)
            {
                // guarda a última linha descartada                
                for (int k = 0; k < Code1.NUMERO_COLUNAS; k++)
                {
                    ultimaLinhaDescartada[k] = matriz[Code1.NUMERO_LINHAS - 1, k];
                }

                // "move" as linhas de cima para baixo
                for (int j = Code1.NUMERO_LINHAS - 1; j > 0; j--)
                {
                    for (int k = 0; k < Code1.NUMERO_COLUNAS; k++)
                    {
                        matriz[j, k] = matriz[j - 1, k];
                    }
                }

                // gera valores para a linha de cima
                for (int k = 0; k < Code1.NUMERO_COLUNAS; k++)
                {
                    matriz[0, k] = CartasPossiveis.GetCartaRandom();
                }

                proximoRolamento = DateTime.Now.AddMilliseconds(Code1.INTERVALO_ROLA);
                alterouMatriz = true;
                return true;
            }
            return false;
        }
        public CartaTemplate GetMatriz(int linha, int coluna)
        {
            return matriz[linha, coluna];
        }
        public CartaTemplate GetLinhaDescartada(int coluna)
        {
            return ultimaLinhaDescartada[coluna];
        }

        public bool TemAlteracoes()
        {
            return alterouMatriz;
        }

        public void ResetTemAlteracoes()
        {
            alterouMatriz = false;
        }
    }
    /// <summary>
    /// Loop principal e escrita para o ecrã
    /// </summary>
    class Code1 
    {
        public const int PONTOS_MINIMOS = -100;
        public const int NUMERO_COLUNAS = 8;
        public const int NUMERO_LINHAS = 5;
        public const int INTERVALO_ROLA = 5000; // milissegundos
        public const int TEMPO_INICIAL = 100; // segundos
        public const int BONUS_TEMPO = 10; // segundos
        public const int DESCANSO_CPU = 80;

        static void Main(string[] args)
        {
            MatrizJogo mat = new MatrizJogo();
            Comando comm = new Comando();
            Pontuacao pontu = new Pontuacao()
            {
                TempoRestante = TEMPO_INICIAL
            };
            //
            escreveBaseEcra();
            atualizaEcra(mat, comm, pontu);
            //
            do
            {
                ComandoEnum ce = comm.Read();
                atualizaEcra(mat, comm, pontu);
                //
                if (ce == ComandoEnum.Terminar)
                {
                    break;
                }
                //
                if (mat.RolaParaBaixo(ce == ComandoEnum.ForcarRolamento))
                {
                    mat.GetLinhaDescartada(comm.PosicaoAtual).AtualizarPontuacao(pontu);
                }
                atualizaEcra(mat, comm, pontu); 
                //
                Thread.Sleep(DESCANSO_CPU);
            }
            while (pontu.TerminarJogo() == false);
            //
            Console.SetCursorPosition(0, 23); // por causa do press any key to continue...
            Console.ReadKey();
        }

        static void escreveBaseEcra()
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.SetCursorPosition(8, 2);
            Console.Write(new String('-', 50));
            for (int i = 3; i < 14; i++)
            {
                Console.SetCursorPosition(8, i);
                Console.Write(":");
                Console.SetCursorPosition(57, i);
                Console.Write(":");
            }
            Console.SetCursorPosition(8, 14);
            Console.Write(new String('-', 50));
        }
        static void atualizaEcra(MatrizJogo mj, Comando c, Pontuacao p)
        {
            if (mj.TemAlteracoes())
            {
                int linha = 4;
                for (int j = 0; j < Code1.NUMERO_LINHAS; j++)
                {
                    Console.SetCursorPosition(25, linha + j);
                    for (int k = 0; k < Code1.NUMERO_COLUNAS; k++)
                    {
                        Console.Write("{0} ", mj.GetMatriz(j, k).Texto.PadLeft(3));
                    }
                }
                mj.ResetTemAlteracoes();
            }
            if (c.TemAlteracoes())
            {
                int linha = 10;
                Console.SetCursorPosition(10, linha);
                Console.Write("Posição atual: {0}", new String('_', 31));
                Console.SetCursorPosition(25 + (c.PosicaoAtual * 4), 10);
                Console.Write("#{0}#", c.PosicaoAtual + 1);
                c.ResetTemAlteracoes();
            }
            if (p.TemAlteracoes())
            {
                int linha = 12;
                Console.SetCursorPosition(10, linha);
                Console.Write("Pontuação: {0}    Tempo restante: {1}    ", p.Pontos, p.TempoRestante);
                p.ResetTemAlteracoes();
            }

        }
    }
}
