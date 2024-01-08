using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

try
{
    var firebaseAppOptions = new AppOptions
    {
        Credential = GoogleCredential.FromFile("/Users/mitro/credentials/credentials.json"),
    };

    FirebaseApp.Create(firebaseAppOptions);

    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

    await Send(cts.Token);

    Console.WriteLine("Push notifications sent");
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());

    Console.WriteLine("Failed to send push notification");
}

async Task Send(CancellationToken cancellationToken)
{
    string[] tokens =
    [
        //"cTH6Kj9sQQuLeNSOLadMo8:APA91bHC84SEbHBKgT61bTbdiDrMMnRH6XnzOD5AHKjrlxTx_DkmId6McLJ3mj90_ottiJ2aBbeetd2WqPMdeDAmVlk3z0iZC1a1rN3cu6RY4cwUs6UvUnNkf1H0-GfBWSgHtPXAJCxA",
        //"foKqjac1RB2CRRtb3yUxL6:APA91bE-P-i_JLoKqo7dwF3IZ2S_-_Avu2A9NNvqy080aAY8HfBtZ-BpykBelrts5k0ZykNxdQ_PzdhA1uh09PCWAv5Wp7gKSyYnAFAjAaXhUgSdwbJuG67vtgiXxDKNeUcee0t3-IuD",
        //"eAPaSpw4T2uuuLuve02TTe:APA91bGA3yi2CSR4hikJZ23B3TrPL8-EipvNqUFWiGyAHfgxP4nMOImOXdLW69pcNui3oemfOvCG1UeC1aPJAKtUbZVMORaMkxR-6eifuodFqpwXdTF7xP3L2rtdKZVv5-7tB92Q6XPq",
        //"d_sGo4eBSw6OtZpnSoVHSi:APA91bEeGJVrgKET84Rk94PuyjECxBVY4rFXg6xtjeSi43WWVufSEXWB8M-vCltYpkfzksS81FYvpyL3B3_kxTb3eOcsJMBz6X5oCWEMfLr1EDzFWqXhR-MWU2Z95m32-V4nS27npHKK",
        //"e3rbG4tAQU6fjtF_MSwVKD:APA91bHSeT0UXarDAlRWoa4bQt7Zc5glGW4GSd9Z3eGzoOIAz6T3ZL6Jb8yPePi1psQpjROqR-FrXdjfsxmix-aJ11KoabLLNB-LubbCtKcWrdO2icrNoC_2jP5Ze6XOKaNT7dX7dFmx",
        "e3rbG4tAQU6fjtF_MSwVKD:APA91bHdMaBzuQa80cxSLJE8k3aqZWG-n40NH4zOT-6RmjQs6Zw8eJmzXdISi_opvbwkagunm7fm_DCXFkRSneVlXB8JFjd1-pEGB_FnRZBXIgpWLMdljbDl8lgqBGLLqrBU8abD3Kgn",
    ];

    var data = new Dictionary<string, string>
    {
        { "titleText", "Notif" },
        { "bodyText", "Notif body" },
        { "notificationType", "SellLimitOrderPlaced" }
    };

    foreach (var token in tokens)
    {
        try
        {
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(new Message
            {
                Data = data,
                Notification = new()
                {
                    Title = "Test title",
                    Body = "Test body",
                },
                Token = token,
            }, cancellationToken);

            if (string.IsNullOrWhiteSpace(response))
            {
                Console.WriteLine($"Failed to send push notification to token {token}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
