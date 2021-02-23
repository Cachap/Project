using AngleSharp.Html.Parser;
using System;
using System.Net;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace Bot
{
	class Program
	{
		static VkApi api = new VkApi();

		static string loadAnecdot(string answer)
		{
			if (answer == null)
			{
				return null;
			}
			var parser = new HtmlParser();
			Random rnd = new Random();
			string[] anectod;
			int count = 0;

			var texts = parser.ParseDocument(answer).GetElementsByClassName("tecst");
			anectod = new string[texts.Length];

			foreach (var text in texts)
			{
				foreach (var tmp in text.GetElementsByClassName("wrrating"))
				{
					tmp.Remove();
				}
				text.TextContent = text.TextContent.Replace("\n", "");
				anectod[count++] = text.TextContent;
			}
			return (anectod[rnd.Next() % texts.Length]);
		}

		static void SendMessage(string message, long? userID)
		{
			Random rnd = new Random();
			api.Messages.Send(new MessagesSendParams
			{
				RandomId = rnd.Next(),
				UserId = userID,
				Message = message
			});

		}

		//static void sendGif(string attachment, long? userID)
		//{
		//	Random rnd = new Random();
		//	api.Messages.Send(new MessagesSendParams
		//	{
		//		RandomId = rnd.Next(),
		//		UserId = userID,
		//		Attachments = "doc{userID}{}"
		//	}
		//		);
			
		//}

		static void Main(string[] args)
		{
			api.Authorize(new ApiAuthParams() { AccessToken = "e2ffe21e7f8ef8fa331f3c0eabdefefd724e851072251b537e2c151666aada41b1e6ce9497352707de801" });
			var s = api.Groups.GetLongPollServer(201782383);
			WebClient wc = new WebClient();
			Random rnd = new Random();

			while (true) 
			{
				var poll = api.Groups.GetBotsLongPollHistory(
			   new BotsLongPollHistoryParams()
			   { Server = s.Server, Ts = s.Ts, Key = s.Key, Wait = 25 });
				s.Ts = poll?.Ts;
				if (poll?.Updates == null) continue; 
				
				foreach (var a in poll.Updates)
					{
						if (a.Type == GroupUpdateType.MessageNew)
						{
							string userMessage = a.MessageNew.Message.Text.ToLower();
							long? userID = a.MessageNew.Message.PeerId;
							if (userMessage == "анекдот")
							{
								SendMessage(loadAnecdot(wc.DownloadString($"https://anekdotbar.ru/page/{rnd.Next() % 535}/")), userID);
							}
						}
					
					}
			}
		}
	}
}
