using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
using System;
using System.Collections;
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

        private static readonly Random toss = new Random();


        public async Task RegisterGame(string Tag)
        {
            var player1 = new Player
            {
                ConnectionId = Context.ConnectionId,
                
            };
            var game = new Game()
            {
                player1 = player1,
                Tag = Tag
            };

            games.Add(game);
            await Clients.All.SendAsync("GetGames", game);
        }
        public async Task FindGame(string Tag)
        {
            ArrayList list = new ArrayList();
            var i = games.ToArray();
            foreach (Game fVar in i)
            {
                if (fVar.Tag.Equals(Tag))
                {
                    list.Add(fVar);
                }
            }
            await Clients.All.SendAsync("FindGames", list);
        }
    }
}
