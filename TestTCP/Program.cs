using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace TestTCP
{
    class Program
    {
        const int PORT_NO = 3839;
        const string SERVER_IP = "127.0.0.1";
        static void Main(string[] args)
        {
            object res = null;
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);
            TcpListener listener = new TcpListener(localAdd, PORT_NO);

            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();

                using (NetworkStream nwStream = client.GetStream())
                {
                    
                    byte[] buffer = new byte[client.ReceiveBufferSize];

                    
                    int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    
                    var datas = dataReceived.Split(' ');
                    var endpoints = datas[1].Split('/');
                    string controllerName = endpoints.Length >= 2 ? endpoints[1] : "home";
                    string methodName = (endpoints.Length >= 3 && endpoints[2] != "") ? endpoints[2] : "index";
                    controllerName += "Controller";

                    res = null;
                    var assembly = Assembly.GetExecutingAssembly();

                    //with foreach
                    /*foreach (var item in assembly.GetTypes())
                    {
                        if (item.Name.ToUpper() == controllerName.ToUpper())
                        {
                            var obj = Activator.CreateInstance(item);
                            foreach (var meth in item.GetMethods())
                            {
                                if (meth.Name.ToUpper() == methodName.ToUpper())
                                {
                                    res = meth.Invoke(obj, null);
                                    if (res != null)
                                    {
                                        Console.WriteLine("Success");
                                    }
                                }
                            }
                        }
                    }*/

                    var type = assembly.GetTypes().FirstOrDefault(x=>x.Name.ToUpper() == controllerName.ToUpper());
                    if (type != null)
                    {
                        var meth = type.GetMethods().FirstOrDefault(x => x.Name.ToUpper() == methodName.ToUpper());
                        var obj = Activator.CreateInstance(type);
                        if (meth != null)
                        {
                            res = meth.Invoke(obj, null);
                            if (res != null)
                            {
                                Console.WriteLine("Success");
                            }
                        }
                        else
                        {
                            res = "Action not found";
                        }
                    }
                    else
                    {
                        res = "Controller not found";
                    }

                    string sendMessage = "HTTP/1.1 200 OK\r\n" +
                            "Content-Type: text/html\r\n"
                         + "Connection: close\r\n\r\n" + "<p> RAHIB JAFAROV </p> <br> <h1>" + res?.ToString() + "</h1>";

                    var responseByteArray = Encoding.UTF8.GetBytes(sendMessage);
                    nwStream.Write(responseByteArray, 0, responseByteArray.Length);
                }
            }
        }
    }
}
