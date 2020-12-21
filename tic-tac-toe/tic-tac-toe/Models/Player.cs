using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tic_tac_toe.Models
{
    internal class Player
    {
        public Player Opponent { get; set; }
        public bool WaitingForMove { get; set; }
        public string ConnectionId { get; set; }
    }
}
