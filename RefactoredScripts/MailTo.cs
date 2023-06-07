using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using UnityEngine;

public class MailTo : MonoBehaviour {
    [Header("Destination")] 
    [SerializeField]
    private string destinationMail = "KilianBonnet@cunet.carleton.ca";

    [Header("SMTP Configuration")]
    [SerializeField] private string smtpUsername = "lab.logs@laposte.net";
    [SerializeField] private string smtpServer = "smtp.laposte.net";
    [SerializeField] private int smtpPort = 587;
    [SerializeField] private string smtpPassword = "L@b.logs123!";

    /// <summary>
    /// Send a report mail witch attached player data.
    /// </summary>
    /// <param name="player">Player of the corresponding data</param>
    /// <param name="filePaths">Path of the documents to attach</param>
    public void SendEmail(Player player, string[] filePaths)
    {
        // Building mail
        MailAddress fromAddress = new MailAddress(smtpUsername, "HCI Lab");
        MailAddress toAddress = new MailAddress(destinationMail);
        
        MailMessage mail = new MailMessage(fromAddress, toAddress);
        mail.Subject = $"User data report ({player.playerName})";
        mail.Body = $"Attached, data from {player.playerName} recorded at {DateTime.Now.ToString(CultureInfo.CurrentCulture)}";
        
        // Adding attachments
        foreach (string filePath in filePaths)
        {
            Attachment attachment = new Attachment(filePath);
            mail.Attachments.Add(attachment);
        }
        
        // SMTP client configuration
        SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
        smtpClient.EnableSsl = true;

        try {
            smtpClient.Send(mail);
            Debug.Log("Mail sent!");
        }
        catch (Exception ex) {
            Debug.Log("Error when sending mail: " + ex.Message);
        }
    }
}
