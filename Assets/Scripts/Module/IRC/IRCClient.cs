using Koapower.KoafishTwitchBot.Module.IRC.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace Koapower.KoafishTwitchBot.Module.IRC
{
    public class IRCClient
    {
        public IRCClient(string host, int port)
        {
            this.hostName = host;
            this.port = port;
        }
        public readonly string hostName;
        public readonly int port;
        private static readonly Regex msgRegex = new Regex(":(.+?)!.+?:(.*?)$");

        internal event Action<IMessage> onMessageRecieved;
        protected TcpClient tcpClient;
        protected NetworkStream ns;
        protected StreamReader sr;
        protected StreamWriter sw;
        protected Thread recieve, send;
        private bool isWorking;
        Queue<string> messageQueue = new Queue<string>();

        public string IRCName { get; private set; }
        public string IRCPassword { get; private set; }
        public string IRCNickname { get; private set; }
        public State status { get; private set; }

        public void SetUserInfo(string name, string password, string nickname)
        {
            IRCName = name;
            IRCPassword = password;
            IRCNickname = nickname;
        }

        public void StartWork()
        {
            if (isWorking) return;
            isWorking = true;

            var success = ConnectAndLogin();
            if (success)
            {
                status = State.Connected;
                recieve = new Thread(RecieveLoop);
                send = new Thread(SendLoop);
                recieve.Start();
                send.Start();

                SendPrivateMessage(new IRCMessage(IRCNickname, "IRC Client Connected"));
            }
            else
            {
                Debug.LogError("IRC Client failed start work!");
                isWorking = false;
            }

            Debug.Log("IRC Client start work");
        }

        private bool ConnectAndLogin()
        {
            status = State.Connecting;
            tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(hostName, port);
                if (!tcpClient.Connected)
                {
                    throw new Exception("Network error!");
                }
                ns = tcpClient.GetStream();
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);

                IRCLogin();

                Debug.Log("IRC Client login success!");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        private void IRCLogin()
        {
            sw.WriteLine($"PASS {IRCPassword}");
            sw.WriteLine($"USER {IRCName} 1 * : {IRCName}");
            sw.WriteLine($"NICK {IRCNickname}");
            sw.Flush();
        }

        public void StopWork()
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                sw.Write("QUIT");
                sw.Flush();
            }
            isWorking = false;

            Debug.Log("IRC Client stop work");
        }

        public void Restart()
        {
            StopWork();
            StartWork();
        }

        private void RecieveRawMessage(string msg)
        {
            if (!msg.Contains(@"PRIVMSG "))
            {
                //处理非对话消息
                if (msg.StartsWith("PING "))
                    SendRawMessage(msg.Replace(@"PING", @"PONG"));
            }
            else
            {
                Match match = msgRegex.Match(msg);
                string nick = match.Groups[1].Value;
                string rawmsg = match.Groups[2].Value;
                onMessageRecieved?.Invoke(new IRCMessage(nick, rawmsg));
            }
        }

        internal void SendPrivateMessage<T>(T message) where T : IMessage
        {
            SendRawMessage($"PRIVMSG {message.User} :{message.Message}");
        }

        private void SendRawMessage(string msg)
        {
            messageQueue.Enqueue(msg);
        }

        private void RecieveLoop()
        {
            try
            {
                while (isWorking)
                {
                    Thread.Sleep(1);

                    if ((tcpClient.Client.Poll(20, SelectMode.SelectRead)) && (tcpClient.Client.Available == 0))
                    {
                        Debug.LogError("[Osu!IRC] Network interrupted");
                        ConnectAndLogin();
                        continue;
                    }

                    if (ns.DataAvailable)
                    {
                        string message = sr.ReadLine();

                        RecieveRawMessage(message);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Osu!IRC] Reciever occured error: " + e.Message);
                status = State.Disconnected;
            }

            Debug.Log("[Osu!IRC] Reciever thread finish");
        }

        private void SendLoop()
        {
            try
            {
                while (isWorking)
                {
                    Thread.Sleep(1);
                    if (messageQueue.Count > 0)
                    {
                        if (!tcpClient.Connected)
                        {
                            //等Reciever线程处理即可
                            continue;
                        }

                        string message = string.Empty;
                        lock (messageQueue)
                        {
                            message = messageQueue.Dequeue();
                        }
                        if (message == string.Empty) continue;
                        if (!tcpClient.Connected)
                        {
                            isWorking = false;
                            status = State.Disconnected;
                            continue;
                        }
                        sw.WriteLine(message);
                        sw.Flush();
                        message = string.Empty;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Osu!IRC] Sender occured error: " + e.Message);
                status = State.Disconnected;
            }

            Debug.Log("[Osu!IRC] Sender thread finish");
        }
    }
}
