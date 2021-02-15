using System;
using System.IO;
using System.Linq;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System.Drawing;

namespace TB
{
    class Program
    {
        private static Telegram.Bot.TelegramBotClient BOT;
        
        static void Main(string[] args)
        {
            BOT = new Telegram.Bot.TelegramBotClient("");

            BOT.OnMessage += BotOnTxtMessageReceived;
            BOT.OnMessage += BotOnImgMessageReceived;
            BOT.StartReceiving();

            Console.ReadKey();
            BOT.StopReceiving();
        }

        private static async void BotOnImgMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            Telegram.Bot.Types.Message msg = messageEventArgs.Message;
            if (msg != null && msg.Type == MessageType.Photo)
            {
                //QRCodeDecoder decoder = new QRCodeDecoder();
                //await BOT.SendTextMessageAsync(msg.Chat.Id, decoder.Decode(new QRCodeBitmapImage(pictureBox1.Image as Bitmap)));
                await BOT.SendTextMessageAsync(msg.Chat.Id, "Да");
            }
        }

        private static async void BotOnTxtMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            Telegram.Bot.Types.Message msg = messageEventArgs.Message;
            if (msg != null && msg.Type == MessageType.Text)
            {
                string message;
                if (msg.Text.Contains(' ')) message = msg.Text.Substring(0,msg.Text.IndexOf(' '));
                else message = msg.Text;

                switch (message)
                {
                    case "/start":
                        await BOT.SendTextMessageAsync(msg.Chat.Id, "/sedinstr1 - Инструкция по созданию проекта исходящего письма, Дело-WEB\r\n" +
                                                                    "/sedinstr2 - Инструкция по созданию проекта исходящего письма, Дело-Предприятие\r\n" +
                                                                    "/map - Как нас найти\r\n" +
                                                                    "/phone имя - Номер телефона сотрудника ГКУ ЦИТ\r\n" +
                                                                    "/support - Техническая поддержка");
                        break;
                    case "/sedinstr1":
                        var stream = File.OpenRead("1_nstrukciya_po_sozdaniyu_proekta_ishodyashhego_pisma_elo_web.pdf");
                        InputOnlineFile iof = new InputOnlineFile(stream);
                        iof.FileName = "Инструкция по созданию проекта исходящего письма, Дело-WEB";
                        await BOT.SendDocumentAsync(msg.Chat.Id, iof, "Инструкция по созданию проекта исходящего письма, Дело-WEB");
                        break;

                    case "/sedinstr2":
                        var stream2 = File.OpenRead("2_nstrukciya_po_sozdaniyu_proekta_ishodyashhego_pisma_elo_predpriyatie.pdf");
                        InputOnlineFile iof2 = new InputOnlineFile(stream2);
                        iof2.FileName = "Инструкция по созданию проекта исходящего письма, Дело-Предприятие";
                        await BOT.SendDocumentAsync(msg.Chat.Id, iof2, "Инструкция по созданию проекта исходящего письма, Дело-Предприятие");
                        break;

                    case "/map":
                        await BOT.SendLocationAsync(msg.Chat.Id, (float)54.622216, (float)39.759228);
                        break;

                    case "/phone":
                        AppContext db;
                        db = new AppContext();
                        string subName = msg.Text.Substring(msg.Text.IndexOf(' ')+1, msg.Text.Length - msg.Text.IndexOf(' ')-1);
                        
                        var result = db.PhoneNumber.Where(p => p.Name.Contains(subName));

                        if (result.Count() == 0)
                        {
                            await BOT.SendTextMessageAsync(msg.Chat.Id, "Номер не найден");
                            break;
                        }
                        else
                        {
                            foreach (PhoneNumber r in result)
                            {
                                await BOT.SendTextMessageAsync(msg.Chat.Id, r.Name.ToString()+" "+r.Phone.ToString());
                            };

                            /*try
                            {
                                foreach (PhoneNumber r in result)
                                {
                                    await BOT.SendContactAsync(msg.Chat.Id, r.Phone, r.Name);
                                };
                            }
                            catch
                            {
                                await BOT.SendTextMessageAsync(msg.Chat.Id, "Отправлено слишком много запросов. Повторите через 24 часа.");
                            }*/

                            break;
                        }

                    default: 
                        await BOT.SendTextMessageAsync(msg.Chat.Id, "Такой команды не существует"); break;
                }
            }
        }
    }
}
