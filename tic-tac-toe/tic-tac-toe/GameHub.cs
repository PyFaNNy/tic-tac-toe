using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tic_tac_toe.Models;

namespace tic_tac_toe
{
    public class GameHub : Hub
    {
        private static readonly ConcurrentBag<Game> games =
             new ConcurrentBag<Game>();

        public void RegisterGame(string Tag)
        {
            string name = Tag;
            var player = new Player
            {
                ConnectionId = Context.ConnectionId,
                Name = name,
                IsPlaying = false,
                IsSearchingOpponent = true
            };
        }

    }
}
