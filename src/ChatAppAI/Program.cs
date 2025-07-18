﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
string endpoint = config["AZURE_OPENAI_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not found");
string deployment = config["AZURE_OPENAI_GPT_NAME"] ?? throw new InvalidOperationException("AZURE_OPENAI_GPT_NAME not found");
string apiKey = config["AZURE_OPENAI_API_KEY"] ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY not found");

IChatClient client =
    new AzureOpenAIClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey))
        .GetChatClient(deployment)
        .AsIChatClient();

// Start the conversation with context for the AI model
List<Microsoft.Extensions.AI.ChatMessage> chatHistory =
    [
        new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.System, """
            You are a friendly hiking enthusiast who helps people discover fun hikes in their area.
            You introduce yourself when first saying hello.
            When helping people out, you always ask them for this information
            to inform the hiking recommendation you provide:

            1. The location where they would like to hike
            2. What hiking intensity they are looking for

            You will then provide three suggestions for nearby hikes that vary in length
            after you get that information. You will also share an interesting fact about
            the local nature on the hikes when making a recommendation. At the end of your
            response, ask if there is anything else you can help with.
        """)
    ];

    // Loop to get user input and stream AI response
while (true)
{
    // Get user prompt and add to chat history
    Console.WriteLine("Your prompt:");
    string? userPrompt = Console.ReadLine();
    chatHistory.Add(new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, userPrompt));

    // Stream the AI response and add to chat history
    Console.WriteLine("AI Response:");
    string response = "";
    await foreach (var item in
        client.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);
        response += item.Text;
    }
    chatHistory.Add(new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.Assistant, response));
    Console.WriteLine();
}