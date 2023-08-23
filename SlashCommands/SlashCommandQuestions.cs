using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System.Threading.Tasks;
using System;

public class SlashCommandQuestions : ApplicationCommandModule // needed for slash commands
{
    [SlashCommandAttribute("random_question", "get a random question", true)]
    public async Task CompletelyRandomQuestion(InteractionContext ctx, [Option("text", "The text to display.")] string text)
    {
        try
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(""));


            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = text,
            };

            await ctx.Channel.SendMessageAsync(embed:embedMessage);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally 
        {

        }

    }
}
