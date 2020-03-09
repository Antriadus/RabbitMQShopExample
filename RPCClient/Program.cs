using RPCClientNamespace;
using System;


public class Rpc
{
    public static void Main()
    {
        var rpcClient = new RpcClient();

        Console.WriteLine("Będę używał RPC aby wygenerować raporty");
        Console.WriteLine();
        var response = rpcClient.CallForAccountReport("wp.pl", "xyz.com");
        foreach (var i in response)
        {
            Console.WriteLine($"{i.Key}\t{i.Value}\tużytkowników");
        }
        Console.ReadLine();
        rpcClient.Close();
    }
}
