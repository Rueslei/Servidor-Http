using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class ServidorHttp
{

    public TcpListener Controlador { get; set; }
    public int Porta { get; set; }

    public int QtdeRequests { get; set; }

    public ServidorHttp(int porta = 8080)
    {
        this.Porta = porta;

        try
        {
            this.Controlador = new TcpListener(IPAddress.Parse("127.0.0.1"), this.Porta);
            this.Controlador.Start();
            System.Console.WriteLine($"O servidor HTTP está rodando na porta {this.Porta}");
            System.Console.WriteLine($"Para acessar, basta colar o link no seu navegador http://localhost:{this.Porta}.");
            Task servidorHttpTask = Task.Run(() => AguardarRequests());
            servidorHttpTask.GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            System.Console.WriteLine($"Erro ao iniciar o servidor na porta{this.Porta}:\n{e.Message}");
        }
    }

    private async Task AguardarRequests()
    {
        while (true)
        {
            Socket conexao = await this.Controlador.AcceptSocketAsync();
            this.QtdeRequests++;
            Task task = Task.Run(() => ProcessarRequest(conexao, this.QtdeRequests));
        }
    }

    private void ProcessarRequest(Socket conexao, int numeroRequest)
    {
        System.Console.WriteLine($"Processando Request #{numeroRequest}...\n");
        if (conexao.Connected)
        {
            byte[] bytesRequisicao = new byte[1024];
            conexao.Receive(bytesRequisicao, bytesRequisicao.Length, 0);
            string textoRequisicao = Encoding.UTF8.GetString(bytesRequisicao).Replace((char)0, ' ').Trim();
            if (textoRequisicao.Length > 0)
            {
                System.Console.WriteLine($"\n{textoRequisicao}\n");
                conexao.Close();
            }
        }
        System.Console.WriteLine($"\n Request: {numeroRequest} finalizada.");
    }
}