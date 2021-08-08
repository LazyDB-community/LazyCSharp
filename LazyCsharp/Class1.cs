using System;
using System.Collections.Generic;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Linq;

namespace LazyCsharp
{
    public class Callback
    {
        public Action<Newtonsoft.Json.Linq.JToken> success;
        public Action<Newtonsoft.Json.Linq.JToken> fail;
    }

    public class Database
    {
        public string addr;
        public int port;
        public int id = 0;
        public string lazy_sep = "\t\n\'lazy_sep\'\t\n";
        public Dictionary<int, Callback> callbacks = new Dictionary<int, Callback> { };
        public WebSocket ws;
        public List<string> messageQueue = new List<string> { };

        public class ReceiveObject
        {
            public bool s { get; set; }
            public int id { get; set; }
            public object r { get; set; }
        }

        public static class Interval
        {
            public static System.Timers.Timer Set(System.Action action, int interval)
            {
                var timer = new System.Timers.Timer(interval);
                timer.Elapsed += (s, e) => {
                    timer.Enabled = false;
                    action();
                    timer.Enabled = true;
                };

                timer.Enabled = true;
                return timer;
            }

            public static void Stop(System.Timers.Timer timer)
            {
                timer.Stop();
                timer.Dispose();
            }
        }

        public Database(string addr, int port, Action<object> onconnect, Action<Object> onclose)
        {
            this.addr = addr;
            this.port = port;
            this.ws = new WebSocket($"ws://{addr}:{port}");
            this.ws.OnOpen += (sender, e) =>
            {
                onconnect(e);
            };

            this.ws.OnClose += (sender, e) =>
            {
                onclose(e);
            };

            this.ws.OnMessage += (sender, e) =>
            {
                var messages = e.Data.Split("|");
                for (int i = 0; i < messages.Length; i++)
                {
                    var msg = messages[i];
                    Newtonsoft.Json.Linq.JObject receivedMessage = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(msg);
                    Newtonsoft.Json.Linq.JToken newReceivedMessage = Newtonsoft.Json.Linq.JToken.FromObject(receivedMessage.ToString());

                    if (receivedMessage.Value<bool>("s"))
                    {
                        this.callbacks[receivedMessage.Value<int>("id")].success.Invoke(receivedMessage.GetValue("r"));
                    }
                    else
                    {
                        this.callbacks[receivedMessage.Value<int>("id")].fail.Invoke(receivedMessage.GetValue("r"));
                    }
                }
            };

            Interval.Set(sendQueue, 100);

            this.ws.Connect();
        }

        public void sendQueue()
        {
            if (this.messageQueue.Count > 0 && this.ws.ReadyState.ToString() == "Open")
            {
                this.ws.Send(string.Join("|", this.messageQueue));
            }

            this.messageQueue = new List<string> { };
        }

        [DataContract]
        public class Send
        {
            [DataMember(Name = "c")]
            public string c { get; set; }

            [DataMember(Name = "id")]
            public int id { get; set; }

            [DataMember(Name = "a")]
            public object a { get; set; }
        }

        public void send(string name, Object args, Callback callback)
        {
            int id = ++this.id;

            Send send = new Send
            {
                c = name,
                id = id,
                a = args
            };

            string message = JsonConvert.SerializeObject(send);
            if (!message.Contains("|"))
            {
                this.messageQueue.Add(message);
                this.callbacks.Add(id, callback);
            }
        }

        public void connect(string email, string password, Callback callback)
        {
            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("email", email);
            args.Add("password", password);

            this.send("connect", args, callback);
        }

        public void register(string email, string password, string username, string full_name, Callback callback)
        {
            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("email", email);
            args.Add("password", password);
            args.Add("username", username);
            args.Add("full_name", full_name);

            this.send("register", args, callback);
        }

        public void create(string keyPath, Newtonsoft.Json.Linq.JObject value, Callback callback)
        {
            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("value", value);

            this.create_process(keyPath, args, callback);
        }

        public void create(string keyPath, Newtonsoft.Json.Linq.JToken value, Callback callback)
        {
            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("value", value);

            this.create_process(keyPath, args, callback);
        }

        public void create_process(string keyPath, Newtonsoft.Json.Linq.JObject args, Callback callback)
        {
            var key = keyPath.Split("/");

            args.Add("keyPath", Newtonsoft.Json.Linq.JToken.FromObject(key));
            args.Add("w", true);

            this.send("create", args, callback);
        }

        public void watch(string keyPath, string command, Callback callback)
        {
            /*
             * IN DEVELOPPEMENT
             */
            /*var key = keyPath.Split("/").Where(x => string.IsNullOrEmpty(x) == false).ToArray();*/
            var key = keyPath.Split("/");

            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("keyPath", Newtonsoft.Json.Linq.JToken.FromObject(key));
            args.Add("command", command);

            this.send("watch", args, callback);
        }

        public void get(string keyPath, Callback callback)
        {
            var key = keyPath.Split("/");

            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("keyPath", Newtonsoft.Json.Linq.JToken.FromObject(key));

            this.send("get", args, callback);
        }

        public void delete(string keyPath, Callback callback)
        {
            var key = keyPath.Split("/");

            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("keyPath", Newtonsoft.Json.Linq.JToken.FromObject(key));

            this.send("delete", args, callback);
        }

        public void keys(string keyPath, string filter, Callback callback)
        {
            var key = keyPath.Split("/");

            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("keyPath", Newtonsoft.Json.Linq.JToken.FromObject(key));
            args.Add("filter", filter);

            this.send("keys", args, callback);
        }

        public void update(string keyPath, string value, Callback callback)
        {
            var key = keyPath.Split("/");

            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("keyPath", Newtonsoft.Json.Linq.JToken.FromObject(key));
            args.Add("value", value);
            args.Add("w", true);

            this.send("update", args, callback);
        }

        public void exists(string keyPath, Callback callback)
        {
            var key = keyPath.Split("/");

            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("keyPath", Newtonsoft.Json.Linq.JToken.FromObject(key));

            this.send("exist", args, callback);
        }

        public void size(string keyPath, Callback callback)
        {
            var key = keyPath.Split("/");

            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("keyPath", Newtonsoft.Json.Linq.JToken.FromObject(key));

            this.send("size", args, callback);
        }

        /*
         * IN DEVELOPPEMENT
         */
        public void sort(string keyPath, String character, int number, int count, int start, String order, Callback callback)
        {
            var key = keyPath.Split("/").Where(x => string.IsNullOrEmpty(x) == false).ToArray();

            Newtonsoft.Json.Linq.JObject split = new Newtonsoft.Json.Linq.JObject { };
            split.Add("char", character);
            split.Add("num", number);

            Newtonsoft.Json.Linq.JObject result = new Newtonsoft.Json.Linq.JObject { };
            result.Add("count", count);
            result.Add("start", start);
            result.Add("order", order);

            Newtonsoft.Json.Linq.JObject args = new Newtonsoft.Json.Linq.JObject { };
            args.Add("keyPath", Newtonsoft.Json.Linq.JToken.FromObject(key));
            args.Add("split", split);
            args.Add("result", result);
            args.Add("order", order);

            Console.WriteLine(args);

            this.send("sort", args, callback);
        }
    }

    /*
    * FOR DEBUG
    */
    /*class Program
    {
        static void Main()
        {
            Database db = new Database("eu.indivis.cloud", 42600, delegate (Object s) {
                Console.WriteLine("LazyDB is start!");
            }, delegate (Object s) {
                Console.WriteLine("LazyDB is stop!");
            });

            Callback callback = new Callback();
            callback.success = delegate (Newtonsoft.Json.Linq.JToken s) {
                Console.WriteLine(s);
            };
            callback.fail = delegate (Newtonsoft.Json.Linq.JToken s) {
                Console.WriteLine(s);
            };
            db.connect("arthur", "test", callback);

            //string sr = File.ReadAllText("C:\\Users\\arthu\\Desktop\\Sans titre.png");
            //db.create("/images/public/3.png", sr, callback);

            db.watch("/users", "update", callback);

            Console.ReadKey();
        }
    }*/
}
