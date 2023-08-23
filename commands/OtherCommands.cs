using Cache_Money_Questions.other;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cache_Money_Questions.commands
{
    public class OtherCommands : BaseCommandModule
    {
        [Command("Cardgame")]
        public async Task CardGame(CommandContext ctx)
        {
            var userCard = new CardSystem();


            var userCardEmbed = new DiscordEmbedBuilder
            {

                Title = $"Your card is {userCard.SelectedCard}",
                Color = DiscordColor.Lilac,
            };

            await ctx.Channel.SendMessageAsync(embed: userCardEmbed);

            var botCard = new CardSystem();

            var botCardEmbed = new DiscordEmbedBuilder
            {
                Title = $"I drew {botCard.SelectedCard}",
                Color = DiscordColor.Orange,

            };

            await ctx.Channel.SendMessageAsync(embed: botCardEmbed);

            if (userCard.SelectedNum > botCard.SelectedNum)
            {
                var winMessage = new DiscordEmbedBuilder
                {
                    Title = $"YOU BEAT ME {ctx.User.Username}!",
                    Description = "run it back!",
                    Color = DiscordColor.Green,

                };
                await ctx.Channel.SendMessageAsync(winMessage);
            }
            else
            {
                var lostMessage = new DiscordEmbedBuilder
                {
                    Title = $"HAHA! Better luck next time {ctx.User.Username}!",
                    Description = $"ayoo {ctx.Guild.EveryoneRole.Mention} come getcha mans! ",
                    Color = DiscordColor.DarkRed,

                };
                await ctx.Channel.SendMessageAsync(lostMessage);
            }
        }
        [Command("Poll")]
        public async Task PollCommand(CommandContext ctx, int TimeLimit,  string Opt1, string Opt2, string Opt3, string Opt4, params string[] Question)
        {
            var interactvity = ctx.Client.GetInteractivity(); // import the interactivity module
            TimeSpan timer = TimeSpan.FromSeconds(TimeLimit); // convert the time limit to actual time

            DiscordEmoji[] optionEmojis = 
            { DiscordEmoji.FromName(ctx.Client, ":one:", false),
            DiscordEmoji.FromName(ctx.Client, ":two:", false),
            DiscordEmoji.FromName(ctx.Client, ":three:", false),
            DiscordEmoji.FromName(ctx.Client, ":four:", false)};

            string optionString =
                optionEmojis[0] + " | " + Opt1 + "\n" +
                optionEmojis[1] + " | " + Opt2 + "\n" +
                optionEmojis[2] + " | " + Opt3 + "\n" +
                optionEmojis[3] + " | " + Opt4;



            var pollMessage = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Azure,
                Title=(string.Join(" ",Question)),
                Description = optionString

            };
            var putReactOn = await ctx.Channel.SendMessageAsync(embed:pollMessage); // storing await command to add reactions

            foreach ( var emoji in optionEmojis)
            {
                await putReactOn.CreateReactionAsync(emoji); // allows reaction on all the emojis
            }
            try
            {
                var result = await interactvity.CollectReactionsAsync(putReactOn, timer); // specify the message we are collecting reactions from
                int count1 = 0; int count2 = 0; int count3 = 0; int count4 = 0;
                foreach (var emoji in result)
                {
                    if (emoji.Emoji == optionEmojis[0])
                    {
                        count1++;
                    }
                    else if (emoji.Emoji == optionEmojis[1])
                    {
                        count2++;
                    }
                    else if (emoji.Emoji == optionEmojis[2])
                    {
                        count3++;
                    }
                    else if (emoji.Emoji == optionEmojis[3])
                    {
                        count4++;
                    }
                }
                int totalVotes = count1 + count2 + count3 + count4;

                string ResultsString =
                optionEmojis[0] + ": " + count1 + "Votes \n" +
                optionEmojis[1] + ": " + count2 + "Votes \n" +
                optionEmojis[2] + ": " + count3 + "Votes \n" +
                optionEmojis[3] + ": " + count4 + "Votes \n\n" +
                "The total number of votes is: " + totalVotes;

                var resultsMessage = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Blurple,
                    Title = "Results Of Pole",
                    Description = ResultsString
                };
                await ctx.Channel.SendMessageAsync(embed: resultsMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            







        }
    }
}
