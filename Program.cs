using Cache_Money_Questions.commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;


namespace Cache_Money_Questions
{
    public sealed class Program
    {
        private static IServiceProvider _services;
        

        private static DiscordClient Client { get; set; } // Dsharp NuGet package Adding Discord API
        //public static InteractivityExtension Interactivity { get;  private set; }
        private static SlashCommandsConfiguration SlashCommandsConfiguration { get; set; }
        private static CommandsNextExtension Commands { get ; set; } // Commands for the BOT
        public static async Task Main(string[] args)
        {
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON(); // jsonReader contains token info

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All, //this is confirming bot settings selected on discord's developer portal
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true, // if it crashes attempt to reconnect 
            };

            Client = new DiscordClient(discordConfig); // creates an instance, Initializing the client with the config

            //Setting our default timeout for Interactivity based commands
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            Client.Ready += Client_Ready;
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] {jsonReader.prefix }, // assigning the prefix from the JSON FILE
                EnableMentionPrefix = true, // enables the prefix
                EnableDms = true,
                EnableDefaultHelp = false,
                 
            };

            Commands = Client.UseCommandsNext(commandsConfig); // Enabling commands  
            var slashCommandsConfig = Client.UseSlashCommands();
           
            //slash commands
             slashCommandsConfig.RegisterCommands<SlashCommandQuestions>();
            
            //prefix commands
            Commands.RegisterCommands<QuestionCommands>();
            Commands.RegisterCommands<OtherCommands>();
            
            await Client.ConnectAsync(); // This connects the bot 
            await Task.Delay(-1); // This ensures the bot will keep running as long as the application is running 
        }

        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            // This is the event that occurs when the bot has successfully connected as is operational 
            return Task.CompletedTask;
        }
    }
}