#load "..\EnumerableExtensions.csx"

#r "Newtonsoft.Json"

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

// Days for alarm before expired
private static readonly TimeSpan limitDaysBefore = TimeSpan.FromDays(30);
// 対象サイト
private static readonly string[] sites = new[]
{
    "https://google.com/", // Sample 
};

public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"SSLCertificateExpireCheck timer triggered!");

    var result = (await Task.WhenAll(sites.Select(async x =>
        {
            return await Task.Run(() =>
            {
                var checker = new CertificateExpireChecker(x);
                checker.ReadX509Certificate2();
                return checker;
            });
        })))
        .ToArray();

    // Not Existing... :(
    var notExistsSites = result.Where(x => !x.IsExists).ToArray();

    // SSL Expired.... :(
    var expiredSites = result.Where(x => x.IsExists).Where(x => x.IsExpired).ToArray();

    // SSL Expired date is comming.... :(
    var nearlyExpiredSites = result.Where(x => x.IsExists).Where(x => !x.IsExpired).Where(x => x.DaysLeft < limitDaysBefore).ToArray();

    // Do anything you want. Here's sample to create message for Slack.
    IEnumerable<string> messages = new string[] { };
    if (notExistsSites.Any())
    {
        messages = messages.Concat(new[] { "- Site not exists. Be fore site is correct to check SSL Certificate." });
        messages = messages.Concat(notExistsSites.Select(x => $@"  {nameof(x.Url)} : {x.Url}, {nameof(x.IsExists)} : {x.IsExists}"));
    }

    if (expiredSites.Any())
    {
        messages = messages.Concat(new[] { "- SSL Certificate seems expired...!!" });
        messages = messages.Concat(expiredSites.Select(x => $@"  {nameof(x.Url)} : {x.Url}, {nameof(x.IsExpired)} : {x.IsExpired}, {nameof(x.ExpiredDate)} : {x.ExpiredDate}, {nameof(x.CommonName)}, {x.CommonName}, {nameof(x.Issuer)} : {x.Issuer}"));
    }

    if (nearlyExpiredSites.Any())
    {
        messages = messages.Concat(new[] { "- SSL Certificate will be expired near future." });
        messages = messages.Concat(nearlyExpiredSites.Select(x => $@"  {nameof(x.Url)} : {x.Url}, {nameof(x.IsExpired)} : {x.IsExpired}, {nameof(x.DaysLeft)} : {x.DaysLeft.Value.TotalDays}, {nameof(x.ExpiredDate)} : {x.ExpiredDate}, {nameof(x.CommonName)}, {x.CommonName}, {nameof(x.Issuer)} : {x.Issuer}"));
    }

    var message = messages.ToJoinedString("\r\n");

    if (!notExistsSites.Any() && !expiredSites.Any() && !nearlyExpiredSites.Any())
    {
        log.Info($"There are no limit SSLs. SSLCertificateChecker finished without sending notification. Message : {message}");
        return;
    }

    // Send warning to Slack as you like!
    // YOUR SLACK SEND LOGIC WILL BE COME HERE.
}

public class CertificateExpireChecker
{
    private TimeSpan TimeOut => TimeSpan.FromSeconds(5);

    public string Url { get; }
    public DateTime LimitDate => DateTime.Now;
    public bool IsExists => Certificate != null;
    public X509Certificate2 Certificate { get; private set; }
    public string CommonName => Certificate?.SubjectName.Name.Split(',').FirstOrDefault(x => x.StartsWith("CN="));
    public TimeSpan? DaysLeft => ExpiredDate - LimitDate;
    public bool IsExpired => DateTime.Now > Certificate?.NotAfter;
    public DateTime? ExpiredDate => Certificate?.NotAfter;
    public string Issuer => Certificate?.Issuer;
    public string Subject => Certificate?.Subject;
    public string FriendlyName => Certificate?.FriendlyName;
    public string ThumbPrint => Certificate?.Thumbprint;
    public string SignatureAlgorithm => Certificate?.SignatureAlgorithm.FriendlyName;

    public CertificateExpireChecker(string url)
    {
        Url = url;
    }

    public void ReadX509Certificate2()
    {
        var request = WebRequest.Create(Url) as HttpWebRequest;
        request.Timeout = (int)TimeOut.TotalMilliseconds;
        try
        {
            using (var response = request.GetResponse()) { }
        }
        catch (Exception)
        {
            return;
        }
        var cert2 = new X509Certificate2(request.ServicePoint.Certificate);
        Certificate = cert2;
    }
}