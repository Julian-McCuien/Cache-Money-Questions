using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache_Money_Questions
{
    internal class JSONReader // class is used to read and extract the data from the JSON file i created to hide the discord bot TOKEN and SQL connection string
    {
        public string token { get; set; }
        public string prefix { get; set; }
        public string connectionString { get; set; }

        public async Task ReadJSON() 
        {
            using (StreamReader sr = new StreamReader("config.json")) // this is the hidden file that contains the bot token in the output folder (bin > debug) does not upload to Github by default 
            {
                string json = await sr.ReadToEndAsync(); // reads the file start to finish
                JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(json); // DeSerialize the data (token) from the JSON file into the token and prefix properties  
                this.token = data.token;
                this.prefix = data.prefix;
                DatabaseConfig data2 = JsonConvert.DeserializeObject<DatabaseConfig>(json);
                this.connectionString = data2.ConnectionString;
            }
        }
    }
    internal sealed class JSONStructure // this class creates a stucture for how I want the "data" to be deserialized into (JSON "string" --> Object "JSONStucture") | sealed the class, I do not want the information accessed anywhere else
    {
        public string token { get; set; }
        public string prefix { get; set; }
    }
    internal sealed class DatabaseConfig
    {
        public string ConnectionString { get; set; }
    }

}
