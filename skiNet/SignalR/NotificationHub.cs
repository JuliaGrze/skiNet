using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.Collections.Concurrent;

namespace API.SignalR
{
    [Authorize]
    // NotificationHub — to klasa SignalR Huba, który pozwala na komunikację w czasie rzeczywistym między serwerem a klientami.
    public class NotificationHub : Hub
    {
        //słownik połączeń użytkowników
        //Przechowuje mapowanie email użytkownika -> ConnectionId
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();

        //Metoda wywoływana, gdy klient się połączy z hubem
        public override Task OnConnectedAsync()
        {
            //Pobiera email aktualnie połączonego użytkownika z tokena (Context.User.GetEmail())
            var email = Context.User?.GetEmail();

            //Jeśli email istnieje, przypisuje w słowniku jego ConnectionId
            if (!string.IsNullOrEmpty(email))
                UserConnections[email] = Context.ConnectionId;

            return base.OnConnectedAsync();
        }

        //Wywoływana, gdy klient rozłącza się z hubem
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var email = Context.User?.GetEmail();

            //Usuwa z mapowania wpis użytkownika (email -> connectionId), bo użytkownik już nie jest połączony
            if (!string.IsNullOrEmpty(email))
                UserConnections.TryRemove(email, out _);

            return base.OnDisconnectedAsync(exception);
        }

        //Pozwala innym klasom (np. kontrolerom) pobrać ConnectionId użytkownika po jego emailu
        public static string? GetConnectionIdByEmail(string email)
        {
            UserConnections.TryGetValue(email, out var connectionId);

            return connectionId;
        }
    }
}
