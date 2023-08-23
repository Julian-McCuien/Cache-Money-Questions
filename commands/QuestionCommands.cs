using Cache_Money_Questions.other;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus;

namespace Cache_Money_Questions.commands
{
    public class QuestionCommands : BaseCommandModule // <-- this class is like a structure that allows out class to have commands in it 
    {
        UserRepository userRepository;
        public QuestionCommands()
        {
            userRepository = new UserRepository();
        }

        private Question lastQuestion;
        private static SqlConnection _dbConnection;
        UserProgress newUser;

        public static async Task<Question> GetRandomQuestion(string type, string difficulty) // linq to get question
        {
            var jsonReader = new JSONReader(); // creating an instance of my JSONreader class to get the connection string and bot token
            await jsonReader.ReadJSON();

            _dbConnection = new SqlConnection(jsonReader.connectionString);
            _dbConnection.Open(); // opening connection to SQL 

            string query = "SELECT TOP 1 * FROM dbo.Questions " +
                           "WHERE Type = @Type AND Difficulty = @Difficulty " +
                           "ORDER BY NEWID()"; // NEWID() generates a random order

            using (var cmd = new SqlCommand(query, _dbConnection))
            {
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Difficulty", difficulty);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        return new Question
                        {
                            QuestionID = (int)reader["QuestionID"],
                            QuestionText = (string)reader["QuestionText"],
                            Answer = (string)reader["Answer"],
                            Type = (string)reader["Type"],
                            difficulty = (string)reader["Difficulty"]
                        };
                    }
                    return null; // No matching question found
                }
            }
        }
        public static DiscordEmbed CreateQuestionEmbed(Question question) // format for how I want to create a discord embed
        {
            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Question")
                .WithDescription(question.Type == "whiteboard" ? $"```\n{question.QuestionText}\n```" : question.QuestionText)
                .AddField("Type", question.Type)
                .AddField("Difficulty", question.difficulty)
                .WithColor(DiscordColor.Blue);

            return embedBuilder.Build();
        }

        [Command("getquestion")] // Command class used to declare name of command 
        public async Task GetQuestion(CommandContext ctx, string type, string difficulty) // commandContext contains all Discord methods 
        {
            lastQuestion = await GetRandomQuestion(type, difficulty); // storing the question in a variable to get the answer for later

            if (lastQuestion != null)
            {
                var embed2 = CreateQuestionEmbed(lastQuestion);
                await ctx.RespondAsync(embed: embed2);  // If there was a valid question, write as embed in Discord
                userRepository.UpdateUser(ctx.User.Username);  // Add new user if new to DB, if not update user
            }
            else
            {
                await ctx.RespondAsync("No question found with the specified criteria.");
            }
        }
        [Command("showanswer")]
        public async Task ShowAnswer(CommandContext ctx)
        {
            if (lastQuestion != null)
            {
                string answerText = lastQuestion.Type == "whiteboard" ? $"```\n{lastQuestion.Answer}\n```" : lastQuestion.Answer;

                var answerEmbed = new DiscordEmbedBuilder()
                    .WithTitle("Answer")
                    .WithDescription(answerText)
                    .WithColor(DiscordColor.Green);

                await ctx.RespondAsync(embed: answerEmbed);
            }
            else
            {
                await ctx.RespondAsync("No question retrieved yet.");
            }
        }

        [Command("addquestion")]
        public async Task AddQuestion(CommandContext ctx)
        {
            var UserSubmitting = ctx.User.Username;


            await ctx.RespondAsync("What type is the question? \n (technical, whiteboard or behavioral)");
            var questionTypeResponse = await WaitForUserResponse(ctx);

            await ctx.RespondAsync("What difficulty is the question? \n (beginner, Intermediate or advanced (type \"na\" for behavioral)");
            var questionDifficultyResponse = await WaitForUserResponse(ctx);

            // Ask the user for question text
            await ctx.RespondAsync("Please provide the question text:");
            var questionTextResponse = await WaitForUserResponse(ctx);

            // Ask the user for answer
            await ctx.RespondAsync("Please provide the answer:");
            var answerResponse = await WaitForUserResponse(ctx);

            // Ask the user to confirm
            var confirmEmbed = new DiscordEmbedBuilder()
                .WithTitle("Confirm Question")
                .WithDescription($"Type: {questionTypeResponse}\nDifficulty: {questionDifficultyResponse}\nQuestion Text: {questionTextResponse}\nAnswer: {answerResponse}\n\nType 'yes' to confirm, or 'no' to cancel.")
                .WithColor(DiscordColor.Gray);
            await ctx.RespondAsync(embed: confirmEmbed);

            // Wait for user confirmation
            var confirmation = await WaitForUserResponse(ctx);
            if (confirmation.ToLower() == "yes")
            {
                // Get the next available QuestionID
                int nextQuestionID = userRepository.GetMaxQuestionID() + 1;

                // Create the new question
                var newQuestion = new Question
                {
                    QuestionID = nextQuestionID,
                    QuestionText = questionTextResponse,
                    Answer = answerResponse,
                    Type = questionTypeResponse,
                    difficulty = questionDifficultyResponse

                };
                // Add the question to the database
                userRepository.AddNewQuestion(newQuestion);
                await ctx.RespondAsync("Question added successfully!");

                var loggingChannelId = 1141921394351931402;
                var loggingChannel = await ctx.Client.GetChannelAsync(1141921394351931402);
                var loggingMessage = $"User '{UserSubmitting}' submitted a question:\n" +
                             $"Type: {questionTypeResponse}\n" +
                             $"Difficulty: {questionDifficultyResponse}\n" +
                             $"Question Text: {questionTextResponse}\n" +
                             $"Answer: {answerResponse}";
                await loggingChannel.SendMessageAsync(loggingMessage);

            }
            else
            {
                await ctx.RespondAsync("Question addition canceled.");
            }

        }

        private async Task<string> WaitForUserResponse(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            // Wait for a message from the same user
            var response = await interactivity.WaitForMessageAsync(x =>
                x.Author.Id == ctx.User.Id && x.ChannelId == ctx.Channel.Id);

            if (response.Result != null)
            {
                return response.Result.Content;
            }
            return string.Empty;
        }


    }
}

