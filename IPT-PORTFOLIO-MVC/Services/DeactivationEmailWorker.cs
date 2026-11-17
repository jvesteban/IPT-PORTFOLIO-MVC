using System.Net;
using System.Net.Mail;
using IPT_PORTFOLIO_MVC.Data;
using Microsoft.EntityFrameworkCore;

namespace IPT_PORTFOLIO_MVC.Services;

public sealed class DeactivationEmailWorker(IServiceScopeFactory scopes, IConfiguration config,
    ILogger<DeactivationEmailWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var host = config["Smtp:Host"];
                var sender = config["Smtp:From"];
                if (!string.IsNullOrWhiteSpace(host) && MailAddress.TryCreate(sender, out var from)
                    && await DeliverNext(host, from, stoppingToken)) continue;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Unable to process pending deactivation notifications.");
            }
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task<bool> DeliverNext(string host, MailAddress from, CancellationToken token)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await using var tx = await db.Database.BeginTransactionAsync(token);
        // Reserve one message until SMTP accepts it or the retry time is recorded.
        var pending = await db.PendingDeactivationEmails.FromSqlRaw("""
            SELECT TOP (1) * FROM dbo.PendingDeactivationEmails WITH (UPDLOCK, ROWLOCK)
            WHERE NextAttemptAt <= SYSUTCDATETIME() ORDER BY NextAttemptAt, UserId
            """).FirstOrDefaultAsync(token);
        if (pending is null) return false;

        using var message = new MailMessage(from, new MailAddress(pending.Recipient))
        {
            Subject = "Your IPT Portfolio account has been deactivated",
            Body = $"Your IPT Portfolio administrator has deactivated your student account.\n\nReason: {pending.Reason}\n\nYou can contact your portfolio administrator to ask about reactivation.",
            BodyEncoding = System.Text.Encoding.UTF8
        };
        using var smtp = new SmtpClient(host, config.GetValue("Smtp:Port", 587))
        {
            EnableSsl = config.GetValue("Smtp:EnableSsl", true)
        };
        if (!string.IsNullOrWhiteSpace(config["Smtp:Username"]))
            smtp.Credentials = new NetworkCredential(config["Smtp:Username"], config["Smtp:Password"]);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeout.CancelAfter(TimeSpan.FromSeconds(25));
        try
        {
            await smtp.SendMailAsync(message, timeout.Token);
            db.PendingDeactivationEmails.Remove(pending);
        }
        catch (Exception ex) when (!token.IsCancellationRequested &&
            ex is SmtpException or OperationCanceledException or System.Net.Sockets.SocketException)
        {
            pending.NextAttemptAt = DateTime.UtcNow.AddMinutes(5);
            logger.LogWarning("SMTP delivery failed for account {UserId}. A retry is scheduled.", pending.UserId);
        }
        await db.SaveChangesAsync(token);
        await tx.CommitAsync(token);
        return true;
    }
}
