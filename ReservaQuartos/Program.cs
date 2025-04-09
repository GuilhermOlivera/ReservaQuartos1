using System;
using System.Collections.Generic;

namespace ReservaQuartos
{
    public enum TipoQuarto
    {
        Single,
        Duplo,
        Suite
    }

    public enum StatusQuarto
    {
        Disponivel,
        Ocupado,
        EmManutencao
    }

    public class Quarto
    {
        public int Numero { get; set; }
        public TipoQuarto Tipo { get; set; }
        public decimal PrecoNoite { get; set; }
        public StatusQuarto Status { get; set; }

        public Quarto(int numero, TipoQuarto tipo, decimal precoNoite)
        {
            Numero = numero;
            Tipo = tipo;
            PrecoNoite = precoNoite;
            Status = StatusQuarto.Disponivel;
        }

        public override string ToString()
        {
            return $"{Numero} - {Tipo} - R${PrecoNoite}/Noite - {Status}";
        }
    }

    public class Reserva
    {
        public Quarto QuartoReservado { get; set; }
        public string NomeHospede { get; set; }
        public string DocumentoHospede { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public int NumeroNoites => (CheckOut - CheckIn).Days;

        public decimal CalcularCusto()
        {
            decimal custo = QuartoReservado.PrecoNoite * NumeroNoites;

            if (NumeroNoites > 5)
            {
                custo *= 0.9m;
            }

            return custo;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            List<Quarto> quartos = new List<Quarto>
            {
                new Quarto(101, TipoQuarto.Single, 150),
                new Quarto(102, TipoQuarto.Duplo, 220),
                new Quarto(103, TipoQuarto.Suite, 350)
            };
            List<Reserva> reservas = new List<Reserva>();

            while (true)
            {
                Console.WriteLine("MENU PRINCIPAL \nEscolha uma das opções: \n1-Registrar uma reserva | 2-Gerenciar status de um quarto | 3-Sair");
                int menu;
                if (!int.TryParse(Console.ReadLine(), out menu))
                {
                    Console.WriteLine("Entrada inválida. Tente novamente.");
                    continue;
                }

                switch (menu)
                {
                    case 1:
                        Console.WriteLine("Você entrou no Registros de Reserva \nQuartos disponíveis:");
                        foreach (var quarto in quartos)
                        {
                            if (quarto.Status == StatusQuarto.Disponivel)
                            {
                                Console.WriteLine(quarto);
                            }
                        }

                        Console.WriteLine("Digite o número do quarto que deseja reservar:");
                        int numQuartoReserva;
                        if (!int.TryParse(Console.ReadLine(), out numQuartoReserva))
                        {
                            Console.WriteLine("Entrada inválida. Tente novamente.");
                            continue;
                        }

                        var quartoEscolhido = quartos.Find(q => q.Numero == numQuartoReserva && q.Status == StatusQuarto.Disponivel);

                        if (quartoEscolhido == null)
                        {
                            Console.WriteLine("Esse quarto não está disponível para reserva.");
                            break;
                        }

                        Console.WriteLine("Digite o nome do hóspede:");
                        string nomeHospede = Console.ReadLine();

                        Console.WriteLine("Digite o documento do hóspede:");
                        string documentoHospede = Console.ReadLine();

                        DateTime checkIn, checkOut;
                        Console.WriteLine("Digite a data de check-in (dd/MM/yyyy):");
                        while (!DateTime.TryParse(Console.ReadLine(), out checkIn))
                        {
                            Console.WriteLine("Data inválida. Tente novamente.");
                        }

                        Console.WriteLine("Digite a data de check-out (dd/MM/yyyy):");
                        while (!DateTime.TryParse(Console.ReadLine(), out checkOut) || checkOut <= checkIn)
                        {
                            Console.WriteLine("Data de check-out inválida. A data de check-out deve ser posterior à de check-in.");
                        }

                        Reserva reserva = new Reserva
                        {
                            QuartoReservado = quartoEscolhido,
                            NomeHospede = nomeHospede,
                            DocumentoHospede = documentoHospede,
                            CheckIn = checkIn,
                            CheckOut = checkOut
                        };

                        reservas.Add(reserva);

                        quartoEscolhido.Status = StatusQuarto.Ocupado;
                        Console.WriteLine($"Reserva registrada com sucesso para o quarto {quartoEscolhido.Numero}. Custo total: R${reserva.CalcularCusto():F2}");
                        break;

                    case 2:
                        Console.WriteLine("Você entrou no Gerenciamento de Status de Quartos!");
                        Console.WriteLine("Escolha uma das opções: \n1- Alterar para ocupado \n2- Alterar para disponível \n3- Alterar para manutenção");
                        int opcaoStatus;
                        if (!int.TryParse(Console.ReadLine(), out opcaoStatus))
                        {
                            Console.WriteLine("Entrada inválida. Tente novamente.");
                            continue;
                        }

                        Console.WriteLine("Digite o número do quarto para alterar o status:");
                        int numQuartoStatus;
                        if (!int.TryParse(Console.ReadLine(), out numQuartoStatus))
                        {
                            Console.WriteLine("Entrada inválida. Tente novamente.");
                            continue;
                        }

                        var quartoStatus = quartos.Find(q => q.Numero == numQuartoStatus);

                        if (quartoStatus == null)
                        {
                            Console.WriteLine("Número de quarto inexistente.");
                            break;
                        }

                        switch (opcaoStatus)
                        {
                            case 1:
                                if (quartoStatus.Status == StatusQuarto.Disponivel)
                                {
                                    quartoStatus.Status = StatusQuarto.Ocupado;
                                    Console.WriteLine($"O {quartoStatus.Numero} agora está ocupado.");
                                }
                                else
                                {
                                    Console.WriteLine($"O {quartoStatus.Numero} não está disponível para ser ocupado.");
                                }
                                break;
                            case 2:
                                if (quartoStatus.Status == StatusQuarto.Ocupado || quartoStatus.Status == StatusQuarto.EmManutencao)
                                {
                                    quartoStatus.Status = StatusQuarto.Disponivel;
                                    Console.WriteLine($"O {quartoStatus.Numero} agora está disponível.");
                                }
                                else
                                {
                                    Console.WriteLine($"O {quartoStatus.Numero} já está disponível.");
                                }
                                break;
                            case 3:
                                if (quartoStatus.Status == StatusQuarto.Disponivel || quartoStatus.Status == StatusQuarto.Ocupado)
                                {
                                    quartoStatus.Status = StatusQuarto.EmManutencao;
                                    Console.WriteLine($"O {quartoStatus.Numero} foi colocado em manutenção.");
                                }
                                break;
                            default:
                                Console.WriteLine("Opção inválida.");
                                break;
                        }
                        break;

                    case 3:
                        Console.WriteLine("Saindo do MENU.");
                        return;

                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }

                Console.WriteLine("\nSTATUS ATUAL:");
                foreach (var quarto in quartos)
                {
                    Console.WriteLine(quarto);
                }

                Console.WriteLine("\nPressione qualquer tecla para voltar ao MENU.");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
