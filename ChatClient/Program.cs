using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class ChatClient
{


    static void Main(string[] args)
    {
        Console.WriteLine("connecting to server");
        //phase1: creer tcpclient jdid liaytconnecta l serveur
        TcpClient client = new TcpClient();//detecter ga3 les ipadress likijiw lport 8000
        client.Connect("127.0.0.1", 8000);// Connect to the server at localhost (127.0.0.1) on port 8080
        Console.WriteLine("connecté au serveur");
        //phase2: nchdo network stream mn serveur(message li jay mno)
        NetworkStream stream = client.GetStream();

        // Check if the user is already registered
        Console.WriteLine("1-registration \n 2-conexion ");
        string choice = Console.ReadLine();
        if (choice == "1")
        {
            Register(stream);

        }
        else if (choice == "2")
        {
            Authenticate(stream);
        }
        else
        {
            Console.WriteLine("choix invalide a  l3jel ");
            return;
        }

        Thread reiciveThread = new Thread(() => RecieveMessages(stream));
        reiciveThread.Start();
        while (true)
        {//ktb message l serveur
            string messagetosend = Console.ReadLine();
            byte[] data = Encoding.UTF8.GetBytes(messagetosend);
            stream.Write(data, 0, data.Length);
        }
    }

    static void Register(NetworkStream stream)
    {
        Console.WriteLine("enter username:");
        string username = Console.ReadLine();
        Console.WriteLine("enter password");
        string password = Console.ReadLine();

        string registerMessage = $"REGISTER {username}:{password}";
        byte[] data = Encoding.UTF8.GetBytes(registerMessage);
        stream.Write(data, 0, data.Length);
        //attends la reponse du serveur
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Server" + response);
        if (response != "Registration successful")
        {
            Console.WriteLine("Registration failed,khrj ");
            Environment.Exit(0);
        }
    }

    //connexion 
    static void Authenticate(NetworkStream stream)
    {
        Console.WriteLine("enter username");
        string username = Console.ReadLine();
        Console.WriteLine("enter password");
        string password = Console.ReadLine();
        string authMessage = $"AUTHENTIFICATE:{username}:{password}";
        byte[] data = Encoding.UTF8.GetBytes(authMessage);
        stream.Write(data, 0, data.Length);

        // Wait for server response
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Server: " + response);

        if (response != "Authentication successful")
        {
            Console.WriteLine("Authentication failed. Exiting...");
            Environment.Exit(0);
        }
    }

    //methode bch trecever data mlserveur
   static void RecieveMessages(NetworkStream stream)
{
    byte[] buffer = new byte[1024];
    try
    {
        while (true)
        {
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead == 0) break; // Server disconnected
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine(message);
        }
    }
    catch (System.IO.IOException ex)
    {
        Console.WriteLine($"Disconnected from server: {ex.Message}");
    }
    finally
    {
        Console.WriteLine("Connection closed.");
    }
}


}
