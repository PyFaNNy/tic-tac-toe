using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tic_tac_toe.Models;
using TicTacToeGame;
using Microsoft.AspNetCore.Identity;
using System.Threading;

namespace tic_tac_toe
{
    public class GameHub : Hub
    {

        private static readonly ConcurrentBag<Game> games =
             new ConcurrentBag<Game>();

        public async Task RegisterGame(string Tag, string Email)
        {
            Player Player2 = new Player();
            Player Player1 = new Player
            {
                ConnectionId = Context.ConnectionId,
                Email = Email,
                WaitingForMove = false,
                game = Tag
            };
            var game = new Game()
            {
                Player1 = Player1,
                Player2 = Player2,
                Tag = Tag
            };

            games.Add(game);
            await Clients.All.SendAsync("GetGames", games.ToArray());
            await Clients.Client(Context.ConnectionId).SendAsync("Redirect" ,game.Tag);
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
            await Clients.Client(Context.ConnectionId).SendAsync("FindGames", list);
        }
        public async Task ConnectGame(string data)
        {
            var splitData = data.Split(' ');
            string id = splitData[0];
            string name = splitData[1];
            string email = splitData[2];
            Game game = games.First(games => games.Tag.Equals(name) && games.Player1.Email.Equals(id));
            game.Player2.ConnectionId = Context.ConnectionId;
            game.Player2.WaitingForMove = true;
            game.Player2.Opponent = game.Player1;
            game.Player2.Email = email;
            game.Player2.game = game.Tag;
            game.Player1.Opponent = game.Player2;
            

            await Clients.Client(Context.ConnectionId).SendAsync("Redirect", game.Tag);
        }
        public void MakeAMove(int position, string Email)
        {
            Game game = games.First(games => games.Player1.Email.Equals(Email) || games.Player2.Email.Equals(Email));

            int symbol = 0;

            if (game .Player2.Email.Equals(Email))
            {
                symbol = 1;
            }

            var player = symbol == 0 ? game.Player1 : game.Player2;
            player.ConnectionId= Context.ConnectionId; 
            if (player.WaitingForMove)
            {
                return;
            }

            if (game.Play(symbol, position))
            {
                Clients.Client(game.Player1.ConnectionId).SendAsync("GameOver", $"The winner is {player.Email}");
                Clients.Client(game.Player2.ConnectionId).SendAsync("GameOver", $"The winner is {player.Email}");
                Clients.Client(game.Player1.ConnectionId).SendAsync("moveMade", game.field);
                Clients.Client(game.Player2.ConnectionId).SendAsync("moveMade", game.field);
                Thread.Sleep(5000);
                this.Clients.Client(player.ConnectionId).SendAsync("RedirectHome");
                this.Clients.Client(player.Opponent.ConnectionId).SendAsync("RedirectHome");
                games.TryTake(out game);
            }
            Clients.Client(game.Player1.ConnectionId).SendAsync("moveMade", game.field);
            Clients.Client(game.Player2.ConnectionId).SendAsync("moveMade",  game.field);


            if (!game.IsOver)
            {
                player.WaitingForMove = !player.WaitingForMove;
                player.Opponent.WaitingForMove = !player.Opponent.WaitingForMove;

                Clients.Client(player.Opponent.ConnectionId).SendAsync(Constants.WaitingForOpponent, player.Opponent.Email);
                Clients.Client(player.ConnectionId).SendAsync(Constants.WaitingForOpponent, player.Opponent.Email);
            }
        }

    }
}
